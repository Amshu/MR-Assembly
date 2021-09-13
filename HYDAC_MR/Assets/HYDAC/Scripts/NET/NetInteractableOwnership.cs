using Photon.Pun;

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
