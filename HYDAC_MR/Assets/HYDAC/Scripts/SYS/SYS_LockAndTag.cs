using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HYDAC.Scripts.INT
{
    public class SYS_LockAndTag : MonoBehaviour
    {
        [SerializeField] private string keyTag;

        [Range(0.1f, 5.0f)]
        public float InteractionTimeout;
        public bool IsBusy = false;

        public UnityEvent OnLock;
        public UnityEvent OnUnlock;

        public bool _isLocked = true;


        private void OnTriggerEnter(Collider other)
        {
            if (IsBusy) return;

            if (other.tag.Equals(keyTag))
            {
                OnKeyInteraction();
            }
        }

        private void OnKeyInteraction()
        {
            // Start countdown
            _isLocked = !_isLocked;

            Debug.Log("#INT_LockAndTag#----------OnKeyInteraction: " + _isLocked);

            if (!_isLocked) OnUnlock?.Invoke();
            else
                OnLock?.Invoke();

            StartCoroutine(InteractionTimeoutCountDown());
        }

        IEnumerator InteractionTimeoutCountDown()
        {
            IsBusy = true;

            yield return new WaitForSeconds(InteractionTimeout);

            IsBusy = false;
        }
    }
}