using System;
using HYDAC.Scripts.MOD;
using HYDAC.Scripts.MOD.SInfo;
using HYDAC.SOCS;
using TMPro;
using UnityEngine;

namespace HYDAC.Scripts
{
    public class ModuleHolderUI : MonoBehaviour
    {
        [SerializeField] private SocModuleUI socUI = null;
        [SerializeField] private SocAssemblyEvents assemblyEvents = null;

        [Space] [Header("Info UI")] 
        [SerializeField] private TextMeshPro idText = null;
        [SerializeField] private TextMeshPro nameText = null;
        [SerializeField] private TextMeshPro descriptionText = null;


        private GameObject currentModule = null;
    
        private void Awake()
        {
        
        }

        private void OnEnable()
        {
            assemblyEvents.ECurrentModuleChange += OnCurrentModuleChanged;
        
            socUI.EUIRequestFocusOff += OnUIRequestFocusOff;
            socUI.EUIRequestToggleInfoUI += OnUIRequestToggleInfoUI;
        }

        private void OnDisable()
        {
            assemblyEvents.ECurrentModuleChange -= OnCurrentModuleChanged;
        
            socUI.EUIRequestFocusOff -= OnUIRequestFocusOff;
            socUI.EUIRequestToggleInfoUI -= OnUIRequestToggleInfoUI;
        }


        private void OnCurrentModuleChanged(SModuleInfo newModule)
        {
            idText.text = newModule.ID.ToString();
            nameText.text = newModule.iname;
            descriptionText.text = newModule.description;
        }

        private void OnUIRequestToggleInfoUI()
        {
            throw new NotImplementedException();
        }

        private void OnUIRequestFocusOff()
        {
            throw new NotImplementedException();
        }
    
        private void Reset()
        {
            // Delete Module
            // Reset UI
        }
    }
}
