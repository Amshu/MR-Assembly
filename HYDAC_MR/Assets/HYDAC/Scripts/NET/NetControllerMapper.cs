using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace com.HYDAC.Scripts.NET
{
    public class NetControllerMapper : MonoBehaviour
    {
        [SerializeField]
        Handedness _handedness = Handedness.None;

#if OCULUSINTEGRATION_PRESENT

        bool _initialized = false;
        Transform _controllerAnchor = null;

        bool InitializeTrackingReference()
        {
            if (_initialized)
                return true;

            if (_handedness != Handedness.Left && _handedness != Handedness.Right)
                return false;

            var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
            if (handJointService != null)
            {
                _controllerAnchor = handJointService.RequestJointTransform(TrackedHandJoint.Palm, _handedness);
            }

            return _initialized;
        }

        void LateUpdate()
        {
            if (!InitializeTrackingReference())
                return;

            Debug.Log("----------");

            transform.position = _controllerAnchor.position;
            transform.rotation = _controllerAnchor.rotation;
        }

#endif
    }
}