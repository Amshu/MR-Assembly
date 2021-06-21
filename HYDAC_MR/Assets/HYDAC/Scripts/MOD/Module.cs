using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine.Serialization;

namespace HYDAC.Scripts.MOD
{
    public sealed class Module : MonoBehaviour, IModule
    {
        private const string MachinePartInfoFolderPath = "MachinePartInfos";
        
        [Header("Assembly Members")]
        [SerializeField] private SocMainSettings mUnitSettings;

        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        public int NoOfSteps => mNoOfSteps;

        private ObjectManipulator _objectManipulator = null;
        private MoveAxisConstraint _moveAxisConstraint = null;
        private BoundsControl _boundsControl = null;

        private bool _isInFocus = false;
        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;
        private Vector3 _defaultScale;
        
        private ISubModule[] _subModules;
        
        [Header("Debug")]
        public bool isExploded;
        [FormerlySerializedAs("currentUnitNo")] public int currentModNo;
        public int startingPosition;

        private void Awake()
        {
            _isInFocus = false;
            
            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation;
            _defaultScale = transform.localScale;

            _objectManipulator = GetComponent<ObjectManipulator>();
            _moveAxisConstraint = GetComponent<MoveAxisConstraint>();
            _boundsControl = GetComponent<BoundsControl>();
            
            GetMachineParts();

            isExploded = false;
            currentModNo = startingPosition;
        }

        private void OnEnable()
        {
            _objectManipulator.OnManipulationStarted.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _objectManipulator.OnManipulationStarted.RemoveListener(OnClicked);
        }


        private void GetMachineParts()
        {
            // Load from Resources
            var machinePartInfos = Resources.LoadAll(MachinePartInfoFolderPath, typeof(ISubModule));
            if (machinePartInfos.Length < 1)
            {
                Debug.LogError("No Machine part infos found or loaded. Exiting Application");
                Application.Quit();
                return;
            }

            // Cast each loaded object to IMachinePart
            List<ISubModule> parts = new List<ISubModule>();
            foreach(object ogj in machinePartInfos)
            {
                parts.Add(ogj as ISubModule);
            }

            // Set to main array
            _subModules = parts.ToArray();
            if (_subModules.Length < 1)
            {
                Debug.LogError("Error in casting to IMachinePart[]. Exiting Application");
                Application.Quit();
                return;
            }

            // Sort all parts by their assembly position
            _subModules = _subModules.OrderBy(x => x.GetUnitPosition()).ToArray();

            // Get total number of assemblies
            mNoOfSteps = _subModules[_subModules.Length - 1].GetUnitPosition();
        }
        
        
        private void OnClicked(ManipulationEventData arg0)
        {
            if (_isInFocus) return;

            Debug.Log("Object Clicked: " + name);
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IModule).ToggleFocus(true);
        }
        
        
        public event Action<Module> OnFocused;
        private void RaiseOnFocused(Module managerRef)
        {
            OnFocused?.Invoke(managerRef);
            
            _isInFocus = true;
        }


        #region  IMacUnit Implementation


        void IModule.ToggleFocus(bool toggle)
        {
            _isInFocus = toggle;
            
            // Switch components
            _moveAxisConstraint.enabled = !toggle;
            _boundsControl.enabled = toggle;
            
            // If toggle focus = false then
            // - Scale Down
            if (!toggle)
            {
                // Disappear
                StopAllCoroutines();
                StartCoroutine(LerpVector3(transform.localScale,Vector3.zero, 1, result =>
                {
                    transform.localScale = result;
                }));
            }
        }

        void IModule.Reset()
        {
            // If this assembly was in focus then
            // - Reset assembly position to 0 - Implode
            // - Lerp Position
            // - Lerp Rotation
            // - SwitchOff Components
            if (_isInFocus)
            {
                ToggleExplode(false, mUnitSettings.positionTimeChange);
                
                // Switch components
                _moveAxisConstraint.enabled = true;
                _boundsControl.enabled = false;

                Debug.Log("#MacUNIT#----------------Lerp Position " + name);
                
                StopAllCoroutines();

                // Reset Position
                StartCoroutine(LerpVector3(transform.position,_defaultPosition, 1, result =>
                {
                    transform.position = result;
                }));
                
                // Reset rotation
                StartCoroutine(LerpQuaternion(transform.rotation,_defaultRotation, 1, result =>
                {
                    transform.rotation = result;
                }));
            }
            // Else
            // - Lerp scale back to normal
            else
            {
                Debug.Log("#MacUNIT#----------------Lerp Scale " + name);
                
                // Reset Scale
                StopAllCoroutines();
                StartCoroutine(LerpVector3(transform.localScale,_defaultScale, 1, result =>
                {
                    transform.localScale = result;
                }));
            }

            _isInFocus = false;
        }

        void IModule.ToggleExplode()
        {
            if (!_isInFocus)
                return;
            
            isExploded = !isExploded;
            ToggleExplode(isExploded, mUnitSettings.positionTimeChange);
        }

        void IModule.ChangePosition(int step)
        {
            if (!_isInFocus)
                return;
            
            int x = Mathf.Clamp(currentModNo + step, 0, mNoOfSteps);
            ChangeCurrentUnitPosition((x));
        }

        #endregion


        private void ToggleExplode(bool toggle, float positionTimeChange)
        {
            currentModNo = (toggle) ? _subModules.Length - 1 : 0;
            
            foreach(ISubModule part in _subModules)
            {
                // If the current state is exploded then: 
                // Implode
                if (toggle)
                {
                    part.ToggleExplode(true, positionTimeChange);
                }
                // Explode
                else
                {
                    part.ToggleExplode(false, positionTimeChange);
                }
            }
        }
        
        
        private void ChangeCurrentUnitPosition(int unitPosition)
        {
            Debug.Log("#MacUnit#-------------------------Changing assembly position to: " + unitPosition);

            currentModNo = unitPosition;

            for (var i = 0; i < _subModules.Length; i++)
            {
                var part = _subModules[i];
                var partPosition = part.GetUnitPosition();

                // If its less than or equal to the passed unit position => Explode
                if (partPosition <= unitPosition)
                {
                    part.ToggleExplode(true, mUnitSettings.positionTimeChange);

                    // Highlight previous part
                    if (partPosition == unitPosition - 1)
                    {
                        part.ChangeMaterial(true, mUnitSettings.previousUnitMaterial);
                    }
                    // Highlight current part
                    else if(partPosition == unitPosition)
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                }
                // If its greater than the passed unit position => Implode
                else
                {
                    part.ToggleExplode(false, mUnitSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == unitPosition + 1)
                    {
                        part.ChangeMaterial(true, mUnitSettings.nextUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                }
            }
        }
        
        
        private IEnumerator LerpVector3(Vector3 start, Vector3 end, float timeTaken, Action<Vector3> updateCall)
        {
            var t = 0f;

            while (t < 1)
            {
                //Debug.Log("#MacUnit#---------------Lerping from " + start + " to " + end);
                
                t += Time.deltaTime / timeTaken;

                Vector3 result = Vector3.Lerp(start, end, t);
                
                updateCall?.Invoke(result);
                
                yield return null;
            }

            //_mLock = false;
        }
        
        private IEnumerator LerpQuaternion(Quaternion start, Quaternion end, float timeTaken, Action<Quaternion> updateCall)
        {
            var t = 0f;

            while (t < 1)
            {
                //Debug.Log("#MacUnit#---------------Lerping from " + start + " to " + end);
                
                t += Time.deltaTime / timeTaken;

                Quaternion result = Quaternion.Lerp(start, end, t);
                
                updateCall?.Invoke(result);
                
                yield return null;
            }

            //_mLock = false;
        }
        
        
        private void OnSliderUpdate(SliderEventData sliderData)
        {
            float assemblyPosition = sliderData.NewValue * mNoOfSteps;

            //Debug.Log("#MainManager#-------------------------OnSliderUpdate: " + sliderData.NewValue + " = " + unitPosition);

            //ChangeCurrentUnitPosition((int)unitPosition);
        }
    }
}
