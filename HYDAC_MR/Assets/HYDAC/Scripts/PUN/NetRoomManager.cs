using UnityEngine;
using UnityEngine.AddressableAssets;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using HYDAC.Scripts.SOCS;
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
        [SerializeField] private SocMainSettings settings;

        [Space] 
        [SerializeField] private int customManualInstantiationEventCode;
        [SerializeField] private AssetReference playerPrefab;
        [SerializeField] private GameObject netPlayerPrefab;
        [SerializeField] private Transform[] playerSpawnPoints;

        #region Public and Private Methods

        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        
        private void InstantiateLocalPlayer(Transform spawnPoint)
        {
            Debug.LogFormat("#NetRoomManager#---------------Instantiating LocalPlayer");
            
            PhotonNetwork.Instantiate(netPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation, 0);
        }
    }
}