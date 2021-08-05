using System;
using HYDAC.Scripts.MOD;
using HYDAC.Scripts.MOD.SInfo;
using UnityEngine;

namespace HYDAC.SOCS
{
    [CreateAssetMenu(menuName = "AssemblySocs/AssemblyEvents", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        private SModuleInfo _currentFocusedModule = null;
        public SModuleInfo CurrentFocusedModule => _currentFocusedModule;
        
        public event Action<SAssemblyInfo> EAssemblyLoad;
        public event Action<SModuleInfo> ECurrentModuleChange;
        
        internal void OnModelLoaded(SAssemblyInfo info)
        {
            EAssemblyLoad?.Invoke(info);
        }
        
        internal void OnCurrentModuleChange(SModuleInfo info)
        {
            _currentFocusedModule = info;
            
            ECurrentModuleChange?.Invoke(info);
        }
    }
}