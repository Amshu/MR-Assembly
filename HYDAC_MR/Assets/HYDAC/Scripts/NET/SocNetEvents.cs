using Photon.Realtime;
using System;
using UnityEngine;

namespace HYDAC.Scripts.NET
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

    public struct NetObjectStruct
    {
        public string obName;
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
    }


    [CreateAssetMenu(menuName = "Socks/Net/NetEvents", fileName = "SOC_NetEvents")]
    public class SocNetEvents : ScriptableObject
    {
        private NetStructInfo _netInfo = new NetStructInfo();
        public NetStructInfo NetInfo => _netInfo;

        public NetObjectStruct[] NetObjectsStructs;

        public event Action<NetStructInfo> ENetworkConnected;
        public event Action<NetStructInfo> ENetworkDisconnected;
        public event Action<NetStructInfo> EJoinedRoom;
        public event Action<NetStructInfo> ELeftRoom;
        public event Action<NetStructInfo> EJoinRoomFailed;

        public event Action<NetStructInfo> EPlayerJoined;
        public event Action<NetStructInfo> EPlayerLeft;


        public event Action TestNetworkAutoJoin;
        public void AutoJoinCheck()
        {
            TestNetworkAutoJoin?.Invoke();
        }


        #region PhotonNetwork Callbacks

        internal void OnNetConnect()
        {
            NetInfo.Reset();

            ENetworkConnected?.Invoke(_netInfo);
        }

        internal void OnNetDisconnect()
        {
            _netInfo.isConnected = false;

            ENetworkDisconnected?.Invoke(_netInfo);
        }

        internal void OnNetJoinRoom(RoomInfo roomInfo)
        {
            _netInfo.inRoom = true;
            _netInfo.roomName = roomInfo.Name;
            _netInfo.playerCount = roomInfo.PlayerCount;

            EJoinedRoom?.Invoke(_netInfo);
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


        public event Action<bool> EPreparePUNPool;
        internal void InvokePreparePUNPool(bool toPrepare)
        {
            EPreparePUNPool?.Invoke(toPrepare);
        }

        public event Action<GameObject[]> EPUNPoolPrepared;
        internal void InvokePUNPoolPrepared(GameObject[] preparedPools)
        {
            Debug.Log("##------------- " + preparedPools.Length);

            EPUNPoolPrepared?.Invoke(preparedPools);
        }


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