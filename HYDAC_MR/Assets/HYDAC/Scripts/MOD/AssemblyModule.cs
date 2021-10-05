using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
    public class AssemblyModule : AUnit
    {
        private SModuleInfo MInfo => info as SModuleInfo;

        internal event Action<SModuleInfo> EOnClicked;
        
        private Interactable _interactable = null;
        private bool isInFocus = false;

        private void Awake()
        {
            if(MInfo.isViewable)
                _interactable = GetComponent<Interactable>();
        }

        private void Start()
        {
            //Addressables.InstantiateAsync(MInfo.LowPolyReference, transform);
        }

        private void OnEnable()
        {
            if(MInfo.isViewable)
                _interactable.OnClick.AddListener(OnInteractableClicked);
        }
        
        private void OnDisable()
        {
            if(MInfo.isViewable)
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
