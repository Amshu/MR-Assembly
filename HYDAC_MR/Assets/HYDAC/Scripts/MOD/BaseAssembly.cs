using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        [SerializeField] private AssemblyModule[] modules;
        
        private IAssemblyModule _currentModule;
        
        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first
        
        private void Awake()
        {
            // Subscribe to assembly modules on click events
            foreach (var module in modules)
            {
                IAssemblyModule assemblyModule = module as IAssemblyModule;
                assemblyModule.OnModuleFocused += OnAssemblyModuleManipulationStart;
            }
        }
        
        private void OnAssemblyModuleManipulationStart(AssemblyModule focusedModule)
        {
            if (_currentModule != null) return;
            
            Debug.Log("#BaseAssembly#-------------OnClicked received: " + focusedModule.Info.iname);
            
            //realtimeView.ClearOwnership();
            //realtimeView.RequestOwnership();

            //model.currentAssemblyName = focusedModule.transform.name;
        }
        
        
    }
}
