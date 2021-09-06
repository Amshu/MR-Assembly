using UnityEngine;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for updating the Player MGR script when a generic tool (one without its own specific controller, eg. Drill)
    /// is grabbed and released.
    /// <br></br>
    /// <br></br>
    /// This is used to prevent tool selection on the left hand panel while the player is currently holding a generic tool.
    /// </summary>
    public class GenericObjectInteraction : MonoBehaviourPun, IPunObservable
    {
        // Private attributes
        private OVRGrabbableExtended _OVRGrabbable;
        private Rigidbody _Rigidbody;
        
        #region Unity Methods
        void Awake()
        {
            _OVRGrabbable = GetComponent<OVRGrabbableExtended>();
            _Rigidbody = GetComponent<Rigidbody>();
        }
        #endregion

        #region OnTrigger Events
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                // Determine which player is currently interacting with object
                PhotonView phView = other.gameObject.GetComponentInParent<PhotonView>();

                // Ignore if we are not the player
                if (!phView.IsMine)
                {
                    return;
                }

                // Transfer ownership to our player and disable rigidbody physics over the network
                photonView.TransferOwnership(phView.Owner);
                photonView.RPC("ToggleKinematic", RpcTarget.Others, true);
                _OVRGrabbable.allowOffhandGrab = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                photonView.RPC("ToggleKinematic", RpcTarget.Others, false);
                _OVRGrabbable.allowOffhandGrab = false;
                photonView.TransferOwnership(0);
            }
        }
        #endregion

        #region PUN SerializeView
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }

        [PunRPC]
        private void ToggleKinematic(bool toggle)
        {
            _Rigidbody.isKinematic = toggle;
        }
        #endregion
    }
}