using UnityEngine;

using Photon.Pun;

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

        private GameObject _localPlayer;

        private void Awake()
        {
            _isMasterClient = PhotonNetwork.IsMasterClient;
        }

        private void Start()
        {
            // If master client
            if (_isMasterClient)
                SetupMasterClient();

            // Setup Player
            CreateLocalPlayer();
        }

        private void SetupMasterClient()
        {
            //Debug.Log("Test--------" + settings.NetObjects.Length);
            var objectToAddToPool = settings.NetObjects;
            CreateObjects_PhotonPool(ref objectToAddToPool);
        }

        private void CreateObjects_PhotonPool(ref PUNPoolObject[] netObjects)
        {
            foreach (var netObject in netObjects)
            {
                if (netObject.toLoadOnStart)
                {
                    Debug.Log("#NetRoomManager#---------Instantiating network object: " + netObject.name);

                    PhotonNetwork.Instantiate(netObject.name, netObject.spawnPosition, netObject.spawnRotation);
                }
            }
        }

        private void CreateLocalPlayer()
        {
            // The total number of players in network room when joined
            int userRank = netEvents.NetInfo.userCount;

            // First create the head
            Transform headTransform = Camera.main.transform;
            var gameObject = PhotonNetwork.Instantiate(settings.PlayerNetHeadPrefab.name, headTransform.position, headTransform.rotation);
            gameObject.transform.parent = headTransform;

            // Then the hands
            PhotonNetwork.Instantiate(settings.PlayerNetHandsPrefab.name, Vector3.zero, Quaternion.identity);

        }


        
    }
}