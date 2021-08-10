using System;
using TMPro;
using UnityEngine;

using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts
{
    public class ModuleHolderUI : MonoBehaviour
    {
        [SerializeField] private SocModuleUI socUI = null;
        [SerializeField] private SocAssemblyEvents assemblyEvents = null;

        [Space] [Header("Info UI")] 
        [SerializeField] private TextMeshProUGUI idText = null;
        [SerializeField] private TextMeshProUGUI nameText = null;
        [SerializeField] private TextMeshProUGUI descriptionText = null;


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
