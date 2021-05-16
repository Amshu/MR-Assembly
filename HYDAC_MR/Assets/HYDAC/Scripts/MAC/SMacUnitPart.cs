using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MAC
{
    /// <summary>
    /// <c>SocMachinePartInfo</c> is a scriptable object class that contains all the main details
    /// of a given machine part such as:
    /// <c>partName</c><value>This is the name of the part in, partName.</value>
    /// </summary>
    public class SMacUnitPart : ScriptableObject, IMacUnitPart
    {
        public string partName = "";
        [FormerlySerializedAs("assemblyPosition")] public int unitPosition = 0;

        [TextArea]
        public string partInfo = "";

        // Events for MachinePart Class
        internal event Action<int, string> OnInitialize;
        internal event Action<float> OnImplode;
        internal event Action<float> OnExplode;
        internal event Action<bool, Material> OnHighlightPart;

        #region IMachinePart implementation

        void IMacUnitPart.Initialize()
        {
            OnInitialize?.Invoke(unitPosition, partName);
        }

        int IMacUnitPart.GetUnitPosition()
        {
            return unitPosition;
        }

        string IMacUnitPart.GetPartName()
        {
            return partName;
        }

        void IMacUnitPart.ToggleExplode(bool toggle, float timeToDest)
        {
            if(toggle)
                OnExplode?.Invoke(timeToDest);
            else
                OnImplode?.Invoke(timeToDest);
        }
        

        void IMacUnitPart.ChangeMaterial(bool toggle, Material highlightMaterial)
        {
            OnHighlightPart?.Invoke(toggle, highlightMaterial);
        }

        #endregion
        
        public void PrintInfo()
        {
            Debug.LogFormat("#SOC_MachinePartInfo#-------------------------{0}{1}\nPartInfo: {2}", 
                unitPosition, partName , partInfo);
        }
    }
}