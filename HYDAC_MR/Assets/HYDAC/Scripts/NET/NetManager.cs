using System.Numerics;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace HYDAC.Scripts.NET
{
    public class NetManager : MonoBehaviour
    {
        [SerializeField] private Realtime _normRealtime = null;
        
        [SerializeField] private Transform _spawnPoint = null;
        
        private RealtimeAvatarManager _normAvatarManager = null;

        private int _remoteUsersInRoom = 0;
        
        public int UsersInRoom => _remoteUsersInRoom + 1;

        private void Awake()
        {
            _normRealtime.didConnectToRoom += OnTryToConnect;
        }

        private void OnTryToConnect(Realtime realtime)
        {
            if (!realtime.connected)
            {
                Debug.Log("#NetManager#-------------------------OnTryConnect: Failed to connect");
                return;
            }
        
            Debug.Log("#NetManager#-------------------------OnTryConnect: Connected");

            _normAvatarManager = realtime.GetComponent<RealtimeAvatarManager>();
            _normAvatarManager.avatarCreated += OnAvatarCreated;

        }

        private void OnAvatarCreated(RealtimeAvatarManager avatarmanager, RealtimeAvatar avatar, bool islocalavatar)
        {
            if (islocalavatar)
            {
                Debug.Log("#NetManager#-------------------------OnAvatarCreated: Local avatar created");
                Debug.Log("#NetManager#-------------------------No of users in room: " + UsersInRoom);

                if(UsersInRoom > 1)
                    AdjustPosition(UsersInRoom);
            }
            else
            {
                Debug.Log("#NetManager#-------------------------OnAvatarCreated: Remote created");
                _remoteUsersInRoom++;
            }
        }

        private void AdjustPosition(int noOfUsers)
        {
            int offset = 1;
            for (int i = 0; i < UsersInRoom; i++)
            {
            }
        }
    }
}
