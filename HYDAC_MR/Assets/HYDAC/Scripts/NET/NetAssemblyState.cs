using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    public class NetAssemblyState: MonoBehaviour, IOnEventCallback
    {
        /// <summary>
        /// Things to track
        ///
        /// - Selected Assembly
        /// - Selected Module
        /// - Is module exploded
        /// 
        /// </summary>
        /// <param name="photonEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnEvent(EventData photonEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}