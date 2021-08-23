using System;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.SOCS
{
    [CreateAssetMenu(menuName = "Socks/Assembly/Events", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        public bool IsInitialised;

        public SCatalogueInfo[] Catalogue { get; private set; }
        
        
        // Current selected machine from catalogue
        public SCatalogueInfo CurrentCatalogue { get; private set; }


        // Current selected module
        public SModuleInfo CurrentFocusedModule { get; private set; }


        // Current state of selected module        
        public bool IsDisassembled { get; }



        internal void SetCatalogue(SCatalogueInfo[] infos)
        {
            Catalogue = infos;
        }
        
        
        
        // On Assembly selected event and method
        public event Action<SCatalogueInfo> EAssemblySelected;
        internal void OnAssemblySelected(SCatalogueInfo info)
        {
            CurrentCatalogue = info;
            EAssemblySelected?.Invoke(info);
        }
        
        
        // On module 
        public event Action<SModuleInfo> EModuleSelected;

        internal void OnModuleSelected(SModuleInfo info)
        {
            Debug.Log("#SocAssemblyEvents#------------OnChangeModule:" + info.iname);

            CurrentFocusedModule = info;

            if (EModuleSelected != null) EModuleSelected.Invoke(info);
        }

        
        private void Awake()
        {
            Debug.Log("#SocAssemblyEvents#------------Start");

            IsInitialised = true;
            
            CurrentCatalogue = null;
            CurrentFocusedModule = null;
        }
    }
}