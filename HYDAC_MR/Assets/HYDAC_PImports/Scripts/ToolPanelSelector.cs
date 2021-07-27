using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for allowing the user to select and spawn / destroy an interactive VR tool in the scene from the hand panel.
    /// When a tool is spawned, an RPC call is made to show / hide the local player's tool in networked players' instances.
    /// </summary>
    public class ToolPanelSelector : MonoBehaviourPun
    {
        #region Public and Private Attributes
        // Public Attributes
        public GameObject voiceButton;
        public GameObject laserButton;
        public GameObject closeToolButton;
        public GameObject panelToggleButton;
        public GameObject avatarToolPanel;              // Synced over network
        [Header("Audio Sources")]
        public AudioSource buttonClickAudio;
        [Header("UI Button Active / Inactive Materials:")]
        public Material highlightedMaterial;
        public Material micOnInactiveMaterial;
        public Material micOffInactiveMaterial;
        public Material highlightedToggleMaterial;
        public Material inactiveToggleMaterial;

        // Private Attributes
        private WristBandController _WristBand;
        private PlayerMgrPUN _PlayerMgr;
        private bool _CanPressButton;

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
            _PlayerMgr = GetComponentInParent<PlayerMgrPUN>();
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
            
            voiceButton.SetActive(_PanelOn);
            laserButton.SetActive(_PanelOn);
            closeToolButton.SetActive(_PanelOn);

            // Disable other panels (if active)
            if (_PanelOn)
            {
                _WristBand.ShowToolPanel();
            }

            photonView.RPC("ShowToolPanel", RpcTarget.Others, _PanelOn);
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
                if (selectedTool == laserButton.name)
                {
                    activeButton = laserButton;
                    toolSpawner.SelectTool(ToolBox.LASER_POINTER, hand);
                }
                else if (selectedTool == panelToggleButton.name)
                {
                    activeButton = panelToggleButton;
                    TogglePanel();
                }
                else if (selectedTool == closeToolButton.name)
                {
                    activeButton = closeToolButton;
                    toolSpawner.HideTools();
                }

                StartCoroutine("ActivateButton", activeButton);
                StartCoroutine("DelayReactivation");
            }
        }
        #endregion

        #region PUN RPCs
        /// <summary>
        /// Toggles and syncs networked players' tool panel on / off
        /// </summary>
        /// <param name="show"></param>
        [PunRPC]
        private void ShowToolPanel(bool show)
        {
            // Toggle Avatar Tool Panel Object on / off
            avatarToolPanel.SetActive(show);
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
            
            button.transform.Translate(Vector3.down * BUTTON_PRESS_DISTANCE);
            buttonClickAudio.Play();

            yield return new WaitForSeconds(ACTIVE_DURATION);

            // Reset button to inactive
            if (button == panelToggleButton)
            {
                button.GetComponent<MeshRenderer>().material = inactiveToggleMaterial;
            }
            
            button.transform.Translate(Vector3.up * BUTTON_PRESS_DISTANCE);
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