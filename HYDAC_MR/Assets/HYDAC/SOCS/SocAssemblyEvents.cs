using System;
using HYDAC.Scripts.MOD.SInfo;
using UnityEngine;

namespace HYDAC.SOCS
{
    [CreateAssetMenu(menuName = "AssemblySocs/AssemblyEvents", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        private SAssemblyInfo _currentAssembly = null;
        public SAssemblyInfo CurrentAssembly => _currentAssembly;
        
        
        private SModuleInfo _currentFocusedModule = null;
        public SModuleInfo CurrentFocusedModule => _currentFocusedModule;
        
        
        public event Action<SAssemblyInfo> EAssemblyLoad;
        internal void OnModelLoaded(SAssemblyInfo info)
        {
            _currentAssembly = info;
            EAssemblyLoad?.Invoke(info);
        }
        
        
        public event Action<SModuleInfo> ECurrentModuleChange;
        public event Action<SModuleInfo> ERequestChangeModule;

        internal void OnRequestChangeModule(SModuleInfo newModule)
        {
            ERequestChangeModule?.Invoke(newModule);
        }

        internal void OnCurrentModuleChange(SModuleInfo info)
        {
            _currentFocusedModule = info;
            
            ECurrentModuleChange?.Invoke(info);
        }
    }
}