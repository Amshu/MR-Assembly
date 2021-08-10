using System;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.SOCS
{
    [CreateAssetMenu(menuName = "Socks/Assembly/Events", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        public bool IsInitialised = false;
        
        
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

        internal void OnCurrentModuleChange(SModuleInfo info)
        {
            Debug.Log("#SocAssemblyEvents#------------OnChangeModule:" + info.iname);

            _currentFocusedModule = info;

            ECurrentModuleChange.Invoke(info);

            // if (ECurrentModuleChange != null)
            // {
            //     Debug.Log("#SocAssemblyEvents#------------Invoking");
            //
            // }
            // else
            //     Debug.Log("#SocAssemblyEvents#------------Null listeners");
        }

        
        private void Awake()
        {
            Debug.Log("#SocAssemblyEvents#------------Start");

            IsInitialised = true;
            
            _currentAssembly = null;
            _currentFocusedModule = null;
        }
    }
}