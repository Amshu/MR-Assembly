using System;
using ExitGames.Client.Photon;
using HYDAC.Scripts.MOD.SInfo;
using HYDAC.SOCS;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        // If you have multiple custom events, it is recommended to define them in the used class
        public const byte ONModuleChangeEventCode = 1;
        
        
        [SerializeField] private SocAssemblyEvents assemblyEvents;

        private PhotonView _photonView = null;
        
        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnPUNEvent;
            
            assemblyEvents.ERequestChangeModule += OnModuleChangeRequest;

            _photonView = GetComponent<PhotonView>();
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnPUNEvent;

            assemblyEvents.ERequestChangeModule -= OnModuleChangeRequest;
        }

        private void OnModuleChangeRequest(SModuleInfo info)
        {
            Debug.Log("#MainManager#------------Raising Network event");

            int content = info.ID;
            // You would have to set the Receivers to All in order to receive this event on the local client as well
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
            PhotonNetwork.RaiseEvent(ONModuleChangeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
        
        private void OnPUNEvent(EventData photonEvent)
        {
            Debug.Log("#MainManager#------------Network event received");

            byte eventCode = photonEvent.Code;
            if (eventCode == ONModuleChangeEventCode)
            {
                int moduleID = (int)photonEvent.CustomData;

                NetworkRequestChangeModule(moduleID);
            }
        }

        private void NetworkRequestChangeModule(int moduleID)
        {
            Debug.Log("#MainManager#------------NetworkRequestChangeModule: " + moduleID);
            
            foreach (SModuleInfo module in assemblyEvents.CurrentAssembly.Modules)
            {
                if (module.ID == moduleID)
                {
                    assemblyEvents.OnCurrentModuleChange(module);
                }
            }
        }


        // private void RemoteOnModuleChange(SModuleInfo newModuleInfo){
        //
        //     if (PhotonNetwork.OfflineMode == true){ //use this you need to support offline mode.
        //         MyRemoteMethod(newModuleInfo.ID, new object [] { 42, true });
        //         return;
        //     }
        //     _photonView.RPC("MyRemoteMethod", PhotonTargets.Others, new object [] { 42, true }) 
        //        
        //     //Target Types
        //     //PhotonTargets.Others
        //     //PhotonTargets.All //triggered instantly
        //     //PhotonTargets.AllViaServer //local client gets even through server just like everyone else
        //     //PhotonTargets.MasterClient
        //     //PhotonNetwork.playerList[0]
        //     //PhotonTargets.AllBuffered
        //     //PhotonTargets.AllBufferedViaServer //used in item pickups where could be contested which client got it first
        //     //An important use is also when a new player connects later, they will recieve this 
        //     //buffered event that the item has been picked up and should be removed from scene
        // }
        //
        // [PunRPC]
        // void MyRemoteMethod(int moduleID, bool someBool) {
        //     Debug.Log(someNumber);
        //     Debug.Log(someBool);
        // }
    }
}
