using System.Collections.Generic;
using HYDAC.Scripts.INFO;
using UnityEngine;

namespace HYDAC.Scripts
{
    [CreateAssetMenu(fileName = "SocEvent_", menuName = "SocEvents/GameEvent")]
    public class SocGameEvent : ScriptableObject {

        private List<GameEventListener> listeners = new List<GameEventListener>();
        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }

        public void Raise(SModuleInfo moduleInfo)
        {
            for (int i = listeners.Count - 1; i >= 0; --i)
            {
                listeners[i].RaiseEvent(moduleInfo);
            }
        }
    }
}