using UnityEngine;

using System;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;

namespace com.HYDAC.Scripts.NET
{
    [Serializable]
    public class NetHandJointMap
    {
        public TrackedHandJoint mrtkJointID;
        public TrackedHandJoint MRTKJointID => mrtkJointID;

        public Transform netJointTransform;
        public Transform NetJointTransform => netJointTransform;

        public Transform relatedTransform;
        public Transform RelatedTransform => relatedTransform;


        private bool _isInitialised;

        public NetHandJointMap(TrackedHandJoint jointName, Transform followTransform, Transform trackedTransform)
        {
            this.mrtkJointID = jointName;
            this.netJointTransform = followTransform;
            this.relatedTransform = trackedTransform;

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


    public class NetHandJointTrackingSync : MonoBehaviour
    {
        public bool isHandTrackingConfidenceHigh = true;

        [SerializeField]
        Handedness handedness = Handedness.None;

        [SerializeField]
        NetHandJointMap[] netJointsMap;

        public NetHandJointMap[] TrackedBones => netJointsMap;

        /// <summary>
        /// Wrist Transform
        /// </summary>
        public Transform Wrist;

        /// <summary>
        /// Palm transform
        /// </summary>
        public Transform Palm;

        /// <summary>
        /// Thumb metacarpal transform  (thumb root)
        /// </summary>
        public Transform ThumbRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool ThumbRootIsMetacarpal = true;

        /// <summary>
        /// Index metacarpal transform (index finger root)
        /// </summary>
        public Transform IndexRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool IndexRootIsMetacarpal = true;

        /// <summary>
        /// Middle metacarpal transform (middle finger root)
        /// </summary>
        public Transform MiddleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool MiddleRootIsMetacarpal = true;

        /// <summary>
        /// Ring metacarpal transform (ring finger root)
        /// </summary>
        public Transform RingRoot;

        [Tooltip("Ring finger node is metacarpal joint.")]
        public bool RingRootIsMetacarpal = true;

        /// <summary>
        /// Pinky metacarpal transform (pinky finger root)
        /// </summary>
        public Transform PinkyRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool PinkyRootIsMetacarpal = true;

        [Tooltip("If non-zero, this vector and the modelPalmFacing vector " +
        "will be used to re-orient the Transform bones in the hand rig, to " +
        "compensate for bone axis discrepancies between Leap Bones and model " +
        "bones.")]
        public Vector3 ModelFingerPointing = new Vector3(0, 0, 0);

        [Tooltip("If non-zero, this vector and the modelFingerPointing vector " +
        "will be used to re-orient the Transform bones in the hand rig, to " +
        "compensate for bone axis discrepancies between Leap Bones and model " +
        "bones.")]
        public Vector3 ModelPalmFacing = new Vector3(0, 0, 0);

        [Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this if your model's palm transform is at the center of the palm similar to Leap API hands.")]
        public bool ModelPalmAtLeapWrist = true;

        public Quaternion wristOffset;

        /// <summary>
        /// Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector.
        /// </summary>
        private Quaternion userBoneRotation
        {
            get
            {
                if (ModelFingerPointing == Vector3.zero || ModelPalmFacing == Vector3.zero)
                {
                    return Quaternion.identity;
                }
                return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
            }
        }

        IMixedRealityHandJointService handJointService;

        private void Awake()
        {
            Initialize();
        }


        private void Initialize()
        {
            handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();

            if (handJointService != null)
            {
                InitialiseHandJoints();

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
                UpdateRotation();
            }
        }


        private Dictionary<TrackedHandJoint, Transform> netHandJoints = new Dictionary<TrackedHandJoint, Transform>();
        private void InitialiseHandJoints()
        {
            // Initialize joint dictionary with their corresponding joint transforms
            netHandJoints[TrackedHandJoint.Wrist] = Wrist;
            netHandJoints[TrackedHandJoint.Palm] = Palm;

            // Thumb joints, first node is user assigned, note that there are only 4 joints in the thumb
            if (ThumbRoot)
            {
                if (ThumbRootIsMetacarpal)
                {
                    netHandJoints[TrackedHandJoint.ThumbMetacarpalJoint] = ThumbRoot;
                    netHandJoints[TrackedHandJoint.ThumbProximalJoint] = RetrieveChild(TrackedHandJoint.ThumbMetacarpalJoint);
                }
                else
                {
                    netHandJoints[TrackedHandJoint.ThumbProximalJoint] = ThumbRoot;
                }
                netHandJoints[TrackedHandJoint.ThumbDistalJoint] = RetrieveChild(TrackedHandJoint.ThumbProximalJoint);
                netHandJoints[TrackedHandJoint.ThumbTip] = RetrieveChild(TrackedHandJoint.ThumbDistalJoint);
            }
            // Look up index finger joints below the index finger root joint
            if (IndexRoot)
            {
                if (IndexRootIsMetacarpal)
                {
                    netHandJoints[TrackedHandJoint.IndexMetacarpal] = IndexRoot;
                    netHandJoints[TrackedHandJoint.IndexKnuckle] = RetrieveChild(TrackedHandJoint.IndexMetacarpal);
                }
                else
                {
                    netHandJoints[TrackedHandJoint.IndexKnuckle] = IndexRoot;
                }
                netHandJoints[TrackedHandJoint.IndexMiddleJoint] = RetrieveChild(TrackedHandJoint.IndexKnuckle);
                netHandJoints[TrackedHandJoint.IndexDistalJoint] = RetrieveChild(TrackedHandJoint.IndexMiddleJoint);
                netHandJoints[TrackedHandJoint.IndexTip] = RetrieveChild(TrackedHandJoint.IndexDistalJoint);
            }

            // Look up middle finger joints below the middle finger root joint
            if (MiddleRoot)
            {
                if (MiddleRootIsMetacarpal)
                {
                    netHandJoints[TrackedHandJoint.MiddleMetacarpal] = MiddleRoot;
                    netHandJoints[TrackedHandJoint.MiddleKnuckle] = RetrieveChild(TrackedHandJoint.MiddleMetacarpal);
                }
                else
                {
                    netHandJoints[TrackedHandJoint.MiddleKnuckle] = MiddleRoot;
                }
                netHandJoints[TrackedHandJoint.MiddleMiddleJoint] = RetrieveChild(TrackedHandJoint.MiddleKnuckle);
                netHandJoints[TrackedHandJoint.MiddleDistalJoint] = RetrieveChild(TrackedHandJoint.MiddleMiddleJoint);
                netHandJoints[TrackedHandJoint.MiddleTip] = RetrieveChild(TrackedHandJoint.MiddleDistalJoint);
            }

            // Look up ring finger joints below the ring finger root joint
            if (RingRoot)
            {
                if (RingRootIsMetacarpal)
                {
                    netHandJoints[TrackedHandJoint.RingMetacarpal] = RingRoot;
                    netHandJoints[TrackedHandJoint.RingKnuckle] = RetrieveChild(TrackedHandJoint.RingMetacarpal);
                }
                else
                {
                    netHandJoints[TrackedHandJoint.RingKnuckle] = RingRoot;
                }
                netHandJoints[TrackedHandJoint.RingMiddleJoint] = RetrieveChild(TrackedHandJoint.RingKnuckle);
                netHandJoints[TrackedHandJoint.RingDistalJoint] = RetrieveChild(TrackedHandJoint.RingMiddleJoint);
                netHandJoints[TrackedHandJoint.RingTip] = RetrieveChild(TrackedHandJoint.RingDistalJoint);
            }

            // Look up pinky joints below the pinky root joint
            if (PinkyRoot)
            {
                if (PinkyRootIsMetacarpal)
                {
                    netHandJoints[TrackedHandJoint.PinkyMetacarpal] = PinkyRoot;
                    netHandJoints[TrackedHandJoint.PinkyKnuckle] = RetrieveChild(TrackedHandJoint.PinkyMetacarpal);
                }
                else
                {
                    netHandJoints[TrackedHandJoint.PinkyKnuckle] = PinkyRoot;
                }
                netHandJoints[TrackedHandJoint.PinkyMiddleJoint] = RetrieveChild(TrackedHandJoint.PinkyKnuckle);
                netHandJoints[TrackedHandJoint.PinkyDistalJoint] = RetrieveChild(TrackedHandJoint.PinkyMiddleJoint);
                netHandJoints[TrackedHandJoint.PinkyTip] = RetrieveChild(TrackedHandJoint.PinkyDistalJoint);
            }

            MapHandBones();
        }
        private Transform RetrieveChild(TrackedHandJoint parentJoint)
        {
            if (netHandJoints[parentJoint] != null && netHandJoints[parentJoint].childCount > 0)
            {
                return netHandJoints[parentJoint].GetChild(0);
            }
            return null;
        }

        void MapHandBones()
        {
            var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();

            List<NetHandJointMap> jointsMapList = new List<NetHandJointMap>();

            foreach (var joint in netHandJoints)
            {
                var mrtkJoint = handJointService.RequestJointTransform(joint.Key, handedness);
                NetHandJointMap jointMap = new NetHandJointMap(joint.Key, joint.Value, mrtkJoint);
                jointsMapList.Add(jointMap);
            }

            netJointsMap = jointsMapList.ToArray();
        }

        void UpdateRotation()
        {
            for (int i = 0; i < netJointsMap.Length; i++)
                netJointsMap[i].UpdateTransform();

            // Render the rigged hand mesh itself
            Transform jointTransform;
            // Apply updated TrackedHandJoint pose data to the assigned transforms
            foreach (NetHandJointMap handJoint in netJointsMap)
            {
                if (netHandJoints.TryGetValue(handJoint.MRTKJointID, out jointTransform))
                {
                    if (jointTransform != null)
                    {
                        if (handJoint.MRTKJointID == TrackedHandJoint.Palm)
                        {
                            if (ModelPalmAtLeapWrist)
                            {
                                //Palm.position = eventData.InputData[TrackedHandJoint.Wrist].Position;
                                Palm.position = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, handedness).position;
                            } 
                            else
                            {
                                Palm.position = handJointService.RequestJointTransform(TrackedHandJoint.Palm, handedness).position;
                            }
                            Palm.rotation = handJointService.RequestJointTransform(TrackedHandJoint.Palm, handedness).rotation * userBoneRotation;// * wristOffset;
                        }
                        else if (handJoint.MRTKJointID == TrackedHandJoint.Wrist)
                        {
                            if (!ModelPalmAtLeapWrist)
                            {
                                Wrist.position = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, handedness).position;
                            }
                        }
                        else
                        {
                            // Finger joints
                            jointTransform.rotation = handJoint.RelatedTransform.rotation * Reorientation();
                            
                            jointTransform.position = handJoint.RelatedTransform.position;
                        }
                    }
                }
            }
        }

        private Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
        }
    }
}