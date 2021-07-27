using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity) and instantiating players into that scene.
    /// </summary>
    public class RoomMgrPUN : MonoBehaviourPunCallbacks
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public Transform spawnPoint;

        #region Photon Callbacks
        /// <summary>
        /// Photon callback advises that local player has left room so reload launcher scene
        /// </summary>
        public override void OnLeftRoom()
        {
            // Load 'first' scene (Launcher.unity)
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();        // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //LoadArena();        // NOTE: Enable if we want to "reload" scene each time a new player joins / leaves
            }
        }

        #endregion

        #region Public and Private Methods
        private void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Missing playerPrefab reference...please set it up in GameObject 'Room Manager", this);
            }
            else
            {
                if (PlayerMgrPUN.localPlayerInstance == null)
                {
                    Debug.LogFormat("Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                    // Spawn a character for the local player
                    // This gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        /// <summary>
        /// Loads the VR room Scene after a connection is made to the Photon Server (via the Launcher)
        /// </summary>
        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master client");
            }

            Debug.LogFormat("PhotonNetwork: Loading Level {0}", PhotonNetwork.CurrentRoom);
            PhotonNetwork.LoadLevel(PUNNetManager.SceneName);
        }
        #endregion
    }
}