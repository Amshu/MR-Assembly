using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        private bool _inFocus;
        private IAssemblyModule _currentModule;
        
        
        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first
        
        private void Start()
        {
            // IAssemblyModule[] modules = info.AssemblyModules;
            //
            // // Subscribe to assembly modules
            // foreach (var module in modules)
            // {
            //     IAssemblyModule assemblyModule = module as IAssemblyModule;
            //     assemblyModule.OnModuleFocused += OnAssemblyModuleManipulationStart;
            // }
        }
        
        private void OnAssemblyModuleManipulationStart(AssemblyModule focusedModule)
        {
            if (_inFocus) return;
            
            Debug.Log("#MainManager#-------------OnManipulationStarted: " + name);
            
            //realtimeView.ClearOwnership();
            //realtimeView.RequestOwnership();

            //model.currentAssemblyName = focusedModule.transform.name;
        }
    }
}
