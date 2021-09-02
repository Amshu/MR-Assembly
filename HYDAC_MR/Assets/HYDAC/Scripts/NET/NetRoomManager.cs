using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AddressableAssets;
using HYDAC.Scripts.ADD;

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
            Debug.Log("Test--------" + settings.PUNPoolObjectStructs.Length);

            CreateObjects_PhotonPool(settings.PUNPoolObjectStructs);
        }

        private void CreateObjects_PhotonPool(PUNPoolObjectStruct[] prefabs)
        {
            foreach (var prefabInfo in prefabs)
            {
                Debug.Log("#NetRoomManager#---------Instantiating network object: " + prefabInfo.name);

                PhotonNetwork.Instantiate(prefabInfo.name, prefabInfo.transform.position, prefabInfo.transform.rotation);
            }
        }

        private void CreateLocalPlayer()
        {
            // The total number of players in network room when joined
            int userRank = netEvents.NetInfo.userCount;

            // Create local player prefab
            _localPlayer = PhotonNetwork.Instantiate(settings.LocalPlayerPrefabname,
                spawnpoints_Players[userRank].position,
                spawnpoints_Players[userRank].rotation);
        }


        
    }
}