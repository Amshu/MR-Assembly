using Photon.Pun;

namespace com.HYDAC.Scripts.NET.Utils
{
    public class NetInteractableOwnership : MonoBehaviourPun
    {
        public void RequestOwnership()
        {
            if (!photonView.IsMine)
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        public void CancelOwnership()
        {
            if (photonView.IsMine)
                photonView.TransferOwnership(0);
        }
    }
}