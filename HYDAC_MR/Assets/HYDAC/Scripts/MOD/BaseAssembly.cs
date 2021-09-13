using System.Collections.Generic;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        // If you have multiple custom events, it is recommended to define them in the used class
        public const byte OnModuleChangeEventCode = 1;
        
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private AssemblyModule[] modules;

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
            Debug.Log("#BaseAssembly#--------------OnAssemblyModuleClicked: " + module.iname);
            
            int content = module.ID;

            assemblyEvents.OnModuleSelected(module);
        }
    }
}
