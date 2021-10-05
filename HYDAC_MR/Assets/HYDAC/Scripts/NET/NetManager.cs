using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using HYDAC.Scripts.ADD;
using System.Threading.Tasks;

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
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();  // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
                return;
            }

            UserStructInfo newUser = new UserStructInfo();
            newUser.UserID = newPlayer.ActorNumber;
            newUser.UserName = "Player";
            newUser.UserColor = Color.red; 
            newUser.IsMod = (PhotonNetwork.MasterClient.Equals(newPlayer));

            netEvents.NetInfo.AddUser(newUser);

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

            // If the local player changed properties
            if (targetPlayer.Equals(PhotonNetwork.LocalPlayer))
            {

            }
            else
            {
                UserStructInfo newInfo = new UserStructInfo();
                newInfo.UserID = targetPlayer.ActorNumber;

                if (changedProps.ContainsKey("Mod"))
                {
                    newInfo.IsMod = (bool)changedProps["Mod"];
                }
                if (changedProps.ContainsKey("Name"))
                {
                    newInfo.UserName = (string)changedProps["Name"];
                }
                else if (changedProps.ContainsKey("ColorR"))
                {
                    Color newColor = Color.white;
                    newColor.r = (float)changedProps["ColorR"];
                    newColor.g = (float)changedProps["ColorG"];
                    newColor.b = (float)changedProps["ColorB"];

                    newInfo.UserColor = newColor;
                }

                netEvents.NetInfo.UpdateUserProperties(
                        targetPlayer.ActorNumber, newInfo);
            }
        }

        #endregion
    }
}