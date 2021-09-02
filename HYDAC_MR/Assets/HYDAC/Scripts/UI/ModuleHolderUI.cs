using TMPro;
using UnityEngine;

using HYDAC.Scripts.MOD;
using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts
{
    public class ModuleHolderUI : MonoBehaviour
    {
        [SerializeField] private SocAssemblyUI socUI;
        [SerializeField] private SocAssemblyEvents assemblyEvents;

        [Space] [Header("Info UI")] 
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private void OnEnable()
        {
            assemblyEvents.EModuleSelected += OnModuleChanged;
        
            //socUI.EUIRequestFocusOff += OnUIRequestFocusOff;
            //socUI.EUIRequestToggleInfoUI += OnUIRequestToggleInfoUI;
        }

        private void OnDisable()
        {
            assemblyEvents.EModuleSelected -= OnModuleChanged;
        
            //socUI.EUIRequestFocusOff -= OnUIRequestFocusOff;
            //socUI.EUIRequestToggleInfoUI -= OnUIRequestToggleInfoUI;
        }


        private void OnModuleChanged(SModuleInfo newModule)
        {
            idText.text = newModule.ID.ToString();
            nameText.text = newModule.iname;
            descriptionText.text = newModule.description;
        }

        //private void OnUIRequestToggleInfoUI()
        //{
        //    throw new NotImplementedException();
        //}

        //private void OnUIRequestFocusOff()
        //{
        //    throw new NotImplementedException();
        //}
    
        private void Reset()
        {
            // Delete Module
            // Reset UI
        }
    }
}
