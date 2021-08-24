using UnityEngine;
using UnityEngine.UI;

using TMPro;

using HYDAC.Scripts.SOCS.NET;
using System;

public class UINetInfo : MonoBehaviour
{
    [SerializeField] private SocNetEvents netEvents;

    [Space]
    [SerializeField] private TextMeshProUGUI isConnectedText;
    [SerializeField] private TextMeshProUGUI inRoomText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI isMasterClientText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerColorImage;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        netEvents.EJoinRoom += OnJoinedRoom;
    }

    private void OnJoinedRoom(NetStructInfo netInfo)
    {
        UpdateDetails(netInfo);
    }

    private void OnDisable()
    {
        netEvents.EJoinRoom -= OnJoinedRoom;
    }

    public void ShowNetworkDetails()
    {
        UpdateDetails(netEvents.NetInfo);
        _canvas.enabled = true;
    }

    private void UpdateDetails(NetStructInfo netInfo)
    {
        isConnectedText.text = netInfo.isConnected.ToString();
        inRoomText.text = netInfo.inRoom.ToString();
        roomNameText.text = netInfo.roomName;
        playerCountText.text = netInfo.playerCount.ToString();
        isMasterClientText.text = netInfo.isMasterClient.ToString();
        playerNameText.text = netInfo.localPlayerName;

        playerColorImage.color = netInfo.localPlayerColour;
    }
}
