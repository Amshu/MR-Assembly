using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;

namespace XPlatformHololens
{
    /// <summary>
    /// This abstract class forms the base class for handling various user interactable "Touch Events" through the Hololens, 
    /// such as Task UI buttons, Gauge UI and Checkbox UI elements.
    /// </summary>
    [RequireComponent(typeof(NearInteractionTouchable))]
    public abstract class TouchEventTrigger : MonoBehaviour, IMixedRealityTouchHandler
    {
        public int interactiveObjectNumber;
        protected bool _CanTrigger;

        protected virtual void Awake()
        {
            _CanTrigger = true;
        }

        #region MRTK Touch Handling Methods
        /// <summary>
        /// This abstract method is required for the various inherited classes, which detects the "Touch Event" when a user's hand interacts
        /// with an interactable object
        /// </summary>
        public abstract void OnTouchStarted(HandTrackingInputEventData eventData);

        /// <summary>
        /// Required method as part of IMixedRealityTouchHandler interface but unused.
        /// </summary>
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Required method as part of IMixedRealityTouchHandler interface but unused.
        /// </summary>
        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            //throw new System.NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Co-Routine method to apply a delay to prevent multiple touch triggers during a single interaction
        /// </summary>
        /// <returns></returns>
        protected IEnumerator DelayTouchActivation()
        {
            const float DELAY = 1f;

            _CanTrigger = false;

            yield return new WaitForSeconds(DELAY);

            _CanTrigger = true;
        }
    } 
}