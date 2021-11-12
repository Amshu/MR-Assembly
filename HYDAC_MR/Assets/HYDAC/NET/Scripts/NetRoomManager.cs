using UnityEngine;

using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviourPunCallbacks
    {
        private const string CATALOGUE = "CatalogueID";
        
        [SerializeField] Transform[] spawnpoints_Players;

        private NetManager _netManager;

        private bool _isMasterClient;
        private Hashtable _localPlayerHash = new Hashtable();

        internal void SetupRoom(NetManager manager)
        {
            _isMasterClient = PhotonNetwork.IsMasterClient;
            _netManager = manager;

            // If master client
            if (_isMasterClient)
            {
                SetupMasterClient();
            }

            // Setup Player
            SetupLocalUser();
        }


        /// <summary>
        /// IF MASTER CLIENT
        /// ----------------
        /// </summary>
        private void SetupMasterClient()
        {
            foreach (var netObject in _netManager.Settings.IntialNetObjects)
            {
                Debug.Log("#NetRoomManager#---------Instantiating network object: " + netObject.name + " at " + netObject.spawnPosition);

                PhotonNetwork.Instantiate(netObject.name, netObject.spawnPosition, netObject.spawnRotation);
            }
        }


        /// <summary>
        /// SETUP LOCAL USER
        /// ----------------
        /// </summary>
        private void  SetupLocalUser()
        {
            // First create the head
            Transform headTransform = Camera.main.transform;
            var gameObject = PhotonNetwork.Instantiate(_netManager.Settings.PlayerNetHeadPrefab.name, headTransform.position, headTransform.rotation);
            gameObject.transform.parent = headTransform;
            // Then the hands
            PhotonNetwork.Instantiate(_netManager.Settings.PlayerNetHandsPrefab.name, Vector3.zero, Quaternion.identity);

            headTransform.parent.position = spawnpoints_Players[_netManager.NetEvents.NetInfo.userCount].position;
            //headTransform.parent.rotation = spawnpoints_Players[netEvents.NetInfo.userCount].rotation;

            // Setup properties
            _localPlayerHash.Add("Mod", true);
            _localPlayerHash.Add("Name", "Player " + _netManager.NetEvents.NetInfo.userCount);

            if (_isMasterClient)
            {
                _localPlayerHash.Add("ColorR", Color.red.r);
                _localPlayerHash.Add("ColorG", Color.red.g);
                _localPlayerHash.Add("ColorB", Color.red.b);
            }
            else
            {
                _localPlayerHash.Add("ColorR", Color.blue.r);
                _localPlayerHash.Add("ColorG", Color.blue.g);
                _localPlayerHash.Add("ColorB", Color.blue.b);
            }
            
            PhotonNetwork.SetPlayerCustomProperties(_localPlayerHash);
        }
    }
}