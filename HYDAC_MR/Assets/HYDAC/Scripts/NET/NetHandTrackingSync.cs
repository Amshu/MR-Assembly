using UnityEngine;

using System;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;


namespace com.HYDAC.Scripts.NET
{
    [Serializable]
    public class NetHandBoneMap
    {
        [SerializeField]
        TrackedHandJoint mrtkJointID;

        [SerializeField]
        Transform netJointTransform;

        private bool _isInitialised;
        private Handedness _handedness;
        private Transform relatedTransform;

        public Transform NetJointTransform => netJointTransform;

        public void Initialize(IMixedRealityHandJointService mrtkHandJoint, Handedness handedness)
        {
            if (_isInitialised) return;

            relatedTransform = mrtkHandJoint.RequestJointTransform(mrtkJointID, handedness);
            Debug.Log($"#NetHandBoneMap#-----------Intitialised {mrtkJointID}");

            _isInitialised = true;
        }


        public void UpdateTransform()
        {
            if (!_isInitialised)
            {
                Debug.Log($"Not initialised {mrtkJointID}");
                return;
            }

            netJointTransform.position = relatedTransform.position;
            netJointTransform.rotation = relatedTransform.rotation;
        }
    }


    public class NetHandTrackingSync : MonoBehaviour
    {
        public bool isHandTrackingConfidenceHigh = true;

        [SerializeField]
        Handedness handedness = Handedness.None;

        [SerializeField]
        NetHandBoneMap[] trackedBones;

        public NetHandBoneMap[] TrackedBones => trackedBones;

        private void Awake()
        {
            Initialize();
        }


        private void Initialize()
        {
            var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();

            if (handJointService != null)
            {
                foreach (var joint in trackedBones)
                {
                    joint.Initialize(handJointService, handedness);
                }

                isHandTrackingConfidenceHigh = true;
            }
            else
            {
                isHandTrackingConfidenceHigh = false;
            }
        }


        private void LateUpdate()
        {
            if (!isHandTrackingConfidenceHigh)
            {
                Initialize();
            }
            else
            {
                for (int i = 0; i < trackedBones.Length; i++)
                    trackedBones[i].UpdateTransform();
            }
        }
    }
}