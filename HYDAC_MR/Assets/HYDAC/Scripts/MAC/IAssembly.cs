using System;
using UnityEngine;

namespace HYDAC.Scripts.MAC
{
    public interface IAssembly
    {
        event Action<AssemblyManager> OnFocused;

        void ToggleFocus(bool toggle, Material fadeMaterial = null);

        void ToggleExplode(float positionTimeChange);
        void ChangeAssemblyPosition(int assemblyPosition, float positionTimeChange, 
            Material previousAssemblyMaterial, Material currentAssemblyMaterial, Material nextAssemblyMaterial);

        
        
        void Reset();
    }
}