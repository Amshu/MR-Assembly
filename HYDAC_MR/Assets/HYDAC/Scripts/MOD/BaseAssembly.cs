using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
    public class BaseAssembly : AUnit
    {
        // If you have multiple custom events, it is recommended to define them in the used class
        public const byte OnModuleChangeEventCode = 1;
        
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private AssemblyModule[] modules;

        // CAUTION: Take care while accessing SAssembly members in Awake -> AssemblyInfo has code to run first

        private void OnEnable()
        {
            //PhotonNetwork.NetworkingClient.EventReceived += OnPUNEvent;

            List<SModuleInfo> modules = new List<SModuleInfo>();
            
            var assemblyModules = transform.GetComponentsInChildren<AssemblyModule>();
            
            foreach (var module in assemblyModules)
            {
                module.EOnClicked += OnAssemblyModuleClicked;
                modules.Add(module.Info as SModuleInfo);
            }
            
            //((SAssemblyInfo) info).SetModules(modules.ToArray());
        }

        private void OnDisable()
        {
            //PhotonNetwork.NetworkingClient.EventReceived -= OnPUNEvent;

            var assemblyModules = transform.GetComponentsInChildren<AssemblyModule>();
            foreach (var module in assemblyModules)
            {
                module.EOnClicked -= OnAssemblyModuleClicked;
            }
        }

        private void OnAssemblyModuleClicked(SModuleInfo module)
        {
            Debug.Log("#BaseAssembly#--------------OnAssemblyModuleClicked: " + module.iname);
            
            int content = module.ID;
            
            // You would have to set the Receivers to All in order to receive this event on the local client as well
            //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
            //PhotonNetwork.RaiseEvent(OnModuleChangeEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
        
        //private void OnPUNEvent(EventData photonEvent)
        //{
        //    Debug.Log("#BaseAssembly#------------Network event received");

        //    byte eventCode = photonEvent.Code;
            
        //    if (eventCode == OnModuleChangeEventCode)
        //    {
        //        Debug.Log("#BaseAssembly#------------OnModuleChangeEventCode");

        //        int moduleID = (int)photonEvent.CustomData;

        //        NetworkRequestChangeModule(moduleID);
        //    }
        //}
        
        //private void NetworkRequestChangeModule(int moduleID)
        //{
        //    Debug.Log("#BaseAssembly#------------NetworkRequestChangeModule: " + moduleID);
            
        //    foreach (AssemblyModule module in modules)
        //    {
        //        Debug.Log("#BaseAssembly#------------Test: " + module.Info.ID);
        //        if (module.Info.ID == moduleID)
        //        {
        //            Debug.Log("#BaseAssembly#------------ModuleFound: " + module.Info.iname);
        //            assemblyEvents.OnModuleSelected((SModuleInfo)module.Info);
        //            return;
        //        }
        //    }
        //}
    }
}
