using Photon.Pun;
using UnityEngine;

namespace com.HYDAC.Scripts.NET.Utils
{
    [RequireComponent(typeof(Renderer))]
    public class NetDisableMeshIfLocallyOwned : MonoBehaviour
    {
        [SerializeField]
        private PhotonView _realtimeView = null;

        Renderer _render = null;

        void Awake()
        {
            _render = GetComponent<Renderer>();

            if (_realtimeView != null && PhotonNetwork.IsConnected)
            {
                _render.enabled = !_realtimeView.IsMine;
                enabled = false;
            }
        }
    }
}