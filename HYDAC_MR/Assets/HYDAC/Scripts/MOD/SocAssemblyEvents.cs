using System;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
    [CreateAssetMenu(menuName = "Socks/Assembly/Events", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents : ScriptableObject
    {
        public SModuleInfo[] Modules;
        public bool IsInitialised;

        public SCatalogueInfo[] Catalogue { get; private set; }

        // Current selected machine from catalogue
        public SCatalogueInfo CurrentCatalogue { get; private set; }
        // Current selected module
        public SModuleInfo CurrentModule { get; private set; }
        // Current selected subModule
        public SSubModuleInfo CurrentSubModule { get; private set; }


        private void Awake()
        {
            CurrentCatalogue = null;
            CurrentModule = null;

            IsInitialised = false;
        }

        private void OnEnable()
        {
            IsInitialised = false;
        }

        internal void SetCatalogue(SCatalogueInfo[] infos)
        {
            Catalogue = infos;
        }


        // On Assembly selected event and method
        public event Action<SCatalogueInfo> EAssemblySelected;
        internal void OnAssemblySelected(SCatalogueInfo info)
        {
            IsInitialised = true;

            CurrentCatalogue = info;
            EAssemblySelected?.Invoke(info);
        }


        // On module selected
        public event Action<SModuleInfo> EModuleSelected;
        internal void OnModuleSelected(SModuleInfo info)
        {
            Debug.Log("#SocAssemblyEvents#------------OnChangeModule:" + info.iname);

            CurrentModule = info;

            EModuleSelected?.Invoke(info);
        }


        // On module explode
        public event Action<bool> EModuleExplode;
        public void OnModuleExplode(bool toggle)
        {
            Debug.Log("#SocAssemblyEvents#------------OnModuleExplode: " + toggle);

            EModuleExplode?.Invoke(toggle);
        }


        // On Sub-Module selected
        public event Action<SSubModuleInfo> ESubModuleSelected;
        internal void OnSubModuleSelected(SSubModuleInfo info)
        {
            Debug.Log("#SocAssemblyEvents#------------OnSelectSubModule:" + info.iname);

            CurrentSubModule = info;

            ESubModuleSelected?.Invoke(info);
        }
    }
}