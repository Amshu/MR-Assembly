using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HYDAC.Scripts.MOD
{
    public interface ISubModule
    {
        void Initialize();
        int GetUnitPosition();
        string GetPartName();

        void Assemble(float timeToDest);
        void Disassemble(float timeToDest);
        
        void ChangeMaterial(bool toggle, Material highlightMaterial);
    }
    
    
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
        internal event Action<float> OnAssemble;
        internal event Action<float> OnDisassemble;
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

        public void Assemble(float timeToDest)
        {
            OnAssemble?.Invoke(timeToDest);
        }

        public void Disassemble(float timeToDest)
        {
            OnDisassemble?.Invoke(timeToDest);
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