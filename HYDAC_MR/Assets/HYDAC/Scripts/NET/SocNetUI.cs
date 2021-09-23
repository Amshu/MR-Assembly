using System;
using UnityEngine;

namespace HYDAC.Scripts.NET
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


        public event Action EUIRequestPromote;
        public void OnUIRequestPromote()
        {
            EUIRequestPromote?.Invoke();
        }

        public event Action EUserUpdate;

        public event Action<string> EUserNameUpdate;
    }
}