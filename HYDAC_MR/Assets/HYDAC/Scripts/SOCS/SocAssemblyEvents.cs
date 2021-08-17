using System;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.SOCS
{
    [CreateAssetMenu(menuName = "Socks/Assembly/Events", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        public bool IsInitialised = false;
        
        
        private SCatalogueInfo _currentCatalogue = null;
        public SCatalogueInfo CurrentCatalogue => _currentCatalogue;
        
        
        private SModuleInfo _currentFocusedModule = null;
        public SModuleInfo CurrentFocusedModule => _currentFocusedModule;
        
        
        public event Action<SCatalogueInfo> EAssemblySelected;
        internal void OnAssemblySelected(SCatalogueInfo info)
        {
            _currentCatalogue = info;
            EAssemblySelected?.Invoke(info);
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
            
            _currentCatalogue = null;
            _currentFocusedModule = null;
        }
    }
}