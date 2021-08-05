using HYDAC.SOCS.NET;
using Photon.Pun;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class FocusedModuleHolder: MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private SocNetEvents netEvents;
        
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}