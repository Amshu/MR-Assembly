using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for managing the player's interaction with the Keyboard and Colour Swatch game objects, allowing the user
    /// to enter in a string of characters (username) which, when entered, displays on the player's VR Headset avatar for other players
    /// to identify.  The colour swatches allow the player to change their id colour for easier visual recognition.
    /// </summary>
    public class AvatarIDController : MonoBehaviourPun
    {
        #region Public and Private Attributes
        // Public Attributes
        [Header("Buttons:")]
        public GameObject keyA;
        public GameObject keyB;
        public GameObject keyC;
        public GameObject keyD;
        public GameObject keyE;
        public GameObject keyF;
        public GameObject keyG;
        public GameObject keyH;
        public GameObject keyI;
        public GameObject keyJ;
        public GameObject keyK;
        public GameObject keyL;
        public GameObject keyM;
        public GameObject keyN;
        public GameObject keyO;
        public GameObject keyP;
        public GameObject keyQ;
        public GameObject keyR;
        public GameObject keyS;
        public GameObject keyT;
        public GameObject keyU;
        public GameObject keyV;
        public GameObject keyW;
        public GameObject keyX;
        public GameObject keyY;
        public GameObject keyZ;
        public GameObject keyEnter;
        public GameObject keyBackspace;
        public GameObject keySpaceBar;
        [Header("UI Text")]
        public Text screenInput;
        [Header("Audio:")]
        public AudioSource buttonClickAudio;
        [Header("VR Headset:")]
        public Text headsetID;
        [Header("Colour Swatches:")]
        public GameObject redSwatch;
        public GameObject yellowSwatch;
        public GameObject greenSwatch;
        public GameObject cyanSwatch;
        public GameObject blueSwatch;
        public GameObject pinkSwatch;
        public GameObject orangeSwatch;
        public GameObject blackSwatch;
        public GameObject whiteSwatch;
        [Header("Materials:")]
        public Material highlightedMaterial;
        public Material inactiveMaterial;
        public GameObject activeColourSwatch;

        // Private Attributes
        //private const int TOTAL_COLOURS = 9;
        //private Color[] _ColourSwatches;          // To be retested - had issues using color array
        private bool _CanPressButton;
        private StringBuilder _UsernameString;
        private bool _IsUsernameSet;
        private bool _IsColourSet;
        #endregion

        private void Awake()
        {
            _CanPressButton = true;
            _UsernameString = new StringBuilder();
        }

        #region Keyboard Panel Methods
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
                if (selectedKey == keyEnter.name)
                {
                    activeButton = keyEnter;
                    headsetID.text = _UsernameString.ToString();

                    // Set Flag
                    _IsUsernameSet = true;

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
                else
                {
                    activeButton = GameObject.Find(selectedKey);        // Get alpabet character key  { is this causing issue???? }
                    AddCharacter(activeButton.name);
                }



                // Animate button activation
                StartCoroutine("ActivateButton", activeButton);
                StartCoroutine("DelayReactivation");

                //CheckDisablePanel();
            }
        }
        #endregion

        // TBC: Disable panel when 'ENTER' key and colour is selected.
        private void CheckDisablePanel()
        {
            if (_IsColourSet && _IsUsernameSet)
            {
                this.gameObject.SetActive(false);
            }
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
                if (selectedKey == redSwatch.name)
                {
                    activeButton = redSwatch;
                    colourIndex = 0;
                }
                else if (selectedKey == yellowSwatch.name)
                {
                    activeButton = yellowSwatch;
                    colourIndex = 1;
                }
                else if (selectedKey == greenSwatch.name)
                {
                    activeButton = greenSwatch;
                    colourIndex = 2;
                }
                else if (selectedKey == cyanSwatch.name)
                {
                    activeButton = cyanSwatch;
                    colourIndex = 3;
                }
                else if (selectedKey == blueSwatch.name)
                {
                    activeButton = blueSwatch;
                    colourIndex = 4;
                }
                else if (selectedKey == pinkSwatch.name)
                {
                    activeButton = pinkSwatch;
                    colourIndex = 5;
                }
                else if (selectedKey == orangeSwatch.name)
                {
                    activeButton = orangeSwatch;
                    colourIndex = 6;
                }
                else if (selectedKey == blackSwatch.name)
                {
                    activeButton = blackSwatch;
                    colourIndex = 7;
                }
                else if (selectedKey == whiteSwatch.name)
                {
                    activeButton = whiteSwatch;
                    colourIndex = 8;
                }

                // Set Flag
                _IsColourSet = true;

                // Highlight currently selected swatch
                activeColourSwatch.transform.SetParent(activeButton.transform);
                activeColourSwatch.transform.localPosition = Vector3.zero;
                activeColourSwatch.transform.localRotation = Quaternion.identity;

                // Change text colour and sync over network
                photonView.RPC("ShowMyColour", RpcTarget.AllBuffered, colourIndex);

                // Delay button re-activation
                StartCoroutine("DelayReactivation");

                //CheckDisablePanel();
            }
        }

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
                headsetID.color = Color.black;
            }
            else if (myColour == 8)
            {
                headsetID.color = Color.white;
            }
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
            const float BUTTON_PRESS_DISTANCE = 0.0075f;

            // Set button to active
            button.GetComponent<MeshRenderer>().material = highlightedMaterial;
            button.transform.Translate(Vector3.back * BUTTON_PRESS_DISTANCE);
            buttonClickAudio.Play();

            yield return new WaitForSeconds(ACTIVE_DURATION);

            // Reset button to inactive
            button.GetComponent<MeshRenderer>().material = inactiveMaterial;
            button.transform.Translate(Vector3.forward * BUTTON_PRESS_DISTANCE);
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
