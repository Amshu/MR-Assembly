using TMPro;
using UnityEngine;

using HYDAC.Scripts.ADD;
using HYDAC.Scripts.MOD;
using HYDAC.Scripts.INFO;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using System;

namespace HYDAC.Scripts
{
    public class UIModuleWindow : MonoBehaviour
    {
        [SerializeField] private SocAssemblyUI socUI;
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [Space]
        [SerializeField] private Transform moduleRoot;
        [Space]
        [SerializeField] private Transform subModuleUIRoot;
        [SerializeField] private GameObject subModuleUIBtnPrefab;
        private Transform[] _subModuleUIs;


        [Space] [Header("Info UI")] 
        //[SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image image;

        SModuleInfo _currentModule;
        Transform _currentModuleTransform;

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
            StartCoroutine(LoadModuleAsset(newModule));
        }

        IEnumerator LoadModuleAsset(SModuleInfo newModule)
        {
            // Clear cahce if present
            if (_currentModule != null)
            {
                AddressableLoader.ReleaseObject(_currentModuleTransform.gameObject);

                foreach (var uiButton in _subModuleUIs)
                    Destroy(uiButton.gameObject);

                _subModuleUIs = null;
            }

            // Load asset
            var assetLoading = AddressableLoader.InstantiateFromReference(newModule.HighPolyReference, moduleRoot);
            yield return new WaitUntil(() => assetLoading.IsCompleted);

            // Set references
            _currentModuleTransform = assetLoading.Result.transform;
            _currentModuleTransform.localPosition = new Vector3(0, 0, 0);
            _currentModule = _currentModuleTransform.GetComponent<FocusedModule>().Info as SModuleInfo;

            // Initiate UI
            //--------------------
            
            // Change Title text
            nameText.text = newModule.iname;

            //descriptionText.text = newModule.description;

            //// Reset image UI
            //image.sprite = null;
            //if (newModule.ImageReference != null)
            //{
            //    LoadImage(newModule.ImageReference);
            //}

            // Only if ModuleInfo is viewable
            if(newModule.IsViewable)
            {
                List<Transform> uiButtons = new List<Transform>();

                // Update Submodule List
                for (int i = 0; i < _currentModule.SubModules.Length; i++)
                {
                    // Spawn UI

                    var uiButton = Instantiate(subModuleUIBtnPrefab, subModuleUIRoot);

                    // Get UI script reference and initialise it
                    uiButtons.Add(uiButton.transform);
                    uiButton.GetComponent<UIBtnSubModule>().Intitialise(_currentModule.SubModules[i]);
                }

                _subModuleUIs = uiButtons.ToArray();
            }
        }


        private async void LoadImage(AssetReference assetRef)
        {
            //image.sprite = await assetRef.LoadAssetAsync<Sprite>().Task;
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
