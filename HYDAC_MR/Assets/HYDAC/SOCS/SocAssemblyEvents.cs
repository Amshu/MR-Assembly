using System;
using HYDAC.Scripts.MOD;
using UnityEngine;

namespace HYDAC.SOCS
{
    [CreateAssetMenu(menuName = "AssemblySocs/AssemblyEvents", fileName = "SOC_AssemblyEvents")]
    public class SocAssemblyEvents: ScriptableObject
    {
        public event Action<SAssemblyInfo> EAssemblyLoad;
        public event Action<SModuleInfo> ECurrentModuleChange;
        
        internal void OnModelLoaded(SAssemblyInfo info)
        {
            EAssemblyLoad?.Invoke(info);
        }
        
        internal void OnCurrentModuleChange(SModuleInfo info)
        {
            ECurrentModuleChange?.Invoke(info);
        }
    }
}