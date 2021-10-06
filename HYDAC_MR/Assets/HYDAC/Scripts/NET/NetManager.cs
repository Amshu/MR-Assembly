using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using HYDAC.Scripts.ADD;
using PHOTON = ExitGames.Client.Photon;
using System.Collections;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for creating a room on Photon Network (if master client: first player to connect) or else joining the room
    /// that the master client has created, allowing players to join a networked session in game.
    /// </summary>
    public class NetManager : MonoBehaviourPunCallbacks
    {
        private const string PLAYERPROPS_NAME = "Name";
        private const string PLAYERPROPS_MOD = "MOD";
        private const string PLAYERPROPS_COLORR = "ColorR";
        private const string PLAYERPROPS_COLORG = "ColorG";
        private const string PLAYERPROPS_COLORB = "ColorB";

        // Singleton instances
        private static NetManager _instance;

        public bool connectOnStart;

        [SerializeField] private SocNetSettings settings; 
        [SerializeField] private SocNetEvents netEvents;
        [SerializeField] private SocNetUI netUI;

        private bool _isConnecting;
        private string _networkRoomName;

        private Room _currentRoom;
        private RoomOptions _roomOptions;

        private DefaultPool _punPool;
        private List<Player> _roomPlayers = new List<Player>();
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
            _roomOptions.BroadcastPropsChangeToAll = true;
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
                StartCoroutine(PreparePhotonPoolNConnect());
            }
        }

        IEnumerator PreparePhotonPoolNConnect()
        {
            var playerHead = AddressableLoader.LoadFromReference(settings.PlayerNetHeadPrefab.assetReference);
            yield return new WaitUntil(() => playerHead.IsCompleted);
            _punPool.ResourceCache.Add(playerHead.Result.name, playerHead.Result);

            var playerHands = AddressableLoader.LoadFromReference(settings.PlayerNetHandsPrefab.assetReference);
            yield return new WaitUntil(() => playerHands.IsCompleted);
            _punPool.ResourceCache.Add(playerHands.Result.name, playerHands.Result);

            List<PUNPoolObject> loadedPoolObjects = new List<PUNPoolObject>();

            // Then load and add all the other networked objects
            foreach (PUNPoolObject poolObject in settings.NetObjects)
            {
                var loadTask = AddressableLoader.LoadFromReference(poolObject.assetReference);
                yield return new WaitUntil(()=>loadTask.IsCompleted);

                var poolObjectPrefab = loadTask.Result;

                _punPool.ResourceCache.Add(poolObjectPrefab.name, poolObjectPrefab);

                poolObject.SetSpawnValues(poolObjectPrefab.name, poolObjectPrefab.transform.position, poolObjectPrefab.transform.rotation);

                _loadedNetworkedPrefab.Add(loadTask.Result);
                loadedPoolObjects.Add(poolObject);

                Debug.Log("#NETManager#-------------Loaded NetObject to PUN pool: " + poolObject.name + " " + poolObject.name + " " + poolObject.spawnPosition);
            }

            settings.SetNetObjects(loadedPoolObjects.ToArray());

            yield return new WaitForSeconds(2.0f);

            //Connect to the Photon Network(server)
            PhotonNetwork.GameVersion = settings.GameVersion;
            PhotonNetwork.ConnectUsingSettings();
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
            Room currentRoom = PhotonNetwork.CurrentRoom;

            Debug.LogFormat("#DEBUG##NETManager#-------------Joined Room: {0}, \n{1}", _networkRoomName, PhotonNetwork.ServerAddress);

            // Leave room if there are more than the max players
            if (currentRoom.PlayerCount > settings.MaxPlayersPerRoom)
            {
                Debug.LogFormat("#DEBUG##NETManager#-------------Room Full. Leaving room");

                PhotonNetwork.LeaveRoom(); 

                return;
            }

            _isConnecting = false;
            _networkRoomName = "";

            netEvents.OnNetJoinRoom(currentRoom.Name, currentRoom.PlayerCount);
        }

       


        public override void OnLeftRoom()
        {
            Debug.LogWarningFormat("#DEBUG##NETManager#-------------OnLeftRoom");

            netEvents.OnNetLeftRoom();
        }
        
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("#DEBUG##NETManager#-------------Unable to join Room. Error: {0} - {1}", returnCode, message);
            
            netEvents.OnNetJoinRoomFailed();
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
                netEvents.OnNetPlayerJoinedRoom(PhotonNetwork.CountOfPlayers);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                return;
            }

            netEvents.NetInfo.RemoveUser(otherPlayer.ActorNumber);

            netEvents.OnNetPlayerLeftRoom();
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
                if (netEvents.NetInfo.UsersInRoom.Length - 1 == targetPlayer.ActorNumber)
                {
                    netEvents.NetInfo.AddUser(newInfo);
                }

                newInfo = netEvents.NetInfo.UsersInRoom[userID];

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
                netEvents.NetInfo.UpdateUserProperties(newInfo.UserID, newInfo);
            }
        }

        #endregion
    }
}