using UnityEngine;

using HYDAC.Scripts.INFO;
using System;

namespace HYDAC.Scripts.MOD
{
    public interface IAssembly
    {
        void OnToggleExplode(bool toggle);

        void OnToggleExplodeSubModule(int id);

        AssemblyModule[] GetAssemblyModules();
    }

    public class BaseAssembly : AUnit, IAssembly
    {
        // If you have multiple custom events, it is recommended to define them in the used class
        public const byte OnModuleChangeEventCode = 1;
        
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private AssemblyModule[] modules;
        [SerializeField] private GameObject modelPrefab;

        public event Action<SModuleInfo> OnModuleSelect;

        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first

        private void Start()
        {
            Instantiate(modelPrefab, transform).transform.localPosition = new Vector3(0f, 0.815f, 0f);
        }

        public void OnToggleExplode(bool toggle)
        {
            throw new NotImplementedException();
        }

        public void OnToggleExplodeSubModule(int id)
        {
            throw new NotImplementedException();
        }

        public AssemblyModule[] GetAssemblyModules()
        {
            return modules;
        }
    }
}
