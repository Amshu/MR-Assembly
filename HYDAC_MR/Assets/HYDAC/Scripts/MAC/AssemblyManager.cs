using System;
using System.Collections;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

namespace HYDAC.Scripts.MAC
{
    public sealed class AssemblyManager : ExplodedViewManager, IAssembly
    {
        [Header("Assembly Members")]
        [SerializeField] private ObjectManipulator objectManipulator = null;
        [SerializeField] private MoveAxisConstraint axisConstraint = null;
        [SerializeField] private BoundsControl boundsControl = null;

        private bool _isInFocus = false;
        private Transform _defaultTransform;

        protected override void Awake()
        {
            base.Awake();
            
            objectManipulator.OnManipulationStarted.AddListener(OnClicked);
        }

        private void OnClicked(ManipulationEventData arg0)
        {
            Debug.Log("Object Clicked: " + name);

            if (_isInFocus) return;
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IAssembly).ToggleFocus(true);
        }
        
        
        public event Action<AssemblyManager> OnFocused;
        private void RaiseOnFocused(AssemblyManager managerRef)
        {
            OnFocused?.Invoke(managerRef);
        }
        
        void IAssembly.ToggleFocus(bool toggle, Material fadeMaterial = null)
        {
            _isInFocus = toggle;
            axisConstraint.enabled = !toggle;
            boundsControl.enabled = toggle;

            if (!toggle)
            {
                // Disappear
                
            }
        }

        void IAssembly.Reset()
        {
            StartCoroutine(LerpPosition(this.transform, _defaultTransform.position, timeTakenToDest));
        }
        
        private IEnumerator LerpPosition(Transform trans, Vector3 position, float timeTakenToDest)
        {
            var currentPos = trans.localPosition;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / timeTakenToDest;
                trans.localPosition = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }

            _mLock = false;
        }
    }
}
