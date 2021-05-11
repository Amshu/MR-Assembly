using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

namespace HYDAC.Scripts.MAC
{
    public sealed class AssemblyManager : MonoBehaviour, IAssembly
    {
        private const string MachinePartInfoFolderPath = "MachinePartInfos";

        [Header("Assembly Members")]
        [SerializeField] private SocMainSettings mainSettings;

        [SerializeField] private ObjectManipulator objectManipulator = null;
        [SerializeField] private MoveAxisConstraint axisConstraint = null;
        [SerializeField] private BoundsControl boundsControl = null;

        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        
        [Header("Debug")]
        public bool isExploded;
        public int currentAssemblyNo;
        public int startingPosition;
        
        private bool _isInFocus = false;
        private Transform _defaultTransform;
        
        private IMachinePart[] _machineParts;
        
        public int NoOfSteps => mNoOfSteps;
        

        private void Awake()
        {
            GetMachineParts();

            isExploded = false;
            currentAssemblyNo = startingPosition;
            
            objectManipulator.OnManipulationStarted.AddListener(OnClicked);
        }

        private void GetMachineParts()
        {
            // Load from Resources
            var machinePartInfos = Resources.LoadAll(MachinePartInfoFolderPath, typeof(IMachinePart));
            if (machinePartInfos.Length < 1)
            {
                Debug.LogError("No Machine part infos found or loaded. Exiting Application");
                Application.Quit();
                return;
            }

            // Cast each loaded object to IMachinePart
            List<IMachinePart> parts = new List<IMachinePart>();
            foreach(object ogj in machinePartInfos)
            {
                parts.Add(ogj as IMachinePart);
            }

            // Set to main array
            _machineParts = parts.ToArray();
            if (_machineParts.Length < 1)
            {
                Debug.LogError("Error in casting to IMachinePart[]. Exiting Application");
                Application.Quit();
                return;
            }

            // Sort all parts by their assembly position
            _machineParts = _machineParts.OrderBy(x => x.GetAssemblyPosition()).ToArray();

            // Get total number of assemblies
            mNoOfSteps = _machineParts[_machineParts.Length - 1].GetAssemblyPosition();
        }
        
        
        private void OnClicked(ManipulationEventData arg0)
        {
            Debug.Log("Object Clicked: " + name);

            if (_isInFocus) return;
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IAssembly).ToggleFocus(true);
        }
        
        
        public event Action<AssemblyManager> OnFocused;
        private void RaiseOnFocused(AssemblyManager managerRef)
        {
            OnFocused?.Invoke(managerRef);
        }
        
        void IAssembly.ToggleFocus(bool toggle, Material fadeMaterial = null)
        {
            _isInFocus = toggle;
            axisConstraint.enabled = !toggle;
            boundsControl.enabled = toggle;

            if (!toggle)
            {
                // Disappear
                
            }
        }

        void IAssembly.ToggleExplode()
        {
             
        }

        void IAssembly.ChangeAssemblyPosition(int position)
        {
            throw new NotImplementedException();
        }

        void IAssembly.Reset()
        {
            StartCoroutine(LerpPosition(this.transform, _defaultTransform.position, 1));
        }
        
        private IEnumerator LerpPosition(Transform trans, Vector3 position, float timeTakenToDest)
        {
            var currentPos = trans.localPosition;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / timeTakenToDest;
                trans.localPosition = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }

            //_mLock = false;
        }
        
        
        
        
        
        
 

       

        public void ToggleAll(float positionTimeChange)
        {
            foreach(IMachinePart part in _machineParts)
            {
                // If the current state is exploded then: 
                // Implode
                if (isExploded)
                {
                    part.Implode(positionTimeChange);
                }
                // Explode
                else
                {
                    part.Explode(positionTimeChange);
                }
            }

            isExploded = !isExploded;
        }


        public void OnSliderUpdate(SliderEventData sliderData)
        {
            float assemblyPosition = sliderData.NewValue * mNoOfSteps;

            //Debug.Log("#MainManager#-------------------------OnSliderUpdate: " + sliderData.NewValue + " = " + assemblyPosition);

            //ChangeCurrentAssemblyPosition((int)assemblyPosition);
        }

        public void StepAssembly(int step)
        {
            int x = Mathf.Clamp(currentAssemblyNo + step, 0, mNoOfSteps);
            //ChangeCurrentAssemblyPosition(x);
        }


        private void ChangeCurrentAssemblyPosition(int assemblyPosition, float positionTimeChange,
        Material previousAssemblyMaterial, Material currentAssemblyMaterial, Material nextAssemblyMaterial)
        {
            Debug.Log("#MainManager#-------------------------Changing assembly position to: " + assemblyPosition);

            currentAssemblyNo = assemblyPosition;

            for (var i = 0; i < _machineParts.Length; i++)
            {
                var part = _machineParts[i];
                var partPosition = part.GetAssemblyPosition();

                // If its less than or equal to the passed assembly position => Explode
                if (partPosition <= assemblyPosition)
                {
                    part.Explode(positionTimeChange);

                    // Highlight previous part
                    if (partPosition == assemblyPosition - 1)
                    {
                        part.HighlightPart(true, previousAssemblyMaterial);
                    }
                    // Highlight current part
                    else if(partPosition == assemblyPosition)
                    {
                        part.HighlightPart(false, currentAssemblyMaterial);
                    }
                    else
                    {
                        part.HighlightPart(false, currentAssemblyMaterial);
                    }
                }
                // If its greater than the passed assembly position => Implode
                else
                {
                    part.Implode(positionTimeChange);

                    // Highlight next part
                    if(partPosition == assemblyPosition + 1)
                    {
                        part.HighlightPart(true, nextAssemblyMaterial);
                    }
                    else
                    {
                        part.HighlightPart(false, currentAssemblyMaterial);
                    }
                }
            }
        }
    }
}
