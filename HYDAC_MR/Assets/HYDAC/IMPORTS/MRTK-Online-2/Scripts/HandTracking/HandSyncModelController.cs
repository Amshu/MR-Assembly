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

using prvncher.MRTK_Online.TrackingHelpers;
using Photon.Pun;

using UnityEngine;

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

        bool _isOwnershipInitialized = false;

        bool _isHandTrackingActive;
        bool _isRightHandTrackingReliable;
        bool _isLeftHandTrackingReliable;


        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }


        void InitalizeLocalSystems()
        {
            if(_isOwnershipInitialized)// || !realtime.connected)
                return;
            
            _isOwnershipInitialized = true;
            
            leftHandSyncController.enabled = true;
            rightHandSyncController.enabled = true;

            leftControllerModelRoot.GetComponent<OculusControllerMapper>().enabled = true;
            rightControllerModelRoot.GetComponent<OculusControllerMapper>().enabled = true;

            foreach (var view in GetComponentsInChildren<PhotonView>())
            {
                view.RequestOwnership();
            }
        }

        void Update()
        {
            if (!_photonView.IsMine) return;

            InitalizeLocalSystems();
                
            _isHandTrackingActive = OVRPlugin.GetHandTrackingEnabled();
            _isRightHandTrackingReliable = rightHandSyncController.isHandTrackingConfidenceHigh;
            _isLeftHandTrackingReliable = leftHandSyncController.isHandTrackingConfidenceHigh;
                
            rightControllerModelRoot.SetActive(!_isHandTrackingActive);
            rightHandModelRoot.SetActive(_isHandTrackingActive);
                
            leftControllerModelRoot.SetActive(!_isHandTrackingActive);
            leftHandModelRoot.SetActive(_isHandTrackingActive);

            //else
            //{
            //    bool isHandTrackingActive = model.isHandTrackingActive;

            //    rightControllerModelRoot.SetActive(!isHandTrackingActive);
            //    rightHandModelRoot.SetActive(isHandTrackingActive && model.isRightHandTrackingReliable);

            //    leftControllerModelRoot.SetActive(!isHandTrackingActive);
            //    leftHandModelRoot.SetActive(isHandTrackingActive && model.isLeftHandTrackingReliable);
            //}
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //This script is local, you write to stream
                stream.SendNext(_isHandTrackingActive);
                stream.SendNext(_isRightHandTrackingReliable);
                stream.SendNext(_isLeftHandTrackingReliable);
            }
            else
            {
                // Network player, receive data
                //This script is receiving data from remote players script
                this._isHandTrackingActive = (bool)stream.ReceiveNext();
                this._isRightHandTrackingReliable = (bool)stream.ReceiveNext();
                this._isLeftHandTrackingReliable = (bool)stream.ReceiveNext();

                rightControllerModelRoot.SetActive(!_isHandTrackingActive);
                rightHandModelRoot.SetActive(_isHandTrackingActive && _isRightHandTrackingReliable);

                leftControllerModelRoot.SetActive(!_isHandTrackingActive);
                leftHandModelRoot.SetActive(_isHandTrackingActive && _isLeftHandTrackingReliable);
            }
        }
    }
}