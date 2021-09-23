using UnityEngine;
using TMPro;

using Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class NetPlayerHead : MonoBehaviourPunCallbacks
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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        Debug.Log("#NetRoomManager#-------------PlayerPropertiesUpdate - " + targetPlayer.ActorNumber);

        if (targetPlayer.Equals(_photonView.Owner))
        {
            Debug.Log("#NetRoomManager#-------------PlayerPropertiesUpdate - " + targetPlayer.ActorNumber);

            if (changedProps.ContainsKey("Name"))
            {
                Debug.Log("#NetRoomManager#-------------PlayerPropertiesUpdate - " + changedProps["Name"].ToString());

                nameText.text = changedProps["Name"].ToString();
            }

            if (changedProps.ContainsKey("ColorR"))
            {
                Debug.Log("#NetRoomManager#-------------PlayerPropertiesUpdate - " + changedProps["ColorR"].ToString());

                Color newColor = new Color();
                newColor.r = (float)changedProps["ColorR"];
                newColor.g = (float)changedProps["ColorG"];
                newColor.b = (float)changedProps["ColorB"];
                newColor.a = 1.0f;

                headMesh.material.SetColor("_RimColor", newColor);
                headMesh.material.SetColor("_InnerGlowColor", newColor);
            }
        }
    }
}
