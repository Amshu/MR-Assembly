using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

using HYDAC.SOCS.NET;

namespace HYDAC.Scripts
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField] private SocNetEvents netEvents = null;
        [SerializeField] private AssetReference modelPrefabRefToLoad;
        [SerializeField] private Transform machineWorldTransform;

        private bool _isNetworkScene;
        private GameObject _loadedModel = null;
        private void Awake()
        {
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            _isNetworkScene = SceneManager.GetActiveScene().buildIndex == 0;

            if (_isNetworkScene)
                netEvents.ELocalUserReady += OnLocalUserReady;
        }

        private void OnLocalUserReady()
        {
            LoadModel();
        }


        private void OnAddressablesInitialised(AsyncOperationHandle<IResourceLocator> obj)
        {
            Debug.Log("#AddressableManager#-------------Initialised");
        
            if (_isNetworkScene)
            { 
                LoadModel();  
            }
        }

        private void LoadModel()
        {
            modelPrefabRefToLoad.InstantiateAsync().Completed += (loadedAsset) =>
            {
                Debug.Log("#AddressableManager#-------------Model Instantiated");
                
                _loadedModel = loadedAsset.Result;
                _loadedModel.transform.position = machineWorldTransform.position;
                _loadedModel.transform.rotation = machineWorldTransform.rotation;
            };
        }

        private void OnDestroy()
        {
            modelPrefabRefToLoad.ReleaseInstance(_loadedModel);
        }
    }
}
