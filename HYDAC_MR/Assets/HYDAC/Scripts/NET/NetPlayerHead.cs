using UnityEngine;
using TMPro;

using Photon.Pun;

public class NetPlayerHead : MonoBehaviour
{
    [SerializeField]
    TextMeshPro nameText;

    [SerializeField]
    MeshRenderer headMesh;

    PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();

        nameText.text = _photonView.Owner.ActorNumber.ToString();
    }
}
