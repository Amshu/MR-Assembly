using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// using Photon.Voice.PUN;
// using Photon.Voice.Unity;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class primarily manages the local player's instance over the PUN network, sending the Transform data of the local player's VR hardware to other
    /// networked players and receiving their data in return.  Other players' Transform data is animated utilising a VR Avatar (head and hands), which allows each
    /// player to "see" other players in their instance in real-time.
    /// <br></br>
    /// <br></br>
    /// This class also handles the local player's "communication gestures", triggered by inputs on the controller or voice activation on the microphone, 
    /// </summary>
    public class PlayerMgrPun : MonoBehaviourPun, IPunObservable
    {
        #region Public and Private Attributes
        [Tooltip("The local player instance. Use this to know if local player is represented in the scene")]
        public static GameObject LocalPlayerInstance;

        // VR Avatar Elements
        [Header("Player Avatar (Displayed to other networked players):")]
        public GameObject headAvatar;
        public GameObject leftHandAvatar;
        public GameObject rightHandAvatar;
        public GameObject mapIcon;
        public GameObject speechOnBubble;
        public GameObject speechMutedBubble;
        private Transform _localVRHeadset;
        private Transform _localVRControllerLeft;
        private Transform _localVRControllerRight;

        // Hand Gestures
        [FormerlySerializedAs("poseNormalLH")] [Header("Avatar Hand Poses:")]
        public SkinnedMeshRenderer poseNormalLh;
        [FormerlySerializedAs("poseThumbUpLH")] public SkinnedMeshRenderer poseThumbUpLh;
        [FormerlySerializedAs("poseFingerPointLH")] public SkinnedMeshRenderer poseFingerPointLh;
        [FormerlySerializedAs("poseNormalRH")] public SkinnedMeshRenderer poseNormalRh;
        [FormerlySerializedAs("poseThumbUpRH")] public SkinnedMeshRenderer poseThumbUpRh;
        [FormerlySerializedAs("poseFingerPointRH")] public SkinnedMeshRenderer poseFingerPointRh;
        private bool _showNormalHandPoseLh;
        private bool _showThumbUpHandPoseLh;
        private bool _showFingerPointHandPoseLh;
        private bool _showNormalHandPoseRh;
        private bool _showThumbUpHandPoseRh;
        private bool _showFingerPointHandPoseRh;

        // Smoothing Variables For Remote Player's Motion
        [Header("Player Avatar Motion Smoothing:")]
        [Tooltip("0: no smoothing, > 0: increased smoothing \n(note: smoothing reduces positional accuracy and increases latency)")]
        [Range(0, 3)]
        public int smoothingFactor;     // Set to 2 as default (based on CUBE use-case tests)
        [Tooltip("Maximum distance (metres) for which to apply smoothing")]
        [Range(0, 3)]
        public float appliedDistance;   // Set to 1 as default (based on CUBE use-case tests)
        private Vector3 _correctPlayerHeadPosition = Vector3.zero;
        private Quaternion _correctPlayerHeadRotation = Quaternion.identity;
        //private Vector3 correctPlayerLeftHandPosition = Vector3.zero;
        //private Quaternion correctPlayerLeftHandRotation = Quaternion.identity;
        //private Vector3 correctPlayerRightHandPosition = Vector3.zero;
        //private Quaternion correctPlayerRightHandRotation = Quaternion.identity;

        // Oculus Elements
        [FormerlySerializedAs("OVRCameraHeadset")]
        [Header("Local Player's Oculus VR (MUST set to INACTIVE in prefab):")]
        [Tooltip("OVR Camera Rig")]
        public GameObject ovrCameraHeadset;
        [FormerlySerializedAs("OVRLefthandController")] [Tooltip("CustomHandLeft")]
        public GameObject ovrLefthandController;
        [FormerlySerializedAs("OVRRighthandController")] [Tooltip("CustomHandRight")]
        public GameObject ovrRighthandController;

        // Tool Elements
        [Header("Player Tools:")]
        //public ToolSpawner ToolSpawner;

        // Encapsulated property for Tool Selection
        private bool _currentlyGrabbingTool;                
        public bool CurrentlyGrabbingTool                   // Accessed by various Tool Controller scripts (to prevent new tool selection while currently holding a tool)
        {
            get { return _currentlyGrabbingTool; }          
            set { _currentlyGrabbingTool = value; }
        }

        // Voice Elements
        private int _currentAvailableLocalGroupNumber;
        private const byte RemoteGroup = 1;               // Listens to Remote group and ALL local groups (in list), transmits to Remote group
        private List<byte> _localGrouplist;                 // Listens to Remote group, transmits to own local group
        //private Recorder _recorderPun;
        //private Speaker _SpeakerPUN;
        private bool _isLocal;                              // False = Remote group member, True = Local group member (physicallly linked)
        private bool _voiceOn;
        public bool VoiceOn
        {
            get { return _voiceOn; }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            // Important:
            // used in RoomManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronised
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;

                // Enable Oculus Camera and controllers (for local player only)
                ovrCameraHeadset.SetActive(true);
                ovrLefthandController.SetActive(true);
                ovrRighthandController.SetActive(true);

                _localVRHeadset = GameObject.Find("CenterEyeAnchor").transform;                 // Get transform data from local VR Headset
                _localVRControllerLeft = GameObject.Find("CustomHandLeft").transform;
                _localVRControllerRight = GameObject.Find("CustomHandRight").transform;

                // Don't display our own "player" avatar to ourselves (except for map icon)
                headAvatar.SetActive(false);
                leftHandAvatar.SetActive(false);
                rightHandAvatar.SetActive(false);
                mapIcon.SetActive(true);
                //LeftHand.transform.SetParent(_OVRLefthand.transform);         // Activate if LeftHand.SetActive(true)
                //RightHand.transform.SetParent(_OVRRighthand.transform);       // Activate if RightHand.SetActive(true)

                // Voice Transmission (default state is ON, ALL players remote)
                _currentAvailableLocalGroupNumber = 2;          // First available group number after remote group (1)
                _localGrouplist = new List<byte>();             // to contain byte values > 1 per local group (up to 255 limit)
                //_recorderPun = GetComponent<Recorder>();

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
                //PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = RemoteGroup;

                ToggleVoice();
            }
        }

        // Update each frame
        private void Update()
        {
            if (photonView.IsMine)
            {
                mapIcon.transform.position = _localVRHeadset.position;
                mapIcon.transform.eulerAngles = new Vector3(0f, _localVRHeadset.eulerAngles.y + 180f, 0f);      // Only show y-axis rotation

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
                    if (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !_currentlyGrabbingTool)
                    {
                        //ToolSpawner.HideTools();
                    }
                }
            }
            else
            {
                // Show networked player's current hand pose
                // Left Hand
                poseNormalLh.enabled = _showNormalHandPoseLh;
                poseThumbUpLh.enabled = _showThumbUpHandPoseLh;
                poseFingerPointLh.enabled = _showFingerPointHandPoseLh;
                // Right Hand
                poseNormalRh.enabled = _showNormalHandPoseRh;
                poseThumbUpRh.enabled = _showThumbUpHandPoseRh;
                poseFingerPointRh.enabled = _showFingerPointHandPoseRh;

                // Smooth Remote player's motion on local machine
                SmoothPlayerMotion(ref headAvatar, ref _correctPlayerHeadPosition, ref _correctPlayerHeadRotation);
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
            _showNormalHandPoseLh = normal;
            _showThumbUpHandPoseLh = thumbsUp;
            _showFingerPointHandPoseLh = fingerPoint;
        }

        /// <summary>
        /// Updates player's right hand avatar pose according to boolean inputs.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="thumbsUp"></param>
        /// <param name="fingerPoint"></param>
        private void SetRightHandPose(bool normal, bool thumbsUp, bool fingerPoint)
        {
            _showNormalHandPoseRh = normal;
            _showThumbUpHandPoseRh = thumbsUp;
            _showFingerPointHandPoseRh = fingerPoint;
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
            _voiceOn = !_voiceOn;
            //_recorderPun.TransmitEnabled = _voiceOn;

            photonView.RPC("ShowMutedBubble", RpcTarget.Others, !_voiceOn);
        }

        /// <summary>
        /// Sets local player's voice interest group
        /// </summary>
        public void SetLocalPlayerGroup()
        {
            // Change remote status to local
            _isLocal = true;

            // Assign current available group number as my new local group
            byte myLocalGroup = (byte)_currentAvailableLocalGroupNumber;

            // Sync new group over network
            photonView.RPC("AssignNewLocalGroup", RpcTarget.AllBuffered, _currentAvailableLocalGroupNumber);

            // Re-subscribe to transmit to LOCAL group by default (not remote)
            // Note: we are still listening to remote group (and remote group will add our group to their listening groups)
            //_recorderPun.InterestGroup = myLocalGroup;
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
            _localGrouplist.Add((byte)groupNum);

            // Change next available group number
            _currentAvailableLocalGroupNumber = groupNum + 1;

            //// Add new local group as a listening group (for ALL REMOTE players only)
            if (!_isLocal)
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
                //PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, new byte[1] { (byte)groupNum });
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
                stream.SendNext(_localVRHeadset.position);
                stream.SendNext(_localVRHeadset.rotation);
                stream.SendNext(_localVRControllerLeft.position);
                stream.SendNext(_localVRControllerLeft.rotation);
                stream.SendNext(_localVRControllerRight.position);
                stream.SendNext(_localVRControllerRight.rotation);
                stream.SendNext(_showNormalHandPoseLh);
                stream.SendNext(_showThumbUpHandPoseLh);
                stream.SendNext(_showFingerPointHandPoseLh);
                stream.SendNext(_showNormalHandPoseRh);
                stream.SendNext(_showThumbUpHandPoseRh);
                stream.SendNext(_showFingerPointHandPoseRh);
                stream.SendNext(mapIcon.transform.position);
                stream.SendNext(mapIcon.transform.rotation);

                if (!_voiceOn)
                {
                    stream.SendNext(_voiceOn);                                  // Do not show "Speaker Bubble" icon when "Muted"
                }
                else
                {
                    //stream.SendNext(_recorderPun.VoiceDetector.Detected);      // Toggle "Speaker Bubble" on / off when speaking / quiet
                }
            }
            else if (stream.IsReading)
            {
                // Receive networked player's VR Headset position and rotation data
                _correctPlayerHeadPosition = (Vector3)stream.ReceiveNext();
                _correctPlayerHeadRotation = (Quaternion)stream.ReceiveNext();
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
                _showNormalHandPoseLh = (bool)stream.ReceiveNext();
                _showThumbUpHandPoseLh = (bool)stream.ReceiveNext();
                _showFingerPointHandPoseLh = (bool)stream.ReceiveNext();
                _showNormalHandPoseRh = (bool)stream.ReceiveNext();
                _showThumbUpHandPoseRh = (bool)stream.ReceiveNext();
                _showFingerPointHandPoseRh = (bool)stream.ReceiveNext();
                mapIcon.transform.position = (Vector3)stream.ReceiveNext();
                mapIcon.transform.rotation = (Quaternion)stream.ReceiveNext();
                speechOnBubble.SetActive((bool)stream.ReceiveNext());         // Show network players' "Speech Bubble" when they are talking
            }
        }
        #endregion
    }
}