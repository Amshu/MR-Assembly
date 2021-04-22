using System.Linq;
using UnityEngine;
using HYDAC_EView._Scripts.MPart;

namespace HYDAC_EView._Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private SocMainSettings mainSettings;
        [SerializeField] private int mNoOfAssemblies = 0;
        
        public struct SState
        {
            public bool IsExploded;
            public int CurrentAssemblyNo;
        }

        private static SState _currentState;
        public SState GetCurrentState => _currentState;


        public int startingPosition;
        
        public int NoOfAssemblies => mNoOfAssemblies;

        private IMachinePart[] MachineParts { get; set; } = null;
        
        private void Awake()
        {
            GetMachineParts();

            _currentState.IsExploded = false;
            _currentState.CurrentAssemblyNo = startingPosition;
        }
        
        private void GetMachineParts()
        {
            // Get all machine parts
            MachineParts = FindObjectsOfType<MachinePart>() as IMachinePart[];

            var parts = MachineParts.ToList();

            // Sort parts by assembly position
            MachineParts = parts.OrderBy(x => x.GetAssemblyPosition()).ToArray();

            // Get total number of assemblies
            mNoOfAssemblies = MachineParts[MachineParts.Length - 1].GetAssemblyPosition();
        }


        public void TriggerAction(bool doExplode)
        {
            _currentState.IsExploded = doExplode;

            for (var i = 0; i < MachineParts.Length; i++)
            {
                var part = MachineParts[i];
                var partPosition = part.GetAssemblyPosition();
                
                if (partPosition <= _currentState.CurrentAssemblyNo)
                {
                    part.Explode(mainSettings.positionTimeChange);
                }
                else
                {
                    part.Implode(mainSettings.positionTimeChange);
                }
            }
        }


        public void ChangeCurrentAssemblyPosition(int positionToChangeTo)
        {
            Debug.Log("#MainManager#-------------------------Changing assembly position to: " + positionToChangeTo);

            _currentState.CurrentAssemblyNo = positionToChangeTo;
            
            for (var i = 0; i < MachineParts.Length; i++)
            {
                var part = MachineParts[i];
                var partPosition = part.GetAssemblyPosition();

                if (partPosition <= positionToChangeTo)
                {
                    //part.Explode(mainSettings.positionTimeChange);

                    // Highlight previous part
                    if (partPosition == positionToChangeTo - 1)
                    {
                        part.HighlightPart(true, mainSettings.previousAssemblyColor);
                    }
                    // Highlight current part
                    else if(partPosition == positionToChangeTo)
                    {
                        part.HighlightPart(true, mainSettings.currentAssemblyColor);
                    }
                    else
                    {
                        part.HighlightPart(false, mainSettings.currentAssemblyColor);
                    }
                }
                else
                {
                    //part.Implode(mainSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == positionToChangeTo + 1)
                    {
                        part.HighlightPart(true, mainSettings.nextAssemblyColor);
                    }
                    else
                    {
                        part.HighlightPart(false, mainSettings.currentAssemblyColor);
                    }
                }
            }
        }
    }
}
