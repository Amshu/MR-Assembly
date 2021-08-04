using HYDAC.Scripts.MOD.SInfo;
using HYDAC.SOCS;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents = null;
        
        private SModuleInfo _currentModule = null;
        
        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first
        
        private void Awake()
        {
            _currentModule = null;
        }

        private void OnEnable()
        {
            var assemblyModules = transform.GetComponentsInChildren<AssemblyModule>();
            foreach (var module in assemblyModules)
            {
                module.EOnClicked += OnAssemblyModuleClicked;
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
            _currentModule = module;
            
            assemblyEvents.OnCurrentModuleChange(module);
        }
    }
}
