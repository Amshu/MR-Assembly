using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

using Photon.Pun;

namespace com.HYDAC.Scripts.NET
{
    public class NetHandJointSync : MonoBehaviourPunCallbacks, IPunObservable
    {       
        [Header("Hands Sync Helpers")]
        [SerializeField]
        NetHandTrackingSync rightHandSyncController = null;
        
        [SerializeField]
        NetHandTrackingSync leftHandSyncController = null;

        [Header("Hands")]
        [SerializeField]
        GameObject rightHandModelRoot = null;

        [SerializeField]
        GameObject leftHandModelRoot = null;

        [SerializeField]
        GameObject jointProxyMesh;


        PhotonView _photonView;

        bool _isRightHandTrackingReliable;
        bool _isLeftHandTrackingReliable;

        IMixedRealityHandJointService mrtkHandjointService;

        private void Awake()
        {
            _photonView = GetComponentInParent<PhotonView>();


            MixedRealityHandTrackingProfile handProfile = null;
            if (CoreServices.InputSystem?.InputSystemProfile != null)
            {
                handProfile = CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile;
                handProfile.EnableHandJointVisualization = false;
            }

            mrtkHandjointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
            

            if (_photonView.IsMine)
            {
                leftHandSyncController.enabled = true;
                rightHandSyncController.enabled = true;
            }
            else
            {
                leftHandSyncController.enabled = false;
                rightHandSyncController.enabled = false;

                SpawnProxies();
            }
        }

        private void SpawnProxies()
        {
            foreach(NetHandBoneMap bone in leftHandSyncController.TrackedBones)
            {
                Instantiate(jointProxyMesh, bone.NetJointTransform);
            }

            foreach (NetHandBoneMap bone in rightHandSyncController.TrackedBones)
            {
                Instantiate(jointProxyMesh, bone.NetJointTransform);
            }
        }

        void LateUpdate()
        {
            if (!_photonView.IsMine) return;

            _isRightHandTrackingReliable = mrtkHandjointService.IsHandTracked(Handedness.Right);
            _isLeftHandTrackingReliable = mrtkHandjointService.IsHandTracked(Handedness.Left);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_isRightHandTrackingReliable);
                stream.SendNext(_isLeftHandTrackingReliable);
            }
            else
            {
                this._isRightHandTrackingReliable = (bool)stream.ReceiveNext();
                this._isLeftHandTrackingReliable = (bool)stream.ReceiveNext();

                rightHandModelRoot.SetActive(_isRightHandTrackingReliable);
                leftHandModelRoot.SetActive(_isLeftHandTrackingReliable);
            }
        }
    }
}