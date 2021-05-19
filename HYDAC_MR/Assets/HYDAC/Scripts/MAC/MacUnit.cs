using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

namespace HYDAC.Scripts.MAC
{
    public sealed class MacUnit : MonoBehaviour, IMacUnit
    {
        private const string MachinePartInfoFolderPath = "MachinePartInfos";
        
        [Header("Assembly Members")]
        [SerializeField] private SocMainSettings unitSettings;

        [SerializeField] private ObjectManipulator objectManipulator = null;
        [SerializeField] private MoveAxisConstraint moveAxisConstraint = null;
        [SerializeField] private BoundsControl boundsControl = null;

        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        public int NoOfSteps => mNoOfSteps;

        [Header("Debug")]
        public bool isExploded;
        public int currentUnitNo;
        public int startingPosition;
        

        private bool _isInFocus = false;
        private Vector3 _defaultPosition;
        private Vector3 _defaultScale;
        
        private IMacUnitPart[] _unitParts;
        
        

        private void Awake()
        {
            _isInFocus = false;
            
            _defaultPosition = transform.position;
            _defaultScale = transform.localScale;

            moveAxisConstraint = GetComponent<MoveAxisConstraint>();
            boundsControl = GetComponent<BoundsControl>();
            
            GetMachineParts();

            isExploded = false;
            currentUnitNo = startingPosition;
        }

        private void OnEnable()
        {
            objectManipulator.OnManipulationStarted.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            objectManipulator.OnManipulationStarted.RemoveListener(OnClicked);
        }


        private void GetMachineParts()
        {
            // Load from Resources
            var machinePartInfos = Resources.LoadAll(MachinePartInfoFolderPath, typeof(IMacUnitPart));
            if (machinePartInfos.Length < 1)
            {
                Debug.LogError("No Machine part infos found or loaded. Exiting Application");
                Application.Quit();
                return;
            }

            // Cast each loaded object to IMachinePart
            List<IMacUnitPart> parts = new List<IMacUnitPart>();
            foreach(object ogj in machinePartInfos)
            {
                parts.Add(ogj as IMacUnitPart);
            }

            // Set to main array
            _unitParts = parts.ToArray();
            if (_unitParts.Length < 1)
            {
                Debug.LogError("Error in casting to IMachinePart[]. Exiting Application");
                Application.Quit();
                return;
            }

            // Sort all parts by their assembly position
            _unitParts = _unitParts.OrderBy(x => x.GetUnitPosition()).ToArray();

            // Get total number of assemblies
            mNoOfSteps = _unitParts[_unitParts.Length - 1].GetUnitPosition();
        }
        
        
        private void OnClicked(ManipulationEventData arg0)
        {
            if (_isInFocus) return;

            Debug.Log("Object Clicked: " + name);
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IMacUnit).ToggleFocus(true);
        }
        
        
        public event Action<MacUnit> OnFocused;
        private void RaiseOnFocused(MacUnit managerRef)
        {
            OnFocused?.Invoke(managerRef);
            
            _isInFocus = true;
        }


        #region  IMacUnit Implementation


        void IMacUnit.ToggleFocus(bool toggle)
        {
            _isInFocus = toggle;
            
            // Switch components
            moveAxisConstraint.enabled = !toggle;
            boundsControl.enabled = toggle;
            
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

        void IMacUnit.Reset(bool flag)
        {
            // If this assembly was in focus then
            // - Reset assembly position to 0 - Implode
            // - Lerp Position
            // - SwitchOff Components
            if (_isInFocus)
            {
                ToggleUnitExplode(false, unitSettings.positionTimeChange);
                
                // Switch components
                moveAxisConstraint.enabled = true;
                boundsControl.enabled = false;

                Debug.Log("#MacUNIT#----------------Lerp Position " + name);

                // Reset Position
                StopAllCoroutines();
                StartCoroutine(LerpVector3(transform.position,_defaultPosition, 1, result =>
                {
                    transform.position = result;
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

        void IMacUnit.ToggleExplode()
        {
            if (!_isInFocus)
                return;
            
            isExploded = !isExploded;
            ToggleUnitExplode(isExploded, unitSettings.positionTimeChange);
        }

        void IMacUnit.ChangeUnitPosition(int step)
        {
            if (!_isInFocus)
                return;
            
            int x = Mathf.Clamp(currentUnitNo + step, 0, mNoOfSteps);
            ChangeCurrentUnitPosition((x));
        }

        #endregion


        private void ToggleUnitExplode(bool toggle, float positionTimeChange)
        {
            currentUnitNo = (toggle) ? _unitParts.Length - 1 : 0;
            
            foreach(IMacUnitPart part in _unitParts)
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

            currentUnitNo = unitPosition;

            for (var i = 0; i < _unitParts.Length; i++)
            {
                var part = _unitParts[i];
                var partPosition = part.GetUnitPosition();

                // If its less than or equal to the passed unit position => Explode
                if (partPosition <= unitPosition)
                {
                    part.ToggleExplode(true, unitSettings.positionTimeChange);

                    // Highlight previous part
                    if (partPosition == unitPosition - 1)
                    {
                        part.ChangeMaterial(true, unitSettings.previousUnitMaterial);
                    }
                    // Highlight current part
                    else if(partPosition == unitPosition)
                    {
                        part.ChangeMaterial(false, unitSettings.currentUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, unitSettings.currentUnitMaterial);
                    }
                }
                // If its greater than the passed unit position => Implode
                else
                {
                    part.ToggleExplode(false, unitSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == unitPosition + 1)
                    {
                        part.ChangeMaterial(true, unitSettings.nextUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, unitSettings.currentUnitMaterial);
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
        
        
        private void OnSliderUpdate(SliderEventData sliderData)
        {
            float assemblyPosition = sliderData.NewValue * mNoOfSteps;

            //Debug.Log("#MainManager#-------------------------OnSliderUpdate: " + sliderData.NewValue + " = " + unitPosition);

            //ChangeCurrentUnitPosition((int)unitPosition);
        }
    }
}
