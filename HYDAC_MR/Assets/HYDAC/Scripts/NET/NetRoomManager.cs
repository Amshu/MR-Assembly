using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviour
    {
        [SerializeField] SocNetEvents netEvents;

        private GameObject _localPlayer;

        private void Start()
        {
            _localPlayer = PhotonNetwork.Instantiate(netEvents.LocalPlayerPrefabName,
                Vector3.zero, Quaternion.identity);

            if (!PhotonNetwork.IsMasterClient) return;

            InstantiatePoolObjects(netEvents.NetObjectPrefabs);
        }

        private void InstantiatePoolObjects(List<GameObject> prefabs)
        {
            foreach(var go in prefabs)
            {
                PhotonNetwork.Instantiate(go.name, go.transform.position, go.transform.rotation);
            }

            netEvents.NetObjectPrefabs.Clear();
        }
    }
}