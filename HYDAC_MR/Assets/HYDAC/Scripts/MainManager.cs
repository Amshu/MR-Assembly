using UnityEngine;

using HYDAC.Scripts.MAC;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private SocMainSettings mainSettings;
        [SerializeField] private AssemblyManager[] assemblyManagers;

        private IAssembly _currentAssembly;
        private IAssembly[] _assemblies; 

        private void Awake()
        {
            GetAllAssemblies();
        }

        private void SetCurrentAssembly(AssemblyManager assembly)
        {
            _currentAssembly = assembly;
        }

        private void GetAllAssemblies()
        {
            _assemblies = new IAssembly[assemblyManagers.Length];
            for(int i = 0; i < _assemblies.Length; i++)
            {
                _assemblies[i] = assemblyManagers[i] as IAssembly;
                _assemblies[i].OnFocused += OnAssemblyFocusChanged;
            }
        }

        private void OnAssemblyFocusChanged(AssemblyManager assembly)
        {
            SetCurrentAssembly((assembly));
            
            for (int i = 0; i < _assemblies.Length; i++)
            {
                if(_currentAssembly.Equals(_assemblies[i]))
                    _assemblies[i].ToggleFocus(true);
                else
                    _assemblies[i].ToggleFocus(false, mainSettings.fadeAssemblyMaterial);
            }
        }

        private void ExitFocus()
        {
            for (int i = 0; i < _assemblies.Length; i++)
            {
                _assemblies[i].ToggleFocus(false);
            }
        }

        #region Assembly Interface Calls

        public void ToggleAssemblyExplode()
        {
            
        }

        #endregion
    }
}
