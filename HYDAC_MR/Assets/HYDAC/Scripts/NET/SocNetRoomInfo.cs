using System;
using UnityEngine;

namespace HYDAC.Scripts.NET
{
    public class SocNetRoomInfo : ScriptableObject
    {
        [SerializeField] private int _maxNoOfUsers = 0;
        private int GetMaxNoOfUsersAllowed => _maxNoOfUsers;


        private int _noOfRemoteUsers = 0;
        public int GetNoOfRemoteUsers => _noOfRemoteUsers;
        
        internal void UpdateNoOfRemoteUsers(int updatedNumber)
        {
            _maxNoOfUsers = updatedNumber;
        }
        
        private void Awake()
        {
            _noOfRemoteUsers = 0;
        }
    }
}
