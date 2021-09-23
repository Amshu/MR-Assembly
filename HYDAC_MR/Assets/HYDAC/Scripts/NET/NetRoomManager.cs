using UnityEngine;

using Photon.Pun;
using ExitGames.Client.Photon;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviourPunCallbacks
    {
        private const string CATALOGUE = "CatalogueID";

        [SerializeField] SocNetSettings settings;
        [SerializeField] SocNetEvents netEvents;
        [SerializeField] Transform[] spawnpoints_Players;

        private bool _isMasterClient;

        private Transform _localPlayer;
        private Hashtable _localPlayerHash = new Hashtable();


        private void Awake()
        {
            _isMasterClient = PhotonNetwork.IsMasterClient;
        }

        private void Start()
        {
            // If master clientf
            if (_isMasterClient)
                SetupMasterClient();

            // Setup Player
            SetupLocalUser();
        }

        /// <summary>
        /// IF MASTER CLIENT
        /// ----------------
        /// </summary>
        private void SetupMasterClient()
        {
            //Debug.Log("Test--------" + settings.NetObjects.Length);
            var objectToAddToPool = settings.NetObjects;
            CreateObjects_PhotonPool(objectToAddToPool);
        }

        /// <summary>
        /// IF MASTER CLIENT
        /// ----------------
        /// </summary>
        private void CreateObjects_PhotonPool(PUNPoolObject[] netObjects)
        {
            foreach (var netObject in netObjects)
            {
                if (netObject.toLoadOnStart)
                {
                    Debug.Log("#NetRoomManager#---------Instantiating network object: " + netObject.name + " at " + netObject.spawnPosition);

                    PhotonNetwork.Instantiate(netObject.name, netObject.spawnPosition, netObject.spawnRotation);
                }
            }
        }

        /// <summary>
        /// SETUP LOCAL USER
        /// ----------------
        /// </summary>
        private void SetupLocalUser()
        {
            // First create the head
            Transform headTransform = Camera.main.transform;
            var gameObject = PhotonNetwork.Instantiate(settings.PlayerNetHeadPrefab.name, headTransform.position, headTransform.rotation);
            gameObject.transform.parent = headTransform;
            // Then the hands
            PhotonNetwork.Instantiate(settings.PlayerNetHandsPrefab.name, Vector3.zero, Quaternion.identity);

            headTransform.parent.position = spawnpoints_Players[netEvents.NetInfo.userCount].position;
            //headTransform.parent.rotation = spawnpoints_Players[netEvents.NetInfo.userCount].rotation;

            // Setup properties
            _localPlayerHash.Add("Mod", true);
            _localPlayerHash.Add("Name", "Player " + netEvents.NetInfo.userCount);

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