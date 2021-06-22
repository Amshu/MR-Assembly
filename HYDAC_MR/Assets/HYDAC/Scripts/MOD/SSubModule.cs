using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HYDAC.Scripts.MOD
{
    /// <summary>
    /// <c>SocMachinePartInfo</c> is a scriptable object class that contains all the main details
    /// of a given machine part such as:
    /// <c>partName</c><value>This is the name of the part in, partName.</value>
    /// </summary>
    public class SSubModule : ScriptableObject, ISubModule
    {
        public string partName = "";
        public int assemblyPosition = 0;

        [TextArea]
        public string partInfo = "";

        // Events for MachinePart Class
        internal event Action<int, string> OnInitialize;
        internal event Action<float> OnImplode;
        internal event Action<float> OnExplode;
        internal event Action<bool, Material> OnHighlight;

        #region IMachinePart implementation

        void ISubModule.Initialize()
        {
            OnInitialize?.Invoke(assemblyPosition, partName);
        }

        int ISubModule.GetUnitPosition()
        {
            return assemblyPosition;
        }

        string ISubModule.GetPartName()
        {
            return partName;
        }

        void ISubModule.ToggleExplode(bool toggle, float timeToDest)
        {
            if(toggle)
                OnExplode?.Invoke(timeToDest);
            else
                OnImplode?.Invoke(timeToDest);
        }
        

        void ISubModule.ChangeMaterial(bool toggle, Material highlightMaterial)
        {
            OnHighlight?.Invoke(toggle, highlightMaterial);
        }

        #endregion
        
        public void PrintInfo()
        {
            Debug.LogFormat("#SSubModule#-------------------------{0}{1}\nPartInfo: {2}", 
                assemblyPosition, partName , partInfo);
        }
    }
}