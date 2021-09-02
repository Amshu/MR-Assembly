using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using HYDAC.Scripts.ADD;

namespace HYDAC.Scripts.NET
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

        [SerializeField] private SocNetSettings settings; 
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocNetUI netUI;

        private bool _isConnecting;
        private string _networkRoomName;
        private RoomOptions _roomOptions;

        private DefaultPool _punPool;
        private GameObject _localPlayerPrefab;
        private IList<GameObject> _loadedNetworkedPrefab = new List<GameObject>();

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

            _punPool = PhotonNetwork.PrefabPool as DefaultPool;

            // Subscribe to UI events
            netUI.EUIRequestJoinRoom += OnUIRequestedJoinRoom;

            if (connectOnStart)
                netEvents.TestNetworkAutoJoin += OnTestAutoJoin;
        }

        #endregion

        private void OnTestAutoJoin()
        {
            OnUIRequestedJoinRoom(settings.DefaultNetRoomName);
        }


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

            // If not connected yet
            if(!PhotonNetwork.IsConnected)
            {
                PreparePhotonPoolNConnect();
            }
        }


        private async Task PreparePhotonPoolNConnect()
        {
            // First load in the local player prefab
            _localPlayerPrefab = await AddressableLoader.LoadFromReference(settings.Asset_LocalPlayer);

            // Add to pool
            _punPool.ResourceCache.Add(_localPlayerPrefab.name, _localPlayerPrefab);

            // Then load in all the other network objects
            _loadedNetworkedPrefab = await AddressableLoader.LoadAssetReferences(settings.Assets_PhotonPool);
            foreach (GameObject go in _loadedNetworkedPrefab)
            {
                _punPool.ResourceCache.Add(go.name, go);

                if(go.Equals(_loadedNetworkedPrefab[_loadedNetworkedPrefab.Count - 1]))
                {
                    //Connect to the Photon Network(server)
                    PhotonNetwork.GameVersion = settings.GameVersion;
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
        }


        /// <summary>
        /// ON JOIN ROOM REQUEST
        /// --------------------
        /// -> Connect to given room if connected to Photon network
        /// </summary>
        private void OnRequestJoinRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnected) return;

            PhotonNetwork.JoinOrCreateRoom(_networkRoomName, _roomOptions, TypedLobby.Default);
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

            netEvents.OnNetConnect();

            // Check if we are wanting to connect (prevent looping when we disconnect from a room)
            if (_isConnecting)
            {
                OnRequestJoinRoom(_networkRoomName);
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("#NETManager#-------------OnDisconnected(): {0}", cause.ToString());

            netEvents.OnNetDisconnect();
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

            // Leave room if there are more than the max players
            if(PhotonNetwork.CurrentRoom.PlayerCount >= settings.MaxPlayersPerRoom)
            {
                Debug.LogFormat("#NETManager#-------------Too many players in room. Leaving room");

                PhotonNetwork.LeaveRoom(); 

                return;
            }

            _isConnecting = false;
            _networkRoomName = "";

            netEvents.OnNetJoinRoom(PhotonNetwork.CurrentRoom);
        }


        public override void OnLeftRoom()
        {
            Debug.LogWarningFormat("#NETManager#-------------OnLeftRoom");

            netEvents.OnNetLeftRoom();
        }
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#NETManager#-------------OnJoinRandomFailed(): {0} - {1}", returnCode, message);
            
            netEvents.OnNetJoinRoomFailed();
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();  // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
                return;
            }

            if (!newPlayer.IsLocal)
                netEvents.OnNetPlayerJoinedRoom(PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            netEvents.OnNetPlayerLeftRoom();
        }

        #endregion
    }
}