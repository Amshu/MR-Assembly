using UnityEngine;

using HYDAC.Scripts.MOD;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private Module[] units;
        [SerializeField] private GameObject buttons;
        private IModule _currentModule;
        private IModule[] _iModules;

        private bool _inFocus;

        private void Awake()
        {
            GetAllAssemblies();
            
            buttons.SetActive(false);
        }

        private void GetAllAssemblies()
        {
            _iModules = new IModule[units.Length];
            for(int i = 0; i < _iModules.Length; i++)
            {
                _iModules[i] = units[i] as IModule;
                _iModules[i].OnFocused += OnModuleFocused;
            }
        }

        private void OnModuleFocused(Module targetModule)
        {
            if (_inFocus) return;
            
            // When Focused
            // - Set inFocus to true
            // - Set current MacUnit
            // - Enable Explode UI
            // - ToggleFocus callback on all MacUnits
            _inFocus = true;
            _currentModule = targetModule;
            
            buttons.SetActive(true);
            
            Debug.Log("#MainManager#--------------Unit Focused");
            
            for (int i = 0; i < _iModules.Length; i++)
            {
                IModule module = _iModules[i];

                if (!_currentModule.Equals(module))
                    module.ToggleFocus(false);
            }
        }

        /// <summary>
        /// Event call from UI to reset 
        /// </summary>
        public void ExitFocus()
        {
            if (!_inFocus) return;

            Debug.Log("#MainManager#--------------Exit Focus");

            for (int i = 0; i < _iModules.Length; i++)
            {
                IModule module = _iModules[i];

                module.Reset();
            }
            
            _inFocus = false;
            _currentModule = null;
        }

        
        #region Assembly Interface Calls

        public void ToggleExplode()
        {
            _currentModule?.ToggleExplode();
        }

        public void ChangePositionStep(int step)
        {
            _currentModule?.ChangePosition(step);
        }

        #endregion
    }
}
