using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

using HYDAC.Scripts.NET;

namespace HYDAC.Scripts.ADD
{
    [RequireComponent(typeof(NetManager))]
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField] private SocAddressablesSettings settings;
        [SerializeField] private SocNetEvents netEvents;

        private IList<IResourceLocation> _onStartLoadedAssetsLocations = new List<IResourceLocation>();

        private SceneInstance _currentScene = default;
        private SceneInstance _currentEnvironment = default;

        private void Awake()
        {
            // Initialise Addressables
            Addressables.InitializeAsync();
            Addressables.InitializeAsync().Completed += OnAddressablesInitialised;

            netEvents.EJoinedRoom += OnRoomJoined;
        }

        private void OnDestroy()
        {
            Addressables.InitializeAsync().Completed -= OnAddressablesInitialised;

            netEvents.EJoinedRoom -= OnRoomJoined;
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

            SetupOnStart();
        }
        

        private async Task SetupOnStart()
        {
            // First load all the assets
            await AddressableLoader.LoadLabels(settings.LoadAssets_OnStart, _onStartLoadedAssetsLocations);

            Debug.Log("#AddressableManager#-------------Loaded OnStartLocations: " + _onStartLoadedAssetsLocations.Count);

            // Then load local scene
            _currentScene =  await AddressablesSceneLoader.LoadScene(settings.List_Scene[0], true);

            // Then load environment
            _currentEnvironment = await AddressablesSceneLoader.LoadScene(settings.Env_Default, true);

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

            _currentScene = await AddressablesSceneLoader.LoadScene(settings.List_Scene[1], true);

            //SceneManager.SetActiveScene(_currentScene.Scene);
        }


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
