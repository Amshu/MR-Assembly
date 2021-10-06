using System;
using System.Collections.Generic;
using UnityEngine;

namespace HYDAC.Scripts.NET
{
    public struct UserStructInfo
    {
        public int UserID;
        public bool IsMod;

        public string UserName;
        public Color UserColor;
    }

    public class NetInfo
    {
        public bool isConnected;
        public bool inRoom;
        public string roomName;
        public int userCount;

        public bool isMasterClient;
        public string localPlayerName;
        public Color localPlayerColour;

        private List<UserStructInfo> usersInRoom = new List<UserStructInfo>();
        public UserStructInfo[] UsersInRoom => usersInRoom.ToArray();

        public void Reset()
        {
            this.isConnected = true;
            this.inRoom = false;
            this.roomName = "";
            this.userCount = 0;

            this.isMasterClient = false;
            this.localPlayerName = "";
            this.localPlayerColour = new Color(0, 0, 0);
        }

        public void AddUser(UserStructInfo user)
        {
            if (usersInRoom.Contains(user))
                return;

            usersInRoom.Add(user);
        }

        public void RemoveUser(int userID)
        {
            if (usersInRoom.Exists(item => item.UserID == userID))
            {
                var userOnList = usersInRoom.Find(item => item.UserID == userID);
                usersInRoom.Remove(userOnList);
            }
        }

        public void UpdateUserProperties(int userID, UserStructInfo newInfo)
        {
            var oldInfo = usersInRoom.Find(item => item.UserID == userID);
            int index = usersInRoom.IndexOf(oldInfo);

            if (index != -1)
                usersInRoom[index] = newInfo;
        }

        public UserStructInfo GetUserProperties(int userID)
        {
            return usersInRoom.Find(item => item.UserID == userID);
        }
    }


    [CreateAssetMenu(menuName = "Socks/Net/NetEvents", fileName = "SOC_NetEvents")]
    public class SocNetEvents : ScriptableObject
    {
        private NetInfo _netInfo = new NetInfo();
        public NetInfo NetInfo => _netInfo;

        public event Action<NetInfo> ENetworkConnected;
        public event Action<NetInfo> ENetworkDisconnected;
        public event Action<NetInfo> EJoinedRoom;
        public event Action<NetInfo> ELeftRoom;
        public event Action<NetInfo> EJoinRoomFailed;

        public event Action<NetInfo> EUserJoined;
        public event Action<NetInfo> EUserLeft;

        public event Action<string> EUserNameChanged;
        public event Action<Color> EUserColorChanged;
        public event Action<bool> EUserModChanged;

        private void OnEnable()
        {
            _netInfo = new NetInfo();
        }


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

        internal void OnNetJoinRoom(string roomName, int noOfUsersInRoom)
        {
            _netInfo.inRoom = true;
            _netInfo.roomName = roomName;
            _netInfo.userCount = noOfUsersInRoom;

            EJoinedRoom?.Invoke(_netInfo);
        }

        internal void OnNetLeftRoom()
        {
            _netInfo.inRoom = false;
            _netInfo.roomName = "";
            _netInfo.userCount = 0;

            ELeftRoom?.Invoke(_netInfo);
        }

        internal void OnNetJoinRoomFailed()
        {
            EJoinRoomFailed?.Invoke(_netInfo);
        }

        internal void OnNetPlayerJoinedRoom(int numberOfPlayers)
        {
            _netInfo.userCount = numberOfPlayers;

            EUserJoined?.Invoke(_netInfo);
        }

        internal void OnNetPlayerLeftRoom()
        {
            _netInfo.userCount--;

            EUserLeft?.Invoke(_netInfo);
        }

        #endregion


    }
}