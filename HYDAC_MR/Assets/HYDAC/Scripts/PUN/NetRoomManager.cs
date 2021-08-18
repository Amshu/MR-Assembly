using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocMainSettings settings;

        [Space] 
        [SerializeField] private int customManualInstantiationEventCode;
        [SerializeField] private AssetReference playerPrefab;
        [SerializeField] private GameObject netPlayerPrefab;
        [SerializeField] private Transform playerSpawnPoint;

        #region Public and Private Methods

        private void Awake()
        {
            netEvents.ENetRoomSetup += OnSetupNetRoom;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        private void OnSetupNetRoom()
        {
            Debug.Log("#RoomManager#--------------Setting up room");

            // Create local player
            InstantiateLocalPlayer(playerSpawnPoint);
            
            netEvents.ENetRoomSetup -= OnSetupNetRoom;
        }


        #endregion

        
        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();  // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
                return;
            }
            
            if(!newPlayer.IsLocal)
                netEvents.OnPlayerJoined();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            netEvents.OnPlayerLeft();
        }

        /// <summary>
        /// Photon callback advises that local player has left room so reload launcher scene
        /// </summary>
        public override void OnLeftRoom()
        {
            // Load 'first' scene (Launcher.unity)
            SceneManager.LoadScene(0);
        }

        #endregion

        
        private void InstantiateLocalPlayer(Transform spawnPoint)
        {
            Debug.LogFormat("#NetRoomManager#---------------Instantiating LocalPlayer");
            
            PhotonNetwork.Instantiate(netPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
            
            // Addressables.InstantiateAsync(playerPrefab, spawnPoint).Completed += handle =>
            // {
            //     GameObject player = handle.Result;
            //     
            //     // Spawn a character for the local player
            //     // This gets synced by using PhotonNetwork.Instantiate
            //     //PhotonNetwork.Instantiate(settings.LocalPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
            //     PhotonView photonViewComponent = player.GetComponent<PhotonView>();
            //
            //     if (PhotonNetwork.AllocateViewID(photonViewComponent))
            //     {
            //         object[] data = new object[]
            //         {
            //             player.transform.position, player.transform.rotation, photonViewComponent.ViewID
            //         };
            //
            //         RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            //         {
            //             Receivers = ReceiverGroup.Others,
            //             CachingOption = EventCaching.AddToRoomCache
            //         };
            //
            //         SendOptions sendOptions = new SendOptions
            //         {
            //             Reliability = true
            //         };
            //
            //         PhotonNetwork.RaiseEvent((byte) customManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            //     }
            //     else
            //     {
            //         Debug.LogError("Failed to allocate a ViewId.");
            //
            //         Destroy(player);
            //     }
            // };
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == customManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;
                
                Addressables.InstantiateAsync(playerPrefab, (Vector3) data[0], (Quaternion) data[1]).Completed += handle =>
                {
                    GameObject player = handle.Result;
                    PhotonView photonViewComponent = player.GetComponent<PhotonView>();
                    photonViewComponent.ViewID = (int) data[2];
                };
            }
        }
    }
}