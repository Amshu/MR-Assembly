using System.Collections.Generic;
using UnityEngine;

using HYDAC.Scripts.MOD;
using UnityEngine.Serialization;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private BaseModule[] modules;
        [SerializeField] private GameObject buttons;
        
        private IAssemblyModule _currentAssemblyModule;
        private IAssemblyModule[] _assemblyModules;

        private bool _inFocus;

        private void Awake()
        {
            GetAllModules();
            
            buttons.SetActive(false);
        }

        /// <summary>
        ///  ON APPLICATION AWAKE
        /// ----------------------
        /// - Find and store all the Assembly Modules in the Modules array
        ///
        /// - Register to the assembly module's onFocus events
        /// </summary>
        private void GetAllModules()
        {
            List<IAssemblyModule> assemblyModules = new List<IAssemblyModule>();
            
            foreach (BaseModule module in modules)
            {
                if (module is IAssemblyModule)
                {
                    //Debug.Log("#MainManager#--------------Assembly Module Found");

                    IAssemblyModule assemblyModule = module as IAssemblyModule;
                    assemblyModules.Add(assemblyModule);

                    assemblyModule.OnModuleFocused += OnModuleFocused;
                }
            }

            _assemblyModules = assemblyModules.ToArray();

            Debug.Log("#MainManager#--------------Assembly Modules Found - " + _assemblyModules.Length);
        }

        /// <summary>
        ///  ON USER MODULE SELECTION
        /// --------------------------
        /// - Set the system mode to 'Focused'
        /// - Get and set the reference of the focused module
        /// 
        /// - Enable the UI for the 'Focused' mode
        /// 
        /// - Set all the other modules in 'UnfocusedMode'
        /// </summary>
        /// <param name="targetAssemblyModule"></param>
        private void OnModuleFocused(IAssemblyModule targetAssemblyModule)
        {
            // If the current mode is already in FOCUSED MODE then ignore
            if (_inFocus) return;
            
            _inFocus = true;
            _currentAssemblyModule = targetAssemblyModule;
            
            buttons.SetActive(true);
            
            //Debug.Log("#MainManager#--------------Unit Focused");
            
            for (int i = 0; i < modules.Length; i++)
            {
                IBaseModule module = modules[i];

                if (!_currentAssemblyModule.Equals(module))
                    module.ToggleFocus(false);
            }
        }


        /// <summary>
        ///  ON USER EXIT FROM FOCUS MODE
        /// -----------------------------
        /// - Remove reference of the focused module
        /// 
        /// - Set all the other modules back to default Mode
        /// - Set the system mode to 'UNFOCUSED'
        /// </summary>
        public void ExitFocus()
        {
            // If the current mode is "Unfocused' then ignore
            if (!_inFocus) return;

            //Debug.Log("#MainManager#--------------Exit Focus");

            for (int i = 0; i < modules.Length; i++)
            {
                IBaseModule module = modules[i];

                module.Reset();
            }
            
            _inFocus = false;
            _currentAssemblyModule = null;
        }

        
        #region UI EVENT METHODS-------------------------

        public void ToggleExplode()
        {
            _currentAssemblyModule?.ToggleExplode();
        }

        public void ChangePositionStep(int step)
        {
            _currentAssemblyModule?.ChangePosition(step);
        }

        #endregion
    }
}
