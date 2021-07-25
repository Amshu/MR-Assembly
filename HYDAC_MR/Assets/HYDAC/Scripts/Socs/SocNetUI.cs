using System;
using UnityEngine;

namespace HYDAC.Scripts.Socs
{
    [CreateAssetMenu(menuName = "UISocs/NetUISoc", fileName = "NetUISoc")]
    public class SocNetUI: ScriptableObject
    {
        /// <summary>
        /// For Join Room
        /// </summary>
        public event Action<string> EUIRequestJoinRoom;
        public void OnUIRequestJoinRoom(string roomName)
        {
            EUIRequestJoinRoom?.Invoke(roomName);
        }

        public event Action<string> EUIUserNameChange;
        public void OnUIRequestNameChange(string newName)
        {
            EUIUserNameChange?.Invoke(newName);
        }
    }
}