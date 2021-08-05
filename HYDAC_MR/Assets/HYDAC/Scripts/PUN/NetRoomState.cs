using HYDAC.Scripts.MOD.SInfo;
using HYDAC.SOCS;
using Photon.Pun;
using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    public class NetRoomState: MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents;

        private void OnEnable()
        {
            assemblyEvents.ECurrentModuleChange += OnCurrentModuleChange;
        }

        private void OnDisable()
        {
            assemblyEvents.ECurrentModuleChange -= OnCurrentModuleChange;
        }

        private void OnCurrentModuleChange(SModuleInfo obj)
        {
            
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }
        
        
        
        
    }
}