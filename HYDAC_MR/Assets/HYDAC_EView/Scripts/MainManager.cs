using System.Linq;
using UnityEngine;
using HYDAC_EView.Scripts.MPart;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;

namespace HYDAC_EView.Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private SocMainSettings mainSettings;
        [SerializeField] private int mNoOfAssemblies = 0;

        private const string MachinePartInfoFolderPath = "MachinePartInfos";

        public bool IsExploded;
        public int CurrentAssemblyNo;
        public int startingPosition;
        
        public int NoOfAssemblies => mNoOfAssemblies;

        private IMachinePart[] MachineParts;
        
        private void Awake()
        {
            GetMachineParts();

            IsExploded = false;
            CurrentAssemblyNo = startingPosition;
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
            MachineParts = parts.ToArray();
            if (MachineParts.Length < 1)
            {
                Debug.LogError("Error in casting to IMachinePart[]. Exiting Application");
                Application.Quit();
                return;
            }

            // Sort all parts by their assembly position
            MachineParts = MachineParts.OrderBy(x => x.GetAssemblyPosition()).ToArray();

            // Get total number of assemblies
            mNoOfAssemblies = MachineParts[MachineParts.Length - 1].GetAssemblyPosition();
        }

        public void ToggleAll()
        {
            foreach(IMachinePart part in MachineParts)
            {
                // If the current state is exploded then: 
                // Implode
                if (IsExploded)
                {
                    part.Implode(mainSettings.positionTimeChange);
                }
                // Explode
                else
                {
                    part.Explode(mainSettings.positionTimeChange);
                }
            }

            IsExploded = !IsExploded;
        }


        public void OnSliderUpdate(SliderEventData sliderData)
        {
            ChangeCurrentAssemblyPosition((int)(sliderData.NewValue * 10));
        }


        private void ChangeCurrentAssemblyPosition(int assemblyPosition)
        {
            Debug.Log("#MainManager#-------------------------Changing assembly position to: " + assemblyPosition);

            CurrentAssemblyNo = assemblyPosition;

            for (var i = 0; i < MachineParts.Length; i++)
            {
                var part = MachineParts[i];
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
