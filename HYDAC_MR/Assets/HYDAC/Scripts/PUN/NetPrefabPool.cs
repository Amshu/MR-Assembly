using Photon.Pun;
using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    public class NetPrefabPool: MonoBehaviourPunCallbacks, IPunPrefabPool
    {
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        public void Destroy(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }
    }
}