using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Serialization;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for managing the player's interaction with the Keyboard and Colour Swatch game objects, allowing the user
    /// to enter in a string of characters (username) which, when entered, displays on the player's VR Headset avatar for other players
    /// to identify.  The colour swatches allow the player to change their headset and gloves colour for additional, easier visual recognition.
    /// </summary>
    public class PlayerIDPanelController : MonoBehaviourPun
    {
        #region Public and Private Attributes
        // Public Attributes
        [Header("Panels:")]
        public GameObject keyboardPanel;
        public GameObject viewScreenPanel;
        public GameObject colourPanel;
        [Tooltip("Avatar Game object, located on left-hand avatar game object in prefab")]
        public GameObject keyboardPanelAvatar;        // Synced over network
        [Header("Buttons:")]
        public GameObject[] characterKeys;
        public GameObject keyEnter;
        public GameObject keyBackspace;
        public GameObject keySpaceBar;
        public GameObject panelToggleButton;
        [Header("UI Text")]
        public Text screenInput;
        [Header("Audio:")]
        public AudioSource buttonClickAudio;
        [Header("VR Headset & Hands:")]
        public Text headsetID;
        public MeshRenderer vrHeadsetFront;
        public MeshRenderer vrHeadsetSide;
        public SkinnedMeshRenderer[] leftGlove;     // Contains mesh renderer for each left hand pose (eg. thumbs up, etc)
        public SkinnedMeshRenderer[] rightGlove;    // Contains mesh renderer for each right hand pose (eg. thumbs up, etc)
        public MeshRenderer vrHeadsetGlasses;
        [Header("Colour Swatches:")]
        [Tooltip("Store colours in left-to-right, top-to-bottom order")]
        public GameObject[] colourSwatches;
        public GameObject activeColourIcon;
        [Header("Materials:")]
        [Tooltip("Store colours in identical order to colour swatches")]
        public Material[] headsetAvatarColours;     // Stores headset material colours
        [Tooltip("Store colours in identical order to colour swatches")]
        public Material[] gloveAvatarColours;       // Stores glove material colours
        [Tooltip("Store colours in identical order to colour swatches")]
        public Material[] glassesAvatarColours;    // Stores glasses material colours
        public Material highlightedMaterial;
        public Material inactiveMaterial;
        public Material highlightedToggleMaterial;
        public Material inactiveToggleMaterial;
        [FormerlySerializedAs("_PlayerMgr")] public DEP_NetPlayerManager netPlayerMgr;

        // Private Attributes
        private Color[] _PlayerColours;          // To be retested - had issues using color array in RPC
        private WristBandController _WristBand;
        private StringBuilder _UsernameString;
        private bool _CanPressButton;
        

        // Encapsulated Read-only Property
        private bool _PanelOn;
        public bool PanelOn
        {
            get { return _PanelOn; }            // Used by WristBandController.cs to check panel status
        }
        #endregion

        private void Awake()
        {
            _WristBand = GetComponentInParent<WristBandController>();
            _UsernameString = new StringBuilder();
            _CanPressButton = true;
            _PanelOn = true;
            PopulateColours();
            TogglePanel();
        }

        #region Colour Methods
        /// <summary>
        /// Populates player colour array with designated colours, according to available colours on the colour swatch panel.
        /// </summary>
        private void PopulateColours()
        {
            const int TOTAL_COLOURS = 9;

            // Instantiate Colour Array
            _PlayerColours = new Color[TOTAL_COLOURS];

            // Populate array with colours
            _PlayerColours[0] = Color.red;
            _PlayerColours[1] = Color.yellow;
            _PlayerColours[2] = Color.green;
            _PlayerColours[3] = Color.cyan;
            _PlayerColours[4] = Color.blue;
            _PlayerColours[5] = new Color(1f, 0.17f, 0.71f);       // Pink
            _PlayerColours[6] = new Color(1f, 0.43f, 0f);          // Orange
            _PlayerColours[7] = Color.white;
            _PlayerColours[8] = new Color(0.11f, 0.08f, 0.39f);    // Purple
        }

        /// <summary>
        /// Changes screen input text colour according to colour selected from colour swatch
        /// </summary>
        /// <param name="colour"></param>
        private void ChangeInputColour(int colour)
        {
            screenInput.color = _PlayerColours[colour];
        }

        /// <summary>
        /// Selects appropriate colour swatch button according to input parameter (from user fingertip interaction) and triggers appropriate
        /// response according to button selected.
        /// </summary>
        /// <param name="selectedKey"></param>
        /// <param name="player"></param>
        /// <param name="hand"></param>
        public void SelectColourSwatch(string selectedKey)
        {
            GameObject activeButton = null;
            int colourIndex = 0;

            if (_CanPressButton)
            {
                // Reset the panel timer
                _WristBand.ResetTimer();

                for (int i = 0; i < colourSwatches.Length; i++)
                {
                    if (colourSwatches[i].name == selectedKey)
                    {
                        colourIndex = i;
                        activeButton = colourSwatches[i];
                        buttonClickAudio.Play();
                        break;
                    }
                }

                // Highlight currently selected swatch
                activeColourIcon.transform.SetParent(activeButton.transform);
                activeColourIcon.transform.localPosition = Vector3.zero;
                activeColourIcon.transform.localRotation = Quaternion.identity;

                // Change text colour and sync over network
                ChangeInputColour(colourIndex);
                photonView.RPC("ShowMyColour", RpcTarget.AllBuffered, colourIndex);

                // Delay button re-activation
                StartCoroutine("DelayReactivation");
            }
        }
        #endregion

        #region Keyboard Panel Methods
        /// <summary>
        /// Toggles Local Player's ID Panels On / Off and syncs panel state over network
        /// </summary>
        public void TogglePanel()
        {
            _PanelOn = !_PanelOn;

            // Activate / Deactivate ID Panels
            keyboardPanel.SetActive(_PanelOn);
            colourPanel.SetActive(_PanelOn);
            viewScreenPanel.SetActive(_PanelOn);

            // Disable other panels on wristband (if active)
            if (_PanelOn)
            {
                _WristBand.ShowPlayerIDPanel();
            }

            // Sync panel over network
            photonView.RPC("ShowKeyboardAvatar", RpcTarget.Others, _PanelOn);
        }

        /// <summary>
        /// Adds an alphabet character to the input string
        /// </summary>
        /// <param name="letter"></param>
        private void AddCharacter(string letter)
        {
            _UsernameString.Append(letter);
            UpdateScreenInput();
        }

        /// <summary>
        /// Removes last character from input string
        /// </summary>
        private void RemoveCharacter()
        {
            // Remove character at last element in string
            _UsernameString.Remove(_UsernameString.Length - 1, 1);
            UpdateScreenInput();
        }

        /// <summary>
        /// Updates input string displayed on Keyboard Screen
        /// </summary>
        private void UpdateScreenInput()
        {
            screenInput.text = _UsernameString.ToString();
        }

        /// <summary>
        /// Selects appropriate keypad button according to input parameter (from user fingertip interaction) and triggers appropriate
        /// response according to button selected.
        /// </summary>
        /// <param name="selectedKey"></param>
        /// <param name="player"></param>
        /// <param name="hand"></param>
        public void SelectKeyboardButton(string selectedKey)
        {
            GameObject activeButton;

            if (_CanPressButton)
            {
                // Reset the panel timer
                _WristBand.ResetTimer();

                if (selectedKey == keyEnter.name)
                {
                    activeButton = keyEnter;
                    headsetID.text = _UsernameString.ToString();

                    // sync text
                    photonView.RPC("ShowMyName", RpcTarget.OthersBuffered, headsetID.text);
                }
                else if (selectedKey == keyBackspace.name)
                {
                    activeButton = keyBackspace;
                    RemoveCharacter();
                }
                else if (selectedKey == keySpaceBar.name)
                {
                    activeButton = keySpaceBar;
                    AddCharacter(" ");
                }
                else if (selectedKey == panelToggleButton.name)
                {
                    activeButton = panelToggleButton;
                    TogglePanel();
                }
                else
                {
                    activeButton = GameObject.Find(selectedKey);        // Get alpabet character key  { is this causing issue???? }
                    AddCharacter(activeButton.name);
                }

                // Animate button activation
                StartCoroutine("ActivateButton", activeButton);
                StartCoroutine("DelayReactivation");
            }
        }
        #endregion

        #region PUN RPCs
        /// <summary>
        /// Syncs networked players' username on their headset avatar
        /// </summary>
        /// <param name="show"></param>
        [PunRPC]
        private void ShowMyName(string name)
        {
            // Toggle Avatar Tool Panel Object on / off
            headsetID.text = name;
        }

        /// <summary>
        /// Syncs networked players' colour on their headset avatar
        /// </summary>
        /// <param name="show"></param>
        [PunRPC]
        private void ShowMyColour(int myColour)
        {
            if (myColour == 0)
            {
                headsetID.color = Color.red;
            }
            else if (myColour == 1)
            {
                headsetID.color = Color.yellow;
            }
            else if (myColour == 2)
            {
                headsetID.color = Color.green;
            }
            else if (myColour == 3)
            {
                headsetID.color = Color.cyan;
            }
            else if (myColour == 4)
            {
                headsetID.color = Color.blue;
            }
            else if (myColour == 5)
            {
                headsetID.color = new Color(1f, 0.17f, 0.71f);      // Pink
            }
            else if (myColour == 6)
            {
                headsetID.color = new Color(1f, 0.43f, 0f);         // Orange
            }
            else if (myColour == 7)
            {
                headsetID.color = Color.white;
            }
            else if (myColour == 8)
            {
                headsetID.color = new Color(0.11f, 0.08f, 0.39f);   // Purple
            }

            // Change VR Headset Colour
            vrHeadsetFront.material = headsetAvatarColours[myColour];
            vrHeadsetSide.material = headsetAvatarColours[myColour];
            vrHeadsetGlasses.material = glassesAvatarColours[myColour];

            // Change Glove Colours
            for (int i = 0; i < leftGlove.Length; i++)
            {
                leftGlove[i].material = gloveAvatarColours[myColour];
                rightGlove[i].material = gloveAvatarColours[myColour];
            }
        }

        /// <summary>
        /// Toggles and syncs networked players' tool panel on / off
        /// </summary>
        /// <param name="show"></param>
        [PunRPC]
        private void ShowKeyboardAvatar(bool show)
        {
            // Toggle Avatar Tool Panel Object on / off
            keyboardPanelAvatar.SetActive(show);
        }
        #endregion

        #region Co-Routine Methods
        /// <summary>
        /// Animates the button by showing a highlight and performing a small translation for a brief duration, providing feedback for the player.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        IEnumerator ActivateButton(GameObject button)
        {
            const float ACTIVE_DURATION = 0.25f;
            const float BUTTON_PRESS_DISTANCE = 0.005f;        // was 0.0075f

            // Set button to active
            if (button == panelToggleButton)
            {
                button.GetComponent<MeshRenderer>().material = highlightedToggleMaterial;
                button.transform.Translate(Vector3.down * BUTTON_PRESS_DISTANCE);
            }
            else
            {
                button.GetComponent<MeshRenderer>().material = highlightedMaterial;
                button.transform.Translate(Vector3.up * BUTTON_PRESS_DISTANCE);
            }

            buttonClickAudio.Play();

            yield return new WaitForSeconds(ACTIVE_DURATION);

            // Reset button to inactive
            if (button == panelToggleButton)
            {
                button.GetComponent<MeshRenderer>().material = inactiveToggleMaterial;
                button.transform.Translate(Vector3.up * BUTTON_PRESS_DISTANCE);
            }
            else
            {
                button.GetComponent<MeshRenderer>().material = inactiveMaterial;
                button.transform.Translate(Vector3.down * BUTTON_PRESS_DISTANCE);
            }
        }

        /// <summary>
        /// A delay co-routine to prevent accidental re-activation "double-tapping" by moving the hand too quickly around the collider.
        /// </summary>
        /// <returns></returns>
        IEnumerator DelayReactivation()
        {
            const float DELAY_TIME = 0.5f;

            _CanPressButton = false;

            yield return new WaitForSeconds(DELAY_TIME);

            _CanPressButton = true;
        }
        #endregion
    }
}
