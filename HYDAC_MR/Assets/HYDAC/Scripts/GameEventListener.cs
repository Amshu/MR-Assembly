using HYDAC.Scripts.INFO;
using UnityEngine;
using UnityEngine.Events;

namespace HYDAC.Scripts
{
    public class GameEventListener : MonoBehaviour
    {
        // The game event instance to register to.
        public SocGameEvent gameEvent;
        // The unity event response created for the event.
        public UnityEvent<SModuleInfo> response;

        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public void RaiseEvent(SModuleInfo moduleInfo)
        {
            response?.Invoke(moduleInfo);
        }
    }
}