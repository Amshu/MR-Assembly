using System.Collections.Generic;
using HYDAC.Scripts.MOD.SInfo;
using HYDAC.SOCS;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents = null;

        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first

        private void OnEnable()
        {
            List<SModuleInfo> modules = new List<SModuleInfo>();
            
            var assemblyModules = transform.GetComponentsInChildren<AssemblyModule>();
            
            foreach (var module in assemblyModules)
            {
                module.EOnClicked += OnAssemblyModuleClicked;
                modules.Add(module.Info as SModuleInfo);
            }
            
            ((SAssemblyInfo) info).SetModules(modules.ToArray());
        }

        private void OnDisable()
        {
            var assemblyModules = transform.GetComponentsInChildren<AssemblyModule>();
            foreach (var module in assemblyModules)
            {
                module.EOnClicked -= OnAssemblyModuleClicked;
            }
        }

        private void OnAssemblyModuleClicked(SModuleInfo module)
        {
            assemblyEvents.OnRequestChangeModule(module);
        }
    }
}
