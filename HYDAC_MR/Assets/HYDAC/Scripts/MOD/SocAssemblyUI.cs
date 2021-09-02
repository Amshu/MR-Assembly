using HYDAC.Scripts.INFO;
using System;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    [CreateAssetMenu(menuName = "Socks/Assembly/UI", fileName = "SOC_AssemblyUI")]
    public class SocAssemblyUI : ScriptableObject
    {
        public event Action<SCatalogueInfo> EUIRequestAssemblySelect;
        public event Action EUIRequestModuleToggle;
        public event Action EUIRequestToggleInfoUI;
        public event Action EUIRequestFocusOff;


        public event Action<bool> EToggleCatalogueUI;
        public void InvokeToggleCatalogueUI(bool toggle)
        {
            EToggleCatalogueUI?.Invoke(toggle);
        }

        public void InvokeUIAssemblySelect(SCatalogueInfo info)
        {
            EUIRequestAssemblySelect?.Invoke(info);
        }

        public void OnUIRequestAssembleToggle()
        {
            EUIRequestModuleToggle?.Invoke();
        }
        
        public void OnUIRequestToggleInfoUI()
        {
            EUIRequestToggleInfoUI?.Invoke();
        }
        
        public void OnUIRequestFocusOff()
        {
            EUIRequestFocusOff?.Invoke();
        }
    }
}
