using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.SOCS.NET
{
    [CreateAssetMenu(menuName = "Socks/Net/NetEvents", fileName = "SOC_NetEvents")]
    public class SocNetEvents : ScriptableObject
    {
        public event Action ENetworkConnect;
        public event Action ENetworkDisconnect;
        public event Action<string> EJoinRoom;
        public event Action EJoinRoomFailed;

        internal void OnNetworkConnect()
        {
            ENetworkConnect?.Invoke();
        }

        internal void InvokeNetworkDisconnect()
        {
            ENetworkDisconnect?.Invoke();
        }
        
        internal void InvokeJoinedRoom(string sceneGUIDKey)
        {
            EJoinRoom?.Invoke(sceneGUIDKey);
        }

        internal void InvokeJoinRoomFailed()
        {
            EJoinRoomFailed?.Invoke();
        }



        public event Action ENetRoomSetup;

        internal void SetupNetRoom()
        {
            ENetRoomSetup?.Invoke();
        }
        

        public event Action<Transform> ELocalUserReady;
        public event Action EPlayerJoined;
        public event Action EPlayerLeft;

        internal void OnPlayerReady(Transform playerTransform)
        {
            ELocalUserReady?.Invoke(playerTransform);
        }
        
        internal void OnPlayerJoined()
        {
            EPlayerJoined?.Invoke();
        }

        internal void OnPlayerLeft()
        {
            EPlayerLeft?.Invoke();
        }
        
    }
}