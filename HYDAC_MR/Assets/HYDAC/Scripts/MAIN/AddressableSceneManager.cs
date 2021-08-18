using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;
using Photon.Pun;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HYDAC.Scripts.MAIN
{
    public class AddressableSceneManager: MonoBehaviour
    {
        [SerializeField] private SocMainSettings settings;
        [SerializeField] private SocNetEvents netEvents;

        private bool _isInitialised;
        
        private bool _clearPreviousScene;
        private SceneInstance _currentScene;
        
        private void Awake()
        {
            Addressables.InitializeAsync();
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            SceneManager.sceneLoaded += OnSceneLoad;

            netEvents.EJoinRoom += OnRoomJoined;
        }

        private void OnRoomJoined(string addressableKey)
        {
            LoadLevel(addressableKey, true);
        }


        private void OnAddressablesInitialised(AsyncOperationHandle<IResourceLocator> obj)
        {
            obj.Completed += handle =>
            {
                Debug.Log("#AddressableSceneManager#----------------Initialised");

                
                _isInitialised = true;
            
                // Load menu scene once Addressables is initialised
                LoadLevel(settings.MenuSceneRef.AssetGUID, false);
            };
        }
        
        
        private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        
        private void LoadLevel(string addressableAssetKey, bool isNetRoom)
        {
            if (!_isInitialised) return;

            if (_clearPreviousScene)
            {
                UnloadLevel();
            }

            Addressables.LoadSceneAsync(addressableAssetKey, LoadSceneMode.Additive).Completed += (asyncHandle) =>
            {
                Debug.Log("#MenuLevelLoader#----------------Loaded scene");
                _clearPreviousScene = true;
                _currentScene = asyncHandle.Result;
                
                if(isNetRoom)
                    netEvents.SetupNetRoom();
            };
        }

        
        private void UnloadLevel()
        {
            Addressables.UnloadSceneAsync(_currentScene).Completed += (asyncHandle) =>
            {
                Debug.Log("#AddressableSceneManager#----------------Unloaded scene");
                _clearPreviousScene = false;
                _currentScene = new SceneInstance();
            };
        }
    }
}