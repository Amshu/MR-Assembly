using System;
using UnityEngine;

namespace HYDAC.Scripts.SOCS.NET
{
    [CreateAssetMenu(menuName = "Socks/Net/UI", fileName = "SOC_NetUI")]
    public class SocNetUI: ScriptableObject
    {
        /// <summary>
        /// For Join Room
        /// </summary>
        public event Action<string> EUIRequestJoinRoom;
        public void OnUIRequestJoinRoom(string roomName)
        {
            Debug.Log("#SocNetUI#-----------Join Room requested");
            EUIRequestJoinRoom?.Invoke(roomName);
        }

        public event Action<string> EUIUserNameChange;
        public void OnUIRequestNameChange(string newName)
        {
            EUIUserNameChange?.Invoke(newName);
        }
    }
}