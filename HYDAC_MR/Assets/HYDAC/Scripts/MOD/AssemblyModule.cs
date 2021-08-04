using System;
using HYDAC.Scripts.MOD.SInfo;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace HYDAC.Scripts.MOD
{
    public interface IBaseModule
    {
        void ToggleFocus(bool toggle);
        
        /// <summary>
        ///  This is for the part in focus 
        /// </summary>
        void Reset();

        string GetName();
    }
    
    public class AssemblyModule : AUnit
    {
        internal event Action<SModuleInfo> EOnClicked;
        
        private Interactable _interactable = null;
        private bool isInFocus = false;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }

        private void OnEnable()
        {
            _interactable.OnClick.AddListener(OnInteractableClicked);
        }
        
        private void OnDisable()
        {
            _interactable.OnClick.RemoveListener(OnInteractableClicked);
        }

        private void OnInteractableClicked()
        {
            if (isInFocus) return;

            Debug.Log("#AssemblyModule#-------------OnClicked: " + name);
            
            EOnClicked?.Invoke(Info as SModuleInfo);
        }
    }
}
