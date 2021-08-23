using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for creating a room on Photon Network (if master client: first player to connect) or else joining the room
    /// that the master client has created, allowing players to join a networked session in game.
    /// </summary>
    public class NetManager : MonoBehaviourPunCallbacks
    {
        // Singleton instances
        private static NetManager _instance;

        public bool connectOnStart;
        
        [SerializeField] private SocMainSettings settings;
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocNetUI netUI;
        
        private RoomOptions _roomOptions;

        private bool _isConnecting;
        private string _networkRoomName;
        
        
        #region Unity Methods
        private void Awake()
        {
            // Create a singleton
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } 
            else 
            {
                _instance = this;
            }
            
            // Critical
            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client 
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            
            // Set default room options
            _roomOptions = new RoomOptions();
            _roomOptions.MaxPlayers = settings.MaxPlayersPerRoom;
            _roomOptions.IsVisible = settings.IsRoomVisible;
            _roomOptions.IsOpen = settings.IsRoomOpen;
            
            if (connectOnStart)
            {
                OnUIRequestedJoinRoom(settings.DefaultNetRoomName);
                return;
            }

            // Subscribe to UI events
            netUI.EUIRequestJoinRoom += OnUIRequestedJoinRoom;
        }

        #endregion

        
        
        #region Connection Methods

        /// <summary>
        /// ON PLAYER UI REQUEST CONNECT
        /// ----------------------------
        /// -> Set isConnection flag to true
        /// -> Set the name of the room the user is trying to connect into
        /// 
        /// -> Check if connected to Photon Network
        ///     -> Load player prefab and add it to the network pool
        ///     -> If not connected to Photon Network using settings
        ///     -> Room is joined when OnConnectedToMaster()
        /// </summary>
        private void OnUIRequestedJoinRoom(string roomName)
        {
            // Set connecting flag - we are wanting to connect
            _isConnecting = true;
            _networkRoomName = roomName;
            
            if(!PhotonNetwork.IsConnected)
            {
                // Add to network pool
                DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
                if (pool != null)
                {
                    Addressables.LoadAssetAsync<GameObject>(settings.LocalPlayerPrefab).Completed += handle =>
                    {
                        pool.ResourceCache.Add(handle.Result.name, handle.Result);
                        
                        // Connect to the Photon Network (server) 
                        PhotonNetwork.GameVersion = settings.GameVersion;
                        PhotonNetwork.ConnectUsingSettings();
                    };
                }
            }
            else
            {
                JoinRoom(roomName);
            }
        }
        
        #endregion

        
        
        #region Photon Callbacks
        
        /// <summary>
        /// ON CONNECT TO PHOTON NETWORK
        /// ----------------------------
        /// ->  Check if we are waiting to connect
        ///     -> Connect to room if so
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("#NETManager#-------------OnConnectedToMaster()");

            // Check if we are wanting to connect (prevent looping when we disconnect from a room)
            if (_isConnecting)
            {
                JoinRoom(_networkRoomName);
            }
        }
        
        
        /// <summary>
        /// ON JOIN ROOM REQUEST
        /// --------------------
        /// -> Connect to given room if connected to Photon network
        /// </summary>
        private void JoinRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnected) return;
            
            PhotonNetwork.JoinOrCreateRoom(_networkRoomName, _roomOptions, TypedLobby.Default);
        }
        

        /// <summary>
        /// ON ROOM JOINED SUCCESSFULLY
        /// ---------------------------
        /// -> Set isConnecting to false and clear connecting room name
        /// -> Check if the user is host
        ///     -> If so then load level
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.LogFormat("#NETManager#-------------OnJoinedRoom(): {0}, {1}", _networkRoomName, PhotonNetwork.ServerAddress);

            _isConnecting = false;
            _networkRoomName = "";
            
            netEvents.InvokeJoinedRoom(settings.SceneList[1].AssetGUID);
            
            // // Critical
            // // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` 
            // // to sync our instance scene.
            // if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            // {
            //     netEvents.InvokeJoinedRoom(settings.NetworkSceneRef.AssetGUID);
            // }
        }
        
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("#NETManager#-------------OnDisconnected(): {0}", cause.ToString());
            netEvents.InvokeNetworkDisconnect();
        }
        
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#NETManager#-------------OnJoinRandomFailed(): {0} - {1}", returnCode, message);

            // Critical
            // We failed to join a random room (room may not exist or room may be already full)
            // So, we create a new room.
            // PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom });
            
            netEvents.InvokeJoinRoomFailed();
        }
        
        #endregion


    }
}