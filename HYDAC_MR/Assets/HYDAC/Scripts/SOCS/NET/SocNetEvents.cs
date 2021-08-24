using Photon.Realtime;
using System;
using UnityEngine;

namespace HYDAC.Scripts.SOCS.NET
{
    public struct NetStructInfo
    {
        public bool isConnected;
        public bool inRoom;
        public string roomName;
        public int playerCount;

        public bool isMasterClient;
        public string localPlayerName;
        public Color localPlayerColour;

        public void Reset()
        {
            this.isConnected = true;
            this.inRoom = false;
            this.roomName = "";
            this.playerCount = 0;

            this.isMasterClient = false;
            this.localPlayerName = "";
            this.localPlayerColour = new Color(0, 0, 0);
        }
    }


    [CreateAssetMenu(menuName = "Socks/Net/NetEvents", fileName = "SOC_NetEvents")]
    public class SocNetEvents : ScriptableObject
    {
        private NetStructInfo _netInfo = new NetStructInfo();
        public NetStructInfo NetInfo => _netInfo;

        public event Action<NetStructInfo> ENetworkConnect;
        public event Action<NetStructInfo> ENetworkDisconnect;
        public event Action<NetStructInfo> EJoinRoom;
        public event Action<NetStructInfo> ELeftRoom;
        public event Action<NetStructInfo> EJoinRoomFailed;

        public event Action<NetStructInfo> EPlayerJoined;
        public event Action<NetStructInfo> EPlayerLeft;


        #region PhotonNetwork Callbacks

        internal void OnNetConnect()
        {
            NetInfo.Reset();

            ENetworkConnect?.Invoke(_netInfo);
        }

        internal void OnNetDisconnect()
        {
            _netInfo.isConnected = false;

            ENetworkDisconnect?.Invoke(_netInfo);
        }

        internal void OnNetJoinRoom(RoomInfo roomInfo)
        {
            _netInfo.inRoom = true;
            _netInfo.roomName = roomInfo.Name;
            _netInfo.playerCount = roomInfo.PlayerCount;

            EJoinRoom?.Invoke(_netInfo);
        }

        internal void OnNetLeftRoom()
        {
            _netInfo.inRoom = false;
            _netInfo.roomName = "";
            _netInfo.playerCount = 0;

            ELeftRoom?.Invoke(_netInfo);
        }

        internal void OnNetJoinRoomFailed()
        {
            EJoinRoomFailed?.Invoke(_netInfo);
        }

        internal void OnNetPlayerJoinedRoom(int numberOfPlayers)
        {
            _netInfo.playerCount = numberOfPlayers;

            EPlayerJoined?.Invoke(_netInfo);
        }

        internal void OnNetPlayerLeftRoom()
        {
            _netInfo.playerCount--;

            EPlayerLeft?.Invoke(_netInfo);
        }

        #endregion





        public event Action ENetRoomSetup;

        internal void SetupNetRoom()
        {
            ENetRoomSetup?.Invoke();
        }
        

        public event Action<Transform> ELocalUserReady;


        internal void OnPlayerReady(Transform playerTransform)
        {
            ELocalUserReady?.Invoke(playerTransform);
        }
        
        

    }
}