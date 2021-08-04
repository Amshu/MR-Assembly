using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

using HYDAC.SOCS.NET;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity) and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private SocNetSettings netSettings;
        [SerializeField] private SocNetEvents netEvents;
        
        [Space]
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;

        [Space] 
        [SerializeField] private GameObject focusedModuleHolderPrefab;
        [SerializeField] private Transform focusedModuleHolderSpawnPoint;

        #region Public and Private Methods

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                InstantiateFocusedModuleHolder(focusedModuleHolderSpawnPoint);
            }
            
            // Create local player
            InstantiateLocalPlayer(playerSpawnPoint);
        }

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();  // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
                return;
            }
            
            if(newPlayer.IsLocal)
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
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
        }

        private void InstantiateFocusedModuleHolder(Transform spawnPoint)
        {
            Debug.LogFormat("#NetRoomManager#---------------Instantiating Focused Module Holder");
            
            GameObject temp = PhotonNetwork.Instantiate(focusedModuleHolderPrefab.name, spawnPoint.position, spawnPoint.rotation, 1);
            
            netEvents.OnFocusedModuleReady(temp.transform);
        }
        
        
        /// <summary>
        /// Loads the VR room Scene after a connection is made to the Photon Server (via the Launcher)
        /// </summary>
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("#NetRoomManager#---------------Trying to load a level but we are not the master client");
            }

            Debug.LogFormat("#NetRoomManager#---------------Loading Level {0}", PhotonNetwork.CurrentRoom);
            PhotonNetwork.LoadLevel(netSettings.NetworkSceneName);
        }
        
        #endregion
    }
}