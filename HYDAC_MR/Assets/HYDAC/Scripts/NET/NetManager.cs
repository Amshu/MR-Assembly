using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using HYDAC.Scripts.SOCS;
using HYDAC.Scripts.SOCS.NET;
using UnityEngine.AddressableAssets;
using System;

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

        [SerializeField] private SocNetSettings settings;
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocNetUI netUI;
        [SerializeField] private Transform[] playerSpawnPoints;

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

            // Subscribe to UI events
            netUI.EUIRequestJoinRoom += OnUIRequestedJoinRoom;

            

            if (connectOnStart)
            {
                netEvents.TestNetworkAutoJoin += OnTestAutoJoin;
                return;
            }
        }

        private void OnTestAutoJoin()
        {
            OnUIRequestedJoinRoom(settings.DefaultNetRoomName);
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
                Debug.Log("#NETManager#-------------Adding player prefab to Photon pool");

                // Add to network pool
                DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
                if (pool != null)
                {
                    Addressables.LoadAssetAsync<GameObject>(settings.LocalPlayerPrefab).Completed += handle =>
                    {
                        pool.ResourceCache.Add(handle.Result.name, handle.Result);

                        Debug.Log("#NETManager#-------------Connecting to server");

                        // Connect to the Photon Network (server) 
                        PhotonNetwork.GameVersion = settings.GameVersion;
                        PhotonNetwork.ConnectUsingSettings();
                    };
                }
            }
            else
            {
                OnRequestJoinRoom(roomName);
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

            Transform playerSpawn = playerSpawnPoints[PhotonNetwork.CurrentRoom.PlayerCount];

            PhotonNetwork.Instantiate("NetworkedPlayer_PUN", playerSpawn.position, playerSpawn.rotation, 0);

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