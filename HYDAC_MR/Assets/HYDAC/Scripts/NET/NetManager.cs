using Normal.Realtime;
using UnityEngine;

namespace HYDAC.Scripts.NET
{
    public class NetManager : MonoBehaviour
    {
        [SerializeField] private Realtime normRealtime;

        private RealtimeAvatarManager _normAvatarManager = null; 
    
        private void Awake()
        {
            normRealtime.didConnectToRoom += OnTryToConnect;
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
            }
            else
            {
                Debug.Log("#NetManager#-------------------------OnAvatarCreated: Remote created");
            
            }
        }
    }
}
