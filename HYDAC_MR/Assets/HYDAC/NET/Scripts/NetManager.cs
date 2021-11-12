using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using PHOTON = ExitGames.Client.Photon;

using HYDAC.Scripts.ADD;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for creating a room on Photon Network (if master client: first player to connect) or else joining the room
    /// that the master client has created, allowing players to join a networked session in game.
    /// </summary>
    
    [RequireComponent(typeof(NetRoomManager))]
    public class NetManager : MonoBehaviourPunCallbacks
    {
        private const string PLAYERPROPS_NAME = "Name";
        private const string PLAYERPROPS_MOD = "MOD";
        private const string PLAYERPROPS_COLORR = "ColorR";
        private const string PLAYERPROPS_COLORG = "ColorG";
        private const string PLAYERPROPS_COLORB = "ColorB";

        public SocNetSettings Settings;
        public SocNetEvents NetEvents;

        private bool _isConnecting;
        private string _networkRoomName;

        private RoomOptions _roomOptions;

        private IList<IResourceLocation> _netRoomObjects = new List<IResourceLocation>();

        private NetRoomManager roomManager;

        #region Unity Methods
        private async void Awake()
        {
            roomManager = GetComponent<NetRoomManager>();

            await AddressableLoader.LoadFromLabel(Settings.NetAssetsLabel , _netRoomObjects);

            // Critical
            // This makes sure we can use PhotonNetwork.LoadLevel() on the master client 
            // and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            
            // Set default room options
            _roomOptions = new RoomOptions();
            _roomOptions.BroadcastPropsChangeToAll = true;
            _roomOptions.MaxPlayers = Settings.MaxPlayersPerRoom;
            _roomOptions.IsVisible = Settings.IsRoomVisible;
            _roomOptions.IsOpen = Settings.IsRoomOpen;

            // Subscribe to UI events
            //netUI.EUIRequestJoinRoom += OnUIRequestedJoinRoom;

            SetupForConnection(Settings.DefaultNetRoomName);
        }

        #endregion


        #region Connection Methods

        private void OnUIRequestedJoinRoom(string roomName)
        {
            SetupForConnection(roomName);
        }


        /// <summary>
        /// SETUP FOR CONNECTION
        /// ----------------------------
        /// -> Set isConnection flag to true
        /// -> Set the name of the room the user is trying to connect into
        /// 
        /// -> Check if connected to Photon Network
        ///     -> Load player prefab and add it to the network pool
        ///     -> If not connected to Photon Network using settings
        ///     -> Room is joined when OnConnectedToMaster()
        /// </summary>
        private async void SetupForConnection(string roomName)
        {
            // Set connecting flag - we are wanting to connect
            _isConnecting = true;
            _networkRoomName = roomName;

            // If not connected yet
            if (!PhotonNetwork.IsConnected)
            {
                // Initialise net pool
                NetPunPool.Initialise();

                // Add main local player net prefabs to net pool
                await NetPunPool.AddObjectToPool(Settings.PlayerNetHeadPrefab);
                await NetPunPool.AddObjectToPool(Settings.PlayerNetHandsPrefab);

                // Adding the intial net objects to net pool
                Settings.IntialNetObjects = await NetPunPool.AddObjectsToPool(Settings.IntialNetObjects);

                //Connect to the Photon Network(server)
                PhotonNetwork.GameVersion = Settings.GameVersion;
                PhotonNetwork.ConnectUsingSettings();
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

            NetEvents.OnNetConnect();

            // Check if we are wanting to connect (prevent looping when we disconnect from a room)
            if (_isConnecting)
            {
                OnRequestJoinRoom(_networkRoomName);
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("#NETManager#-------------OnDisconnected(): {0}", cause.ToString());

            NetEvents.OnNetDisconnect();
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
            Room currentRoom = PhotonNetwork.CurrentRoom;

            Debug.LogFormat("#DEBUG##NETManager#-------------Joined Room: {0}, \n{1}", _networkRoomName, PhotonNetwork.ServerAddress);

            // Leave room if there are more than the max players
            if (currentRoom.PlayerCount > Settings.MaxPlayersPerRoom)
            {
                Debug.LogFormat("#DEBUG##NETManager#-------------Room Full. Leaving room");

                PhotonNetwork.LeaveRoom(); 

                return;
            }

            _isConnecting = false;
            _networkRoomName = "";

            roomManager.SetupRoom(this);

            NetEvents.OnNetJoinRoom(currentRoom.Name, currentRoom.PlayerCount);
        }

       


        public override void OnLeftRoom()
        {
            Debug.LogWarningFormat("#DEBUG##NETManager#-------------OnLeftRoom");

            NetEvents.OnNetLeftRoom();
        }
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#DEBUG##NETManager#-------------Unable to join Room. Error: {0} - {1}", returnCode, message);
            
            NetEvents.OnNetJoinRoomFailed();
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UserStructInfo newUser = new UserStructInfo();

            // Set the player custom properties in Master Client only
            if (PhotonNetwork.IsMasterClient)
            {
                newUser.UserID = newPlayer.ActorNumber;
                newUser.UserName = "Player";
                newUser.UserColor = Color.red;
                newUser.IsMod = (newUser.UserID == 0) ? true : false;

                // Create New Custom Properties
                PHOTON.Hashtable playerProps = new PHOTON.Hashtable();
                playerProps.Add(PLAYERPROPS_NAME, newUser.UserName);
                playerProps.Add(PLAYERPROPS_MOD, newUser.IsMod);
                playerProps.Add(PLAYERPROPS_COLORR, newUser.UserColor.r);
                playerProps.Add(PLAYERPROPS_COLORG, newUser.UserColor.g);
                playerProps.Add(PLAYERPROPS_COLORB, newUser.UserColor.b);

                // Set new properties
                newPlayer.SetCustomProperties(playerProps);
            }


            if (!newPlayer.IsLocal)
                NetEvents.OnNetPlayerJoinedRoom(PhotonNetwork.CountOfPlayers);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            NetEvents.NetInfo.RemoveUser(otherPlayer.ActorNumber);

            NetEvents.OnNetPlayerLeftRoom();
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            // If the local player's properties were changed
            if (targetPlayer.Equals(PhotonNetwork.LocalPlayer))
            {
                // Check what was changed


            }
            // If other player's properties were changed
            // ---- 
            else
            {
                int userID = targetPlayer.ActorNumber;

                UserStructInfo newInfo = new UserStructInfo();
                newInfo.UserID = userID;

                // If user info does not exist in userList
                if (NetEvents.NetInfo.UsersInRoom.Length - 1 == targetPlayer.ActorNumber)
                {
                    NetEvents.NetInfo.AddUser(newInfo);
                }

                newInfo = NetEvents.NetInfo.UsersInRoom[userID];

                // Check which properties changed and update
                if (changedProps.ContainsKey(PLAYERPROPS_MOD))
                {
                    newInfo.IsMod = (bool)changedProps[PLAYERPROPS_MOD];
                }
                if (changedProps.ContainsKey(PLAYERPROPS_NAME))
                {
                    newInfo.UserName = (string)changedProps[PLAYERPROPS_NAME];
                }
                if (changedProps.ContainsKey(PLAYERPROPS_COLORR) ||
                        changedProps.ContainsKey(PLAYERPROPS_COLORG) ||
                        changedProps.ContainsKey(PLAYERPROPS_COLORB))
                {
                    Color newColor = Color.white;
                    newColor.r = (float)changedProps[PLAYERPROPS_COLORR];
                    newColor.g = (float)changedProps[PLAYERPROPS_COLORG];
                    newColor.b = (float)changedProps[PLAYERPROPS_COLORB];

                    newInfo.UserColor = newColor;
                }

                // Update user in list
                NetEvents.NetInfo.UpdateUserProperties(newInfo.UserID, newInfo);
            }
        }

        #endregion
    }
}