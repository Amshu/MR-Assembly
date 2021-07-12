using Normal.Realtime;
using UnityEngine;

namespace HYDAC.Scripts.NET
{
    public class NetManager : MonoBehaviour
    {
        [SerializeField] private Realtime _normRealtime = null;
        [SerializeField] private SocNetRoomInfo _socNetInfo = null;

        private RealtimeAvatarManager _normAvatarManager = null;

        private void Awake()
        {
            _normRealtime.didConnectToRoom += OnTryToConnect;
            
            //AdjustPosition(UsersInRoom, _modelTransform.position, _distanceFromModel);
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
                Debug.Log("#NetManager#-------------------------No of users in room: " + _socNetInfo.GetNoOfRemoteUsers);
                
                //AdjustPosition(UsersInRoom, _modelTransform.position, _distanceFromModel);
            }
            else
            {
                Debug.Log("#NetManager#-------------------------OnAvatarCreated: Remote created");
                _socNetInfo.UpdateNoOfRemoteUsers(_socNetInfo.GetNoOfRemoteUsers + 1);
            }
        }

        
    }
}
