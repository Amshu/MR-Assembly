using UnityEngine;

using Photon.Pun;
using HYDAC.Scripts.SOCS.NET;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviour
    {
        [SerializeField] private SocNetEvents netEvents;

        [Space] 
        [SerializeField] private GameObject netPlayerPrefab;
        [SerializeField] private Transform[] playerSpawnPoints;
        



        private void InstantiateLocalPlayer(Transform spawnPoint)
        {
            Debug.LogFormat("#NetRoomManager#---------------Instantiating LocalPlayer");
            
        }
    }
}