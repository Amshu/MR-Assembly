using UnityEngine;
using Photon.Pun;

namespace HYDAC.Scripts.NET
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviour
    {
        [SerializeField] SocNetEvents netEvents;

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            InstantiatePoolObjects(netEvents.NetObjectsStructs);
        }

        private void InstantiatePoolObjects(NetObjectStruct[] prefabNames)
        {
            foreach(var objectStruct in prefabNames)
            {
                PhotonNetwork.Instantiate(objectStruct.obName, objectStruct.spawnPosition, objectStruct.spawnRotation);
            } 
        }
    }
}