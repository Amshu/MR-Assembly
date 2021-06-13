using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

namespace HYDAC.Scripts.PUN.Old
{
    /// <summary>
    /// This class primarily manages the local player's instance over the PUN network, sending the Transform data of the local player's VR hardware to other
    /// networked players and receiving their data in return.  Other players' Transform data is animated utilising a VR Avatar (head and hands), which allows each
    /// player to "see" other players in their instance in real-time.
    /// <br></br>
    /// <br></br>
    /// This class also handles the local player's "communication gestures", triggered by inputs on the controller or voice activation on the microphone, 
    /// </summary>
    public class PlayerMgrPUN1 : MonoBehaviourPun, IPunObservable
    {
        #region Public and Private Attributes
        [Tooltip("The local player instance. Use this to know if local player is represented in the scene")]
        public static GameObject localPlayerInstance;

        // VR Avatar Elements
        [Header("Player Avatar (Displayed to other networked players):")]
        public GameObject headAvatar;
        public GameObject leftHandAvatar;
        public GameObject rightHandAvatar;
        public GameObject mapIcon;
        public GameObject speechOnBubble;
        public GameObject speechMutedBubble;
        private Transform _LocalVRHeadset;
        private Transform _LocalVRControllerLeft;
        private Transform _LocalVRControllerRight;

        // Hand Gestures
        [Header("Avatar Hand Poses:")]
        public SkinnedMeshRenderer poseNormalLH;
        public SkinnedMeshRenderer poseThumbUpLH;
        public SkinnedMeshRenderer poseFingerPointLH;
        public SkinnedMeshRenderer poseNormalRH;
        public SkinnedMeshRenderer poseThumbUpRH;
        public SkinnedMeshRenderer poseFingerPointRH;
        private bool _ShowNormalHandPose_LH;
        private bool _ShowThumbUpHandPose_LH;
        private bool _ShowFingerPointHandPose_LH;
        private bool _ShowNormalHandPose_RH;
        private bool _ShowThumbUpHandPose_RH;
        private bool _ShowFingerPointHandPose_RH;

        // Smoothing Variables For Remote Player's Motion
        [Header("Player Avatar Motion Smoothing:")]
        [Tooltip("0: no smoothing, > 0: increased smoothing \n(note: smoothing reduces positional accuracy and increases latency)")]
        [Range(0, 3)]
        public int smoothingFactor;     // Set to 2 as default (based on CUBE use-case tests)
        [Tooltip("Maximum distance (metres) for which to apply smoothing")]
        [Range(0, 3)]
        public float appliedDistance;   // Set to 1 as default (based on CUBE use-case tests)
        private Vector3 _CorrectPlayerHeadPosition = Vector3.zero;
        private Quaternion _CorrectPlayerHeadRotation = Quaternion.identity;
        //private Vector3 correctPlayerLeftHandPosition = Vector3.zero;
        //private Quaternion correctPlayerLeftHandRotation = Quaternion.identity;
        //private Vector3 correctPlayerRightHandPosition = Vector3.zero;
        //private Quaternion correctPlayerRightHandRotation = Quaternion.identity;

        // Oculus Elements
        [Header("Local Player's Oculus VR (MUST set to INACTIVE in prefab):")]
        [Tooltip("OVR Camera Rig")]
        public GameObject OVRCameraHeadset;
        [Tooltip("CustomHandLeft")]
        public GameObject OVRLefthandController;
        [Tooltip("CustomHandRight")]
        public GameObject OVRRighthandController;

        // Tool Elements
        [Header("Player Tools:")]
        //public ToolSpawner toolSpawner;

        // Encapsulated property for Tool Selection
        private bool _CurrentlyGrabbingTool;                
        public bool CurrentlyGrabbingTool                   // Accessed by various Tool Controller scripts (to prevent new tool selection while currently holding a tool)
        {
            get { return _CurrentlyGrabbingTool; }          
            set { _CurrentlyGrabbingTool = value; }
        }

        // Voice Elements
        private int _CurrentAvailableLocalGroupNumber;
        private const byte _REMOTE_GROUP = 1;               // Listens to Remote group and ALL local groups (in list), transmits to Remote group
        private List<byte> _LocalGrouplist;                 // Listens to Remote group, transmits to own local group
        private Recorder _RecorderPUN;
        //private Speaker _SpeakerPUN;
        private bool _IsLocal;                              // False = Remote group member, True = Local group member (physicallly linked)
        private bool _VoiceOn;
        public bool VoiceOn
        {
            get { return _VoiceOn; }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // Important:
            // used in RoomManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronised
            if (photonView.IsMine)
            {
                localPlayerInstance = gameObject;

                // Enable Oculus Camera and controllers (for local player only)
                OVRCameraHeadset.SetActive(true);
                OVRLefthandController.SetActive(true);
                OVRRighthandController.SetActive(true);

                _LocalVRHeadset = GameObject.Find("CenterEyeAnchor").transform;                 // Get transform data from local VR Headset
                _LocalVRControllerLeft = GameObject.Find("CustomHandLeft").transform;
                _LocalVRControllerRight = GameObject.Find("CustomHandRight").transform;

                // Don't display our own "player" avatar to ourselves (except for map icon)
                headAvatar.SetActive(false);
                leftHandAvatar.SetActive(false);
                rightHandAvatar.SetActive(false);
                mapIcon.SetActive(true);
                //LeftHand.transform.SetParent(_OVRLefthand.transform);         // Activate if LeftHand.SetActive(true)
                //RightHand.transform.SetParent(_OVRRighthand.transform);       // Activate if RightHand.SetActive(true)

                // Voice Transmission (default state is ON, ALL players remote)
                _CurrentAvailableLocalGroupNumber = 2;          // First available group number after remote group (1)
                _LocalGrouplist = new List<byte>();             // to contain byte values > 1 per local group (up to 255 limit)
                _RecorderPUN = GetComponent<Recorder>();

                // Hand Gestures (default state)
                SetLeftHandPose(true, false, false);
                SetRightHandPose(true, false, false);
            }

            // Critical
            // Don't Destroy on load to prevent player from being destroyed when another player joins / leaves the room
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                // Subscribe to REMOTE group by default
                //_RecorderPUN.InterestGroup = _REMOTE_GROUP;                                                  // Transmit
                //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[1] { _REMOTE_GROUP });      // Listen
                PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = _REMOTE_GROUP;

                ToggleVoice();
            }
        }

        // Update each frame
        private void Update()
        {
            if (photonView.IsMine)
            {
                mapIcon.transform.position = _LocalVRHeadset.position;
                mapIcon.transform.eulerAngles = new Vector3(0f, _LocalVRHeadset.eulerAngles.y + 180f, 0f);      // Only show y-axis rotation

                // AUDIO GROUPS: 
                // Allow user to set local group
                // Sets next available group.
                // Remote group players add that group to their listen list.

                // Record LEFT-HAND Pose
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))         // Side Grip Button
                {
                    if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))    // Front Trigger Button
                    {
                        // Show 'THUMBS UP'
                        SetLeftHandPose(false, true, false);
                    }
                    else
                    {
                        // Show 'FINGER POINT'
                        SetLeftHandPose(false, false, true);
                    }
                }
                else
                {
                    // Show 'NORMAL POSE'
                    SetLeftHandPose(true, false, false);
                }

                // Record RIGHT-HAND Pose
                if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))         // Side Grip Button
                {
                    if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))    // Front Trigger Button
                    {
                        // Show 'THUMBS UP'
                        SetRightHandPose(false, true, false);
                    }
                    else
                    {
                        // Show 'FINGER POINT'
                        SetRightHandPose(false, false, true);
                    }
                }
                else
                {
                    // Show 'NORMAL POSE'
                    SetRightHandPose(true, false, false);
                }

                // Hide Active Tool (if not grabbed) by pressing RH thumbstick up or down [short-hand alternative to using Tool UI Panel]
                if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown) || (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp)))
                {
                    if (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !_CurrentlyGrabbingTool)
                    {
                        //toolSpawner.HideTools();
                    }
                }
            }
            else
            {
                // Show networked player's current hand pose
                // Left Hand
                poseNormalLH.enabled = _ShowNormalHandPose_LH;
                poseThumbUpLH.enabled = _ShowThumbUpHandPose_LH;
                poseFingerPointLH.enabled = _ShowFingerPointHandPose_LH;
                // Right Hand
                poseNormalRH.enabled = _ShowNormalHandPose_RH;
                poseThumbUpRH.enabled = _ShowThumbUpHandPose_RH;
                poseFingerPointRH.enabled = _ShowFingerPointHandPose_RH;

                // Smooth Remote player's motion on local machine
                SmoothPlayerMotion(ref headAvatar, ref _CorrectPlayerHeadPosition, ref _CorrectPlayerHeadRotation);
                //SmoothPlayerMotion(ref LeftHand, ref correctPlayerLeftHandPosition, ref correctPlayerLeftHandRotation);
                //SmoothPlayerMotion(ref RightHand, ref correctPlayerRightHandPosition, ref correctPlayerRightHandRotation);
            }
        }
        #endregion

        #region Avatar Related Methods
        /// <summary>
        /// Updates player's left hand avatar pose according to boolean inputs.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="thumbsUp"></param>
        /// <param name="fingerPoint"></param>
        private void SetLeftHandPose(bool normal, bool thumbsUp, bool fingerPoint)
        {
            _ShowNormalHandPose_LH = normal;
            _ShowThumbUpHandPose_LH = thumbsUp;
            _ShowFingerPointHandPose_LH = fingerPoint;
        }

        /// <summary>
        /// Updates player's right hand avatar pose according to boolean inputs.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="thumbsUp"></param>
        /// <param name="fingerPoint"></param>
        private void SetRightHandPose(bool normal, bool thumbsUp, bool fingerPoint)
        {
            _ShowNormalHandPose_RH = normal;
            _ShowThumbUpHandPose_RH = thumbsUp;
            _ShowFingerPointHandPose_RH = fingerPoint;
        }

        /// <summary>
        /// Applies LERP interpolation to smooth the remote player's game object motion over the network. 
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="gameObjectCorrectTransformPosition"></param>
        /// <param name="gameObjectCorrectTransformRotation"></param>
        private void SmoothPlayerMotion(ref GameObject gameObject, ref Vector3 gameObjectCorrectTransformPosition, ref Quaternion gameObjectCorrectTransformRotation)
        {
            // Smoothing variables
            float distance = Vector3.Distance(gameObject.transform.position, gameObjectCorrectTransformPosition);

            if (distance < appliedDistance)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObjectCorrectTransformPosition, Time.deltaTime * smoothingFactor);
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, gameObjectCorrectTransformRotation, Time.deltaTime * smoothingFactor);
            }
            else
            {
                gameObject.transform.position = gameObjectCorrectTransformPosition;
                gameObject.transform.rotation = gameObjectCorrectTransformRotation;
            }
        }
        #endregion

        #region Photon Voice Methods
        /// <summary>
        /// Toggles a player's Voice Transmission On / Off
        /// </summary>
        public void ToggleVoice()
        {
            _VoiceOn = !_VoiceOn;
            _RecorderPUN.TransmitEnabled = _VoiceOn;

            photonView.RPC("ShowMutedBubble", RpcTarget.Others, !_VoiceOn);
        }

        /// <summary>
        /// Sets local player's voice interest group
        /// </summary>
        public void SetLocalPlayerGroup()
        {
            // Change remote status to local
            _IsLocal = true;

            // Assign current available group number as my new local group
            byte myLocalGroup = (byte)_CurrentAvailableLocalGroupNumber;

            // Sync new group over network
            photonView.RPC("AssignNewLocalGroup", RpcTarget.AllBuffered, _CurrentAvailableLocalGroupNumber);

            // Re-subscribe to transmit to LOCAL group by default (not remote)
            // Note: we are still listening to remote group (and remote group will add our group to their listening groups)
            _RecorderPUN.InterestGroup = myLocalGroup;
        }
        #endregion

        #region PUN RPCs and Serialize View Method
        /// <summary>
        /// Assigns local player to a local group (for players sharing same physical space)
        /// </summary>
        [PunRPC]
        private void AssignNewLocalGroup(int groupNum)
        {
            // Add assigned group number to list of local groups
            _LocalGrouplist.Add((byte)groupNum);

            // Change next available group number
            _CurrentAvailableLocalGroupNumber = groupNum + 1;

            //// Add new local group as a listening group (for ALL REMOTE players only)
            if (!_IsLocal)
            {
                /* The following code may not be needed:
                 * additional interest groups can simply be added, according to Photon Docs...need to test to be sure
                 
                 * This code block creates a new array, adding the new local group to the list - may not be required
                    
                    // Create temporay byte array for group storage
                    byte[] interestGroups = new byte[_LocalGrouplist.Count + 1];        // +1 to also incorporate remote group

                    // Add remote group to interest groups array
                    interestGroups[0] = _RemoteGroup;       

                    // Add all stored local groups to interest groups array
                    for (int i = 0; i < _LocalGrouplist.Count; i++)
                    {
                        interestGroups[i + 1] = _LocalGrouplist[i];
                    }

                    // Update remote player's subscription to all new interest groups
                    PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, interestGroups);
                  */

                // Add new local group to interest groups
                PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[1] { (byte)groupNum });
            }
        }

        [PunRPC]
        private void ShowMutedBubble(bool show)
        {
            speechMutedBubble.SetActive(show);
        }

        /// <summary>
        /// Controls the exchange of data between local and remote player's VR data
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send local VR Headset position and rotation data to networked player
                stream.SendNext(_LocalVRHeadset.position);
                stream.SendNext(_LocalVRHeadset.rotation);
                stream.SendNext(_LocalVRControllerLeft.position);
                stream.SendNext(_LocalVRControllerLeft.rotation);
                stream.SendNext(_LocalVRControllerRight.position);
                stream.SendNext(_LocalVRControllerRight.rotation);
                stream.SendNext(_ShowNormalHandPose_LH);
                stream.SendNext(_ShowThumbUpHandPose_LH);
                stream.SendNext(_ShowFingerPointHandPose_LH);
                stream.SendNext(_ShowNormalHandPose_RH);
                stream.SendNext(_ShowThumbUpHandPose_RH);
                stream.SendNext(_ShowFingerPointHandPose_RH);
                stream.SendNext(mapIcon.transform.position);
                stream.SendNext(mapIcon.transform.rotation);

                if (!_VoiceOn)
                {
                    stream.SendNext(_VoiceOn);                                  // Do not show "Speaker Bubble" icon when "Muted"
                }
                else
                {
                    stream.SendNext(_RecorderPUN.VoiceDetector.Detected);      // Toggle "Speaker Bubble" on / off when speaking / quiet
                }
            }
            else if (stream.IsReading)
            {
                // Receive networked player's VR Headset position and rotation data
                _CorrectPlayerHeadPosition = (Vector3)stream.ReceiveNext();
                _CorrectPlayerHeadRotation = (Quaternion)stream.ReceiveNext();
                //correctPlayerLeftHandPosition = (Vector3)stream.ReceiveNext();
                //correctPlayerLeftHandRotation = (Quaternion)stream.ReceiveNext();
                //correctPlayerRightHandPosition = (Vector3)stream.ReceiveNext();
                //correctPlayerRightHandRotation = (Quaternion)stream.ReceiveNext();
                //Head.transform.position = (Vector3)stream.ReceiveNext();
                //Head.transform.rotation = (Quaternion)stream.ReceiveNext();
                leftHandAvatar.transform.position = (Vector3)stream.ReceiveNext();
                leftHandAvatar.transform.rotation = (Quaternion)stream.ReceiveNext();
                rightHandAvatar.transform.position = (Vector3)stream.ReceiveNext();
                rightHandAvatar.transform.rotation = (Quaternion)stream.ReceiveNext();
                _ShowNormalHandPose_LH = (bool)stream.ReceiveNext();
                _ShowThumbUpHandPose_LH = (bool)stream.ReceiveNext();
                _ShowFingerPointHandPose_LH = (bool)stream.ReceiveNext();
                _ShowNormalHandPose_RH = (bool)stream.ReceiveNext();
                _ShowThumbUpHandPose_RH = (bool)stream.ReceiveNext();
                _ShowFingerPointHandPose_RH = (bool)stream.ReceiveNext();
                mapIcon.transform.position = (Vector3)stream.ReceiveNext();
                mapIcon.transform.rotation = (Quaternion)stream.ReceiveNext();
                speechOnBubble.SetActive((bool)stream.ReceiveNext());         // Show network players' "Speech Bubble" when they are talking
            }
        }
        #endregion
    }
}