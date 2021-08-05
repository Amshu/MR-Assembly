using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using HYDAC.SOCS.NET;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private SocNetSettings netSettings;
        [SerializeField] private SocNetEvents netEvents;
        
        [Space]
        [SerializeField] private Transform playerSpawnPoint;

        #region Public and Private Methods

        private void Awake()
        {
            netEvents.ENetRoomSetup += OnSetupNetRoom;
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

            // Spawn a character for the local player
            // This gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(netSettings.LocalPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
        }
        
        
        
        /// <summary>
        /// Loads the VR room Scene after a connection is made to the Photon Server (via the Launcher)
        /// </summary>
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("#NetRoomManager#---------------Trying to " +
                               "load a level but we are not the master client");
            }

            Debug.LogFormat("#NetRoomManager#---------------Loading Level {0}", PhotonNetwork.CurrentRoom);
            PhotonNetwork.LoadLevel(netSettings.NetworkSceneName);
        }
    }
}