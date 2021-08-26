using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;
using System;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace HYDAC.Scripts.ADD
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField] private SocAddressablesSettings settings;
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocAssemblyEvents assemblyEvents;

        private bool _isInitialised;

        private IList<IResourceLocation> OnStartLoadedAssetsLocations = new List<IResourceLocation>();
        private SceneInstance _currentScene = default;
        private SceneInstance _currentEnvironment = default;

        private AddressablesNetLoader _netLoader;

        private void Awake()
        {
            // Get References
            _netLoader = GetComponent<AddressablesNetLoader>();

            // Initialise Addressables
            Addressables.InitializeAsync();
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            netEvents.EJoinedRoom += OnRoomJoined;
        }


        private void OnDestroy()
        {
            Addressables.InitializeAsync().Completed -= OnAddressablesInitialised;

            //assemblyEvents.EModuleSelected -= OnModuleSelected;

            //if(_loadedModel)
            //modelPrefabRefToLoad.ReleaseInstance(_loadedModel);
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

            SetupOnStart();
        }
        

        private async Task SetupOnStart()
        {
            // First load all the assets
            await AddressableLocationLoader.LoadLabels(settings.AssetsToLoadOnStart, OnStartLoadedAssetsLocations);

            // Then load local scene
            _currentScene =  await AddressablesSceneLoader.LoadScene(settings.SceneList[0], true);

            // Then load environment
            _currentEnvironment = await AddressablesSceneLoader.LoadScene(settings.DefaultEnvironment, true);

            netEvents.AutoJoinCheck();
        }


        private void OnRoomJoined(NetStructInfo obj)
        {
            // Load network scene
            LoadNetworkScene();
        }

        private async Task LoadNetworkScene()
        {
            await AddressablesSceneLoader.UnloadScene(_currentScene);

            _currentScene = await AddressablesSceneLoader.LoadScene(settings.SceneList[1], true);
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


        //private void OnPUNEvent(EventData photonEvent)
        //{
        //    Debug.Log("#BaseAssembly#------------Network event received");
        //    byte eventCode = photonEvent.Code;
        //    if (eventCode == OnMachineSelectedEventCode)
        //    {
        //        Debug.Log("#BaseAssembly#------------OnMachineSelectedEventCode");

        //        int moduleID = (int)photonEvent.CustomData;
        //    }
        //}



        //private void OnAssemblySelected(SCatalogueInfo info)
        //{
        //    int content = info.ID;

        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache}; 
        //        PhotonNetwork.RaiseEvent(OnMachineSelectedEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        //    }
        //}



        //public void OnModuleSelected(SModuleInfo moduleInfo)
        //{
        //    //if (!_isInitialised) return;

        //    Debug.Log("#AddressableManager#-------------Module Changed");

        //    if (_currentModuleTransform != null)
        //    {
        //        _currentModuleReference.ReleaseInstance(_currentModuleTransform.gameObject);
        //    }

        //    _currentModuleReference = moduleInfo.HighPolyReference;

        //    _currentModuleReference.InstantiateAsync(focusedModuleHolderTransform).Completed += (handle) =>
        //    {
        //        Debug.Log("#AddressableManager#-------------Module Instantiated");

        //        _currentModuleTransform = handle.Result.transform;
        //        _currentModuleTransform.position = focusedModuleHolderTransform.position;
        //        _currentModuleTransform.rotation = focusedModuleHolderTransform.rotation;
        //    };
        //}
    }
}
