using System;
using UnityEngine;

namespace HYDAC.SOCS
{
    [CreateAssetMenu(menuName = "AssemblySocs/ModuleUI", fileName = "SOC_ModuleUI")]
    public class SocModuleUI : ScriptableObject
    {
        public event Action EUIRequestModuleToggle;
        public event Action EUIRequestToggleInfoUI;
        public event Action EUIRequestFocusOff;
        
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
