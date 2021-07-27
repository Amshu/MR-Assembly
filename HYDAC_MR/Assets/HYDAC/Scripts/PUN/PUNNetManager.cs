using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using HYDAC.SOCS.NET;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for creating a room on Photon Network (if master client: first player to connect) or else joining the room
    /// that the master client has created, allowing players to join a networked session in game.
    /// </summary>
    public class PUNNetManager : MonoBehaviourPunCallbacks
    {
        public const string SceneName = "AssemblyView - HyBox";
        
        [SerializeField] private SocNetUI netUI = null;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created. MAX 20 users")]
        [SerializeField] private byte maxPlayersPerRoom = 4;

        
        #region Private and Public Attributes

        // Private Attributes
        private string _gameVersion = "1";          // Set to 1 by default, unless we need to make breaking changes on a project that is Live.
        private bool _isConnecting;
        private string _connectingRoomName = "";
        private RoomOptions _roomOptions;

        #endregion

        
        #region Unity Methods
        private void Awake()
        {
            // Critical
            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client 
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;

            // Subscribe to UI events
            netUI.EUIRequestJoinRoom += OnUIRequestedJoinRoom;
            
            // Set default room options
            _roomOptions = new RoomOptions();
            _roomOptions.MaxPlayers = maxPlayersPerRoom;
            _roomOptions.IsVisible = true;
            _roomOptions.IsOpen = true;
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
        ///     -> If not connected to Photon Network using settings
        ///     -> Room is joined when OnConnectedToMaster()
        /// </summary>
        private void OnUIRequestedJoinRoom(string roomName)
        {
            // Set connecting flag - we are wanting to connect
            _isConnecting = true;
            _connectingRoomName = roomName;
            
            if(!PhotonNetwork.IsConnected)
            {
                // Connect to the Photon Network (server) 
                PhotonNetwork.GameVersion = _gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                JoinRoom(roomName);
            }
        }
        
        #endregion

        
        
        #region Photon Callbacks
        
        /// <summary>
        /// ON JOIN ROOM REQUEST
        /// --------------------
        /// -> Connect to given room if connected to Photon network
        /// </summary>
        private void JoinRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnected) return;
            
            PhotonNetwork.JoinOrCreateRoom(_connectingRoomName, _roomOptions, TypedLobby.Default);
        }
        
        
        /// <summary>
        /// ON CONNECT TO PHOTON NETWORK
        /// ----------------------------
        /// ->  Check if we are waiting to connect
        ///     -> Connect to room if so
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("#LauncherPUN#-------------OnConnectedToMaster()");

            // Check if we are wanting to connect (prevent looping when we disconnect from a room)
            if (_isConnecting)
            {
                JoinRoom(_connectingRoomName);
            }
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
            Debug.LogFormat("#LauncherPUN#-------------OnJoinedRoom(): {0}, {1}", _connectingRoomName, PhotonNetwork.ServerAddress);

            _isConnecting = false;
            _connectingRoomName = "";
            
            // Critical
            // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` 
            // to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("Loading VR Room");

                // Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel(SceneName);
            }
        }
        
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("#LauncherPUN#-------------OnDisconnected(): {0}", cause.ToString());
        }
        
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#LauncherPUN#-------------OnJoinRandomFailed(): {0} - {1}", returnCode, message);

            // Critical
            // We failed to join a random room (room may not exist or room may be already full)
            // So, we create a new room.
            // PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom });
        }
        
        #endregion
    }
}