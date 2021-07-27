using UnityEngine;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for generating a vertical raycast from the centre anchor of the VR headset.
    /// The raycast is used to detect which zone the local user is in, triggering a switch between a low resolution video clip (default)
    /// to a high resolution video clip (whilst the user remains in the zone).  
    /// <br></br>
    /// <br></br>
    /// NOTE: This functionality is being utilised to balance performance with quality (only one high resolution video can be played in the
    /// scene at a time).
    /// </summary>
    public class HeadsetRaycast : MonoBehaviourPun
    {
        #region Public and Private Attributes
        // Public Attributes
        public LayerMask validRaycastLayer;

        // Private Attributes
        private float _RaycastLengthVertical = 6f;             // 6 metres
        private PhotonView _LocalPhotonView;
        #endregion

        #region Unity Methods
        private void Start()
        {
            _LocalPhotonView = GetComponentInParent<PhotonView>();
        }

        private void Update()
        {
            if (_LocalPhotonView.IsMine)
            {
                // Fire Raycast upward and check if "HIT"
                if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, _RaycastLengthVertical, validRaycastLayer))
                {
                    // Do Something...
                }
                else
                {
                    // Do Something else...
                }
            }
        }
        #endregion
    }
}