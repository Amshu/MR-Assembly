using System;
using UnityEngine;

namespace HYDAC_EView.Scripts.MPart
{
    /// <summary>
    /// <c>SocMachinePartInfo</c> is a scriptable object class that contains all the main details
    /// of a given machine part such as:
    /// <c>partName</c><value>This is the name of the part in, partName.</value>
    /// </summary>
    public class SocMachinePartInfo : ScriptableObject, IMachinePart
    {
        public string partName = "";
        public int assemblyPosition = 0;

        [TextArea]
        public string partInfo = "";

        // Events for MachinePart Class
        internal event Action<int, string> OnInitialize;
        internal event Action<float> OnImplode;
        internal event Action<float> OnExplode;
        internal event Action<bool, Material> OnHighlightPart;

        #region IMachinePart implementation

        void IMachinePart.Initialize()
        {
            OnInitialize?.Invoke(assemblyPosition, partName);
        }

        int IMachinePart.GetAssemblyPosition()
        {
            return assemblyPosition;
        }

        string IMachinePart.GetPartName()
        {
            return partName;
        }

        void IMachinePart.Implode(float timeToDest)
        {
            OnImplode?.Invoke(timeToDest);
        }

        void IMachinePart.Explode(float timeToDest)
        {
            OnExplode?.Invoke((timeToDest));
        }

        void IMachinePart.HighlightPart(bool toggle, Material highlightMaterial)
        {
            OnHighlightPart?.Invoke(toggle, highlightMaterial);
        }

        #endregion
        
        public void PrintInfo()
        {
            Debug.LogFormat("#SOC_MachinePartInfo#-------------------------{0}{1}\nPartInfo: {2}", 
                assemblyPosition, partName , partInfo);
        }
    }
}