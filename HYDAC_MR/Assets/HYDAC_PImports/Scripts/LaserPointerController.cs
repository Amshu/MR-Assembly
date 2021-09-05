using UnityEngine;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for controlling the Laser Pointer tool, generating the "laser-beam" and hit point objects, as well
    /// as syncing these objects over the network.
    /// </summary>
    public class LaserPointerController : MonoBehaviourPun, IPunObservable
    {
        #region Public and Private Attributes
        // Public Attributes
        public GameObject laserBeam;
        public GameObject laserHitPoint;
        public LayerMask validLaserHitLayer;

        // Private Attributes
        private const float LASER_CENTRE_POINT = 1.5f;              // Half the length of the beam (in z-axis), update if this changes
        private Material _NormalLaserHitPoint;                      // Red Laser Point
        private OVRGrabbableExtended _OVRGrabbable;
        private bool _LaserOn;
        private Vector3 _LaserLength;
        private DEP_NetPlayerManager _netPlayerMgr;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            _OVRGrabbable = GetComponent<OVRGrabbableExtended>();
            _netPlayerMgr = GetComponentInParent<DEP_NetPlayerManager>();
            _NormalLaserHitPoint = laserHitPoint.GetComponent<MeshRenderer>().material;
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                laserBeam.SetActive(false);
                laserHitPoint.SetActive(false);
                _LaserLength = laserBeam.transform.localScale;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.IsMine)
            {
                if (_OVRGrabbable.isGrabbed)
                {
                    // Update Player Mgr
                    //if (!_PlayerMGR.CurrentlyGrabbingTool)
                    //{
                    //    _PlayerMGR.CurrentlyGrabbingTool = true;
                    //}

                    // Show and update laser beam
                    if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) || (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp)))
                    {
                        // Turn on laser
                        if (!laserBeam.activeSelf)
                        {
                            _LaserOn = true;
                            ActivateLaserBeam(_LaserOn);
                        }

                        // Shoot RayCast and check if hit object collider (only applies to valid Layer)
                        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, validLaserHitLayer))
                        {
                            // Position and scale the laser
                            laserBeam.transform.position = Vector3.Lerp(transform.position, hit.point, 0.5f);
                            laserBeam.transform.localScale = new Vector3(laserBeam.transform.localScale.x, laserBeam.transform.localScale.y, hit.distance);

                            // Get hit point and activate hit point object
                            laserHitPoint.transform.position = hit.point;
                            laserHitPoint.SetActive(true);
                        }
                        else
                        {
                            laserHitPoint.SetActive(false);
                            laserBeam.transform.localScale = _LaserLength;
                            laserBeam.transform.localPosition = Vector3.zero;
                            laserBeam.transform.Translate(Vector3.forward * LASER_CENTRE_POINT);
                        }
                    }
                    else
                    {
                        // Turn off beam when trigger button is released
                        if (laserBeam.activeSelf)
                        {
                            DeactivateLaser();
                        }
                    }
                }
                else
                {
                    // Turn off beam when player releases laser pointer tool
                    if (laserBeam.activeSelf)
                    {
                        DeactivateLaser();
                    }

                    // Update Player Mgr
                    //if (_PlayerMGR.CurrentlyGrabbingTool)
                    //{
                    //    _PlayerMGR.CurrentlyGrabbingTool = false;
                    //}
                }
            }
        }
        #endregion

        #region Laser Methods
        private void DeactivateLaser()
        {
            _LaserOn = false;
            laserHitPoint.SetActive(false);
            laserBeam.SetActive(false);
        }

        /// <summary>
        /// Toggles the laser beam on / off
        /// </summary>
        /// <param name="activate"></param>
        private void ActivateLaserBeam(bool activate)
        {
            laserBeam.SetActive(activate);
        }
        #endregion

        #region OnTrigger Events
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                PhotonView phView = other.gameObject.GetComponentInParent<PhotonView>();

                if (!phView.IsMine)
                {
                    return;
                }

                _OVRGrabbable.allowOffhandGrab = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("controllerLeft") || other.CompareTag("controllerRight"))
            {
                _OVRGrabbable.allowOffhandGrab = false;
            }
        }
        #endregion

        #region PUN OnPhotonSerializeView Method
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);                        // Laser tool
                stream.SendNext(transform.rotation);
                stream.SendNext(_LaserOn);                                  // Laser Beam
                stream.SendNext(laserBeam.transform.localScale);            // Update scale when hit / not hit events occur
                stream.SendNext(laserBeam.transform.position);
                stream.SendNext(laserHitPoint.activeSelf);                  // Laser Hit Point
                stream.SendNext(laserHitPoint.transform.position);          // Update laser hit point position
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.rotation = (Quaternion)stream.ReceiveNext();
                ActivateLaserBeam((bool)stream.ReceiveNext());
                laserBeam.transform.localScale = (Vector3)stream.ReceiveNext();
                laserBeam.transform.position = (Vector3)stream.ReceiveNext();
                laserHitPoint.SetActive((bool)stream.ReceiveNext());
                laserHitPoint.transform.position = (Vector3)stream.ReceiveNext();
            }
        }
        #endregion
    }
}