using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
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
