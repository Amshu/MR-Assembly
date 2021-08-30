using UnityEngine;

using Photon.Pun;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;


namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for loading the main scene (Hydac_Factory.unity)
    /// and instantiating players into that scene.
    /// </summary>
    public class NetRoomManager : MonoBehaviour
    {
        [SerializeField] AssetReference[] refs;

        private void Awake()
        {
            LoadObjects();
        }

        private async Task LoadObjects()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;

            foreach (var reference in refs)
            {
                var handle = await Addressables.LoadAssetAsync<GameObject>(reference).Task;
                pool.ResourceCache.Add(handle.name, handle);

                PhotonNetwork.Instantiate(handle.name, handle.transform.position, handle.transform.rotation, 1);
            }
        }
    }
}