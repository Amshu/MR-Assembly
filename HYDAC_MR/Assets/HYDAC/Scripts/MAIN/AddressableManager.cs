using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace HYDAC.Scripts.MAIN
{
    public class AddressableManager : MonoBehaviour
    {
        public const byte OnMachineSelectedEventCode = 2;
        
        [SerializeField] private SocMainSettings settings;
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private SocNetEvents netEvents;

        [Space]
        [SerializeField] private string[] lablesToload;
        [SerializeField] private Transform machineWorldTransform;
        [SerializeField] private Transform focusedModuleHolderTransform;

        private bool _isInitialised;
        
        private AssetReference _currentModuleReference;
        private Transform _currentModuleTransform;
        
        private bool _clearPreviousScene;
        private SceneInstance _currentScene;

        private List<AsyncOperationHandle<IList<IResourceLocation>>> _mainAssetHandles 
            = new List<AsyncOperationHandle<IList<IResourceLocation>>>();

        private void Awake()
        {
            // Initialise Addressables
            Addressables.InitializeAsync();
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            assemblyEvents.EAssemblySelected += OnAssemblySelected;
            assemblyEvents.EModuleSelected += OnModuleSelected;
            
            netEvents.EJoinRoom += OnRoomJoined;
            PhotonNetwork.NetworkingClient.EventReceived += OnPUNEvent;
        }
        

        /// <summary>
        /// ON ADDRESSABLES INITIALISED
        /// ---------------------------
        ///     -> Set initialised to true
        ///     -> Load catalogue
        /// </summary>
        /// <param name="obj"></param>
        private void OnAddressablesInitialised(AsyncOperationHandle<IResourceLocator> obj)
        {
            Debug.Log("#AddressableManager#-------------Initialised");

            _isInitialised = true;

            // Load Assets
            LoadAssets(lablesToload);

            // Load menu scene once Addressables is initialised
            StartCoroutine(RequestCatalogueFromRemote());
        }


        /// <summary>
        /// Fetches catalogue from remote location and saves the list in assembly events
        /// </summary>
        IEnumerator RequestCatalogueFromRemote()
        {
            List<SCatalogueInfo> fetchedCatalogueList = new List<SCatalogueInfo>();

            AsyncOperationHandle<IList<SCatalogueInfo>> catalogueHandle =
                Addressables.LoadAssetsAsync<SCatalogueInfo>(settings.CatalogueAssetGroupLabel, info =>
                {
                    Debug.Log("AddressableManager#------------Loaded catalogue info of: " + info.iname);

                    // Add to list
                    fetchedCatalogueList.Add(info);
                });

            yield return new WaitUntil(() => catalogueHandle.Task.IsCompleted);

            assemblyEvents.SetCatalogue(fetchedCatalogueList.ToArray());

            //Use this only when the objects are no longer needed
            //Addressables.Release(intersectionWithMultipleKeys);
        }

        private void LoadAssets(string[] lablesToload)
        {
            foreach(string label in lablesToload)
            {
                StartCoroutine(DownloadNLoadAssets(label));
            }
        }

        IEnumerator DownloadNLoadAssets(string assetFolderKey)
        {
            AsyncOperationHandle<long> downloadSize = Addressables.GetDownloadSizeAsync(assetFolderKey);
            
            yield return new WaitUntil(() => downloadSize.Task.IsCompleted);

            if (downloadSize.Result != 0)
            
            {
                Debug.Log("#AddressableManager#-------------Downloading label " + assetFolderKey +" .Download size: " + downloadSize.Result);

                AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(assetFolderKey, true);

                while (!downloadHandle.IsDone)
                {
                    Debug.Log("#AddressableManager#-------------Asset label: " + assetFolderKey + " download progress: " + downloadHandle.PercentComplete);

                    yield return new WaitForSeconds(1f);
                }

                yield return new WaitUntil(() => downloadHandle.Task.IsCompleted);

                LoadDownloadedAssets();
            }

            // Load Assets and store handles for release later
            void LoadDownloadedAssets()
            {
                Debug.Log("#AddressableManager#-------------Asset label: " + assetFolderKey + " already downloaded. Loading assets.");
                //AsyncOperationHandle<IList<IResourceLocation>> loadHandle =
                Addressables.LoadResourceLocationsAsync(assetFolderKey).Completed += (loadHandle) =>
                {
                    _mainAssetHandles.Add(loadHandle);
                };
            }
        }


        //private void OnDownloadComplete(AsyncOperationHandle obj)
        //{
        //    Debug.Log("#AddressableManager#-------------Loading asset");
        //    AsyncOperationHandle handle = Addressables.InstantiateAsync(assemblyEvents.CurrentCatalogue.AssemblyPrefab, machineWorldTransform);

        //    handle.Completed += operationHandle =>
        //    {
        //        Debug.Log("#AddressableManager#-------------Assembly intantiated");
        //    };
        //}



        
        
        //private async Task<TObject> FetchCatalogue()
        //{
        //    AsyncOperationHandle<IList<SCatalogueInfo>> catalogueHandle = 
        //        Addressables.LoadAssetsAsync<SCatalogueInfo>(settings.CatalogueAssetGroupLabel, info =>
        //        {
        //            Debug.Log("AddressableManager#------------Loaded catalogue info of: " + info.iname);
                    
        //            // Add to list
        //            fetchedCatalogueList.Add(info);
                    
        //        });

        //    return catalogueHandle.Task;
        //} 
        
        
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
        
        
        private void OnRoomJoined(NetStructInfo roomInfo)
        {
            LoadLevel(settings.SceneList[0].AssetGUID, true);
        }
        
        
        private void OnPUNEvent(EventData photonEvent)
        {
            Debug.Log("#BaseAssembly#------------Network event received");
            byte eventCode = photonEvent.Code;
            if (eventCode == OnMachineSelectedEventCode)
            {
                Debug.Log("#BaseAssembly#------------OnMachineSelectedEventCode");
                
                int moduleID = (int)photonEvent.CustomData;
            }
        }

 
        
        
        private void OnAssemblySelected(SCatalogueInfo info)
        {
            int content = info.ID;

            if (PhotonNetwork.IsMasterClient)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache}; 
                PhotonNetwork.RaiseEvent(OnMachineSelectedEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            }
        }

        
        
        

        public void OnModuleSelected(SModuleInfo moduleInfo)
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
        

        
        private void OnDestroy()
        {
            Addressables.InitializeAsync().Completed -= OnAddressablesInitialised;
            
            assemblyEvents.EModuleSelected -= OnModuleSelected;
            
            //if(_loadedModel)
                //modelPrefabRefToLoad.ReleaseInstance(_loadedModel);
        }
    }
}
