using TMPro;
using UnityEngine;

using HYDAC.Scripts.ADD;
using HYDAC.Scripts.MOD;
using HYDAC.Scripts.INFO;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts
{
    public class UIModuleWindow : MonoBehaviour
    {
        [SerializeField] private SocAssemblyUI socUI;
        [SerializeField] private SocAssemblyEvents assemblyEvents;

        [Space] [Header("Info UI")] 
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image image;

        Transform _currentModule;

        Sprite _imageSprite;

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

            image.sprite = null;
            if (newModule.ImageReference != null)
            {
                LoadImage(newModule.ImageReference);
            }

            LoadNewModule(newModule);
        }


        private async void LoadImage(AssetReference assetRef)
        {
            image.sprite = await assetRef.LoadAssetAsync<Sprite>().Task;
        }


        private async void LoadNewModule(SModuleInfo newModule)
        {
            if (_currentModule != null)
                AddressableLoader.ReleaseObject(_currentModule.gameObject);

            var temp = await AddressableLoader.InstantiateFromReference(newModule.HighPolyReference, transform);

            _currentModule = temp.transform;
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
