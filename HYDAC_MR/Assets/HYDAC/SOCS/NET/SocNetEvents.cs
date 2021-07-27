using System;
using UnityEngine;

namespace HYDAC.SOCS.NET
{
    [CreateAssetMenu(menuName = "NetSocs/NetEventsSoc", fileName = "SOC_NetEvents")]
    public class SocNetEvents : ScriptableObject
    {
        public event Action EOnJoinRoom;
        public event Action EOnJoinRoomFailed;
        public event Action EOnPlayerJoined;

        internal void BroadcastJoinedRoom()
        {
            EOnJoinRoom?.Invoke();
        }

        internal void BroadcastJoinRoomFailed()
        {
            EOnJoinRoomFailed?.Invoke();
        }

        internal void BroadcastPlayerJoined()
        {
            EOnPlayerJoined?.Invoke();
        }
    }
}