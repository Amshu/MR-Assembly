//------------------------------------------------------------------------------ -
//MRTK - Quest - Online 2
//https ://github.com/provencher/MRTK-Quest-Online
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

using Photon.Pun;

namespace prvncher.MRTK_Online.HandTracking
{
    public class HandSyncModelController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Header("Controller")]
        [SerializeField]
        GameObject rightControllerModelRoot = null;
        
        [SerializeField]
        GameObject leftControllerModelRoot = null;

        [Header("Hands")]
        [SerializeField]
        GameObject rightHandModelRoot = null;

        [SerializeField]
        GameObject leftHandModelRoot = null;
        
        [Header("Hands Sync Helpers")]
        [SerializeField]
        OculusHandTrackingSync rightHandSyncController = null;
        
        [SerializeField]
        OculusHandTrackingSync leftHandSyncController = null;

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

                //SpawnProxies();
            }
        }

        //private void SpawnProxies()
        //{
        //    foreach(NetHandBoneMap bone in leftHandSyncController.TrackedBones)
        //    {
        //        Instantiate(jointProxyMesh, bone.NetJointTransform);
        //    }

        //    foreach (NetHandBoneMap bone in rightHandSyncController.TrackedBones)
        //    {
        //        Instantiate(jointProxyMesh, bone.NetJointTransform);
        //    }
        //}

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