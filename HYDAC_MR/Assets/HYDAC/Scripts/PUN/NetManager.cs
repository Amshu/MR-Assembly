using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using HYDAC.SOCS.NET;
using UnityEngine.SceneManagement;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for creating a room on Photon Network (if master client: first player to connect) or else joining the room
    /// that the master client has created, allowing players to join a networked session in game.
    /// </summary>
    public class NetManager : MonoBehaviourPunCallbacks
    {
        private const string PCTESTROOMNAME = "T_PCLauncher";
        private const string DEFAULTROOMNAME = "HYDAC_DRoomA";
        
        [SerializeField] private SocNetSettings netSettings = null;
        [SerializeField] private SocNetUI netUI = null;
        
        #region Private and Public Attributes

        // Private Attributes
        private RoomOptions _roomOptions;

        private bool _isConnecting;
        private string _networkRoomName;
        
        #endregion

        
        #region Unity Methods
        private void Awake()
        {
            // Critical
            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client 
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            
            // Set default room options
            _roomOptions = new RoomOptions();
            _roomOptions.MaxPlayers = netSettings.MaxPlayersPerRoom;
            _roomOptions.IsVisible = netSettings.IsRoomVisible;
            _roomOptions.IsOpen = netSettings.IsRoomOpen;

#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name.Equals(PCTESTROOMNAME))
            {
                OnUIRequestedJoinRoom(DEFAULTROOMNAME);
                return;
            }
#endif
            
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
                // Connect to the Photon Network (server) 
                PhotonNetwork.GameVersion = netSettings.GameVersion;
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
            
            PhotonNetwork.JoinOrCreateRoom(_networkRoomName, _roomOptions, TypedLobby.Default);
        }
        
        
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
            
            // Critical
            // We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` 
            // to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("Loading VR Room");

                // Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel(netSettings.NetworkSceneName);
            }
        }
        
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("#NETManager#-------------OnDisconnected(): {0}", cause.ToString());
        }
        
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#NETManager#-------------OnJoinRandomFailed(): {0} - {1}", returnCode, message);

            // Critical
            // We failed to join a random room (room may not exist or room may be already full)
            // So, we create a new room.
            // PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom });
        }
        
        #endregion
    }
}