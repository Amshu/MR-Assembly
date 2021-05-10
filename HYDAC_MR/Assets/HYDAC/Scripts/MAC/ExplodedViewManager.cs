using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

namespace HYDAC.Scripts.MAC
{
    public abstract class ExplodedViewManager : MonoBehaviour
    {
        private const string MachinePartInfoFolderPath = "MachinePartInfos";
        
        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        
        [Header("Debug")]
        public bool isExploded;
        public int currentAssemblyNo;
        public int startingPosition;
        
        public int NoOfSteps => mNoOfSteps;

        private IMachinePart[] _machineParts;
        
        protected virtual void Awake()
        {
            GetMachineParts();

            isExploded = false;
            currentAssemblyNo = startingPosition;
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

        public void ToggleAll()
        {
            foreach(IMachinePart part in _machineParts)
            {
                // If the current state is exploded then: 
                // Implode
                if (isExploded)
                {
                    part.Implode(mainSettings.positionTimeChange);
                }
                // Explode
                else
                {
                    part.Explode(mainSettings.positionTimeChange);
                }
            }

            isExploded = !isExploded;
        }


        public void OnSliderUpdate(SliderEventData sliderData)
        {
            float assemblyPosition = sliderData.NewValue * mNoOfSteps;

            //Debug.Log("#MainManager#-------------------------OnSliderUpdate: " + sliderData.NewValue + " = " + assemblyPosition);

            ChangeCurrentAssemblyPosition((int)assemblyPosition);
        }

        public void StepAssembly(int step)
        {
            int x = Mathf.Clamp(currentAssemblyNo + step, 0, mNoOfSteps);
            ChangeCurrentAssemblyPosition(x);
        }


        private void ChangeCurrentAssemblyPosition(int assemblyPosition)
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
                    part.Explode(mainSettings.positionTimeChange);

                    // Highlight previous part
                    if (partPosition == assemblyPosition - 1)
                    {
                        part.HighlightPart(true, mainSettings.previousAssemblyMaterial);
                    }
                    // Highlight current part
                    else if(partPosition == assemblyPosition)
                    {
                        part.HighlightPart(false, mainSettings.currentAssemblyMaterial);
                    }
                    else
                    {
                        part.HighlightPart(false, mainSettings.currentAssemblyMaterial);
                    }
                }
                // If its greater than the passed assembly position => Implode
                else
                {
                    part.Implode(mainSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == assemblyPosition + 1)
                    {
                        part.HighlightPart(true, mainSettings.nextAssemblyMaterial);
                    }
                    else
                    {
                        part.HighlightPart(false, mainSettings.currentAssemblyMaterial);
                    }
                }
            }
        }
    }
}
