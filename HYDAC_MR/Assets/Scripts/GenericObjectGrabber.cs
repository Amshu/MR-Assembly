using UnityEngine;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    [RequireComponent(typeof(PhotonView))]
    /// <summary>
    /// This class is responsible for allowing a player to grab a generic object in the scene, which is synced over the network.
    /// </summary>
    public class GenericObjectGrabber : MonoBehaviourPun, IPunObservable
    {
        private OVRGrabbableExtended _OVRGrabbable;

        private void Awake()
        {
            _OVRGrabbable = GetComponent<OVRGrabbableExtended>();
            _OVRGrabbable.allowOffhandGrab = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                PhotonView phView = other.gameObject.GetComponentInParent<PhotonView>();

                if (!phView.IsMine)
                {
                    return;
                }

                photonView.TransferOwnership(phView.Owner);
                _OVRGrabbable.allowOffhandGrab = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                _OVRGrabbable.allowOffhandGrab = false;

                photonView.TransferOwnership(0);
                
            }
        }

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
    }
}