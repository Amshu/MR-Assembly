using UnityEngine;
using System.Collections;
using Photon.Pun;

/* TO DO: Add time delay for Prev / Next buttons */

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for allowing the user to select and spawn / destroy an interactive VR tool in the scene from the hand panel.
    /// When a tool is spawned, an RPC call is made to show / hide the local player's tool in networked players' instances.
    /// </summary>
    public class ToolPanelController : MonoBehaviourPun
    {
        #region Public and Private Attributes
        // Public Attributes
        [Header("Panel Buttons:")]
        public GameObject panelToggleButton;
        [Header("Audio Sources")]
        public AudioSource buttonClickAudio;
        [Header("UI Button Active / Inactive Materials:")]
        public Material highlightedMaterial;
        public Material highlightedToggleMaterial;
        public Material inactiveToggleMaterial;

        // Private Attributes
        private WristBandController _WristBand;
        //private DEP_NetPlayerManager _netPlayerMgr;
        private bool _CanPressButton;
        private bool _LaserIsOn;

        // Encapsulated Read-only Property
        private bool _PanelOn;
        public bool PanelOn
        {
            get { return _PanelOn; }            // Used by WristBandController.cs to check panel status
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            //_netPlayerMgr = GetComponentInParent<DEP_NetPlayerManager>();
            _WristBand = GetComponentInParent<WristBandController>();
            _CanPressButton = true;
            _PanelOn = true;
            TogglePanel();
        }
        #endregion

        #region Tool Selector Methods
        /// <summary>
        /// Toggles Local Player's Tool Panel Buttons On / Off and syncs panel state over network
        /// </summary>
        public void TogglePanel()
        {
            _PanelOn = !_PanelOn;

            // Disable other panels (if active)
            if (_PanelOn)
            {
                _WristBand.ShowToolPanel();
            }
        }

        /// <summary>
        /// Selects appropriate tool pad button according to input parameter (from user fingertip interaction) and triggers appropriate
        /// response according to button selected.
        /// </summary>
        /// <param name="selectedTool"></param>
        /// <param name="player"></param>
        /// <param name="hand"></param>
        public void SelectTool(string selectedTool, GameObject player, Transform hand)
        {
            GameObject activeButton = null;
            ToolSpawner toolSpawner = player.GetComponentInChildren<ToolSpawner>();

            if (_CanPressButton)
            {
                if (selectedTool == panelToggleButton.name)
                {
                    // Toggle Laser ON
                    _LaserIsOn = !_LaserIsOn;

                    if (_LaserIsOn)
                    {
                        toolSpawner.SelectTool(ToolBox.LASER_POINTER, hand);
                    }
                    else
                    {
                        toolSpawner.HideTools();
                    }
                    
                    activeButton = panelToggleButton;
                    TogglePanel();
                }

                StartCoroutine(ActivateButton(activeButton));
                StartCoroutine(DelayReactivation());
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
            if (button == panelToggleButton)
            {
                button.GetComponent<MeshRenderer>().material = highlightedToggleMaterial;
            }
            else
            {
                button.GetComponent<MeshRenderer>().material = highlightedMaterial;
            }

            button.transform.Translate(Vector3.left * BUTTON_PRESS_DISTANCE);
            buttonClickAudio.Play();

            yield return new WaitForSeconds(ACTIVE_DURATION);

            // Reset button to inactive
            if (button == panelToggleButton)
            {
                button.GetComponent<MeshRenderer>().material = inactiveToggleMaterial;
            }

            button.transform.Translate(Vector3.right * BUTTON_PRESS_DISTANCE);
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