using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.MOD;
using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;

namespace HYDAC.Scripts.MAIN
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private SocNetEvents netEvents;

        [SerializeField] private AssetReference modelPrefabRefToLoad;
        [SerializeField] private Transform machineWorldTransform;
        [SerializeField] private Transform focusedModuleHolderTransform;

        private bool _isNetworkScene;
        private bool _isInitialised;
        
        private GameObject _loadedModel;
        
        private AssetReference _currentModuleReference;
        private Transform _currentModuleTransform;

        private void Awake()
        {
            Addressables.InitializeAsync();
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            assemblyEvents.ECurrentModuleChange += OnCurrentModuleChange;
        }


        private void OnAddressablesInitialised(AsyncOperationHandle<IResourceLocator> obj)
        {
            Debug.Log("#AddressableManager#-------------Initialised");

            _isInitialised = true;
            
            if (_isNetworkScene)
            { 
                LoadModel();  
            }
        }


        public void OnCurrentModuleChange(SModuleInfo moduleInfo)
        {
            //if (!_isInitialised) return;

            Debug.Log("#AddressableManager#-------------Module Changed");
            
            if (_currentModuleTransform != null)
            {
                _currentModuleReference.ReleaseInstance(_currentModuleTransform.gameObject);
            }
            
            _currentModuleReference = moduleInfo.HighPolyReference;
            
            _currentModuleReference.InstantiateAsync(focusedModuleHolderTransform).Completed += (handle) =>
            {
                Debug.Log("#AddressableManager#-------------Module Instantiated");
                
                _currentModuleTransform = handle.Result.transform;
                _currentModuleTransform.position = focusedModuleHolderTransform.position;
                _currentModuleTransform.rotation = focusedModuleHolderTransform.rotation;
            };
        }

        
        private void LoadModel()
        {
            modelPrefabRefToLoad.InstantiateAsync().Completed += (loadedAsset) =>
            {
                Debug.Log("#AddressableManager#-------------Model Instantiated");
                
                _loadedModel = loadedAsset.Result;
                _loadedModel.transform.position = machineWorldTransform.position;
                _loadedModel.transform.rotation = machineWorldTransform.rotation;

                assemblyEvents.OnModelLoaded(_loadedModel.GetComponent<BaseAssembly>().Info as SAssemblyInfo);
            };
        }

        
        private void OnDestroy()
        {
            Addressables.InitializeAsync().Completed -= OnAddressablesInitialised;
            
            assemblyEvents.ECurrentModuleChange -= OnCurrentModuleChange;
            
            if(_loadedModel)
                modelPrefabRefToLoad.ReleaseInstance(_loadedModel);
        }
    }
}
