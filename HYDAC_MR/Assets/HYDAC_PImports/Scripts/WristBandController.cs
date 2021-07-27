using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for toggling active wristband panels according to selection made by user.  In active panel automatically 
    /// shuts down after a designated period of time (20 seconds by default).
    /// </summary>
    public class WristBandController : MonoBehaviour
    {
        #region Public and Private Attributes
        // Public Attributes
        public ToolPanelController toolPanel;
        //public MapTeleportController mapPanel;
        public PlayerIDPanelController playerIDPanel;
        [Tooltip("Duration (sec) panel remains active when not in use")]
        public float panelTimer = 10f;     // default

        // Private Attributes
        private float _ElapsedTime;
        #endregion

        private void Update()
        {
            _ElapsedTime += Time.deltaTime;

            if (_ElapsedTime >= panelTimer)
            {
                ClosePanel();
            }
        }

        /// <summary>
        /// Closes the active panel
        /// </summary>
        private void ClosePanel()
        {
            //if (mapPanel.PanelOn)
            //{
            //    mapPanel.TogglePanel();
            //}
            if (toolPanel.PanelOn)
            {
                toolPanel.TogglePanel();
            }
            else if (playerIDPanel.PanelOn)
            {
                playerIDPanel.TogglePanel();
            }

            _ElapsedTime = 0f;
            enabled = false;
        }

        /// <summary>
        /// Resets timer and starts Update()
        /// </summary>
        private void StartTimer()
        {
            _ElapsedTime = 0f;
            enabled = true;
        }

        /// <summary>
        /// Disables Map and PlayerID panels (if on) when Tool Panel is selected by player.
        /// </summary>
        public void ShowToolPanel()
        {
            // Disable map and Keyboard
            //if (mapPanel.PanelOn)
            //{
            //    mapPanel.TogglePanel();
            //}
            if (playerIDPanel.PanelOn)
            {
                playerIDPanel.TogglePanel();
            }

            StartTimer();
        }

        /// <summary>
        /// Disables Tool and PlayerID panels (if on) when Map Panel is selected by player.
        /// </summary>
        public void ShowMapPanel()
        {
            // Disable tool and Keyboard
            if (toolPanel.PanelOn)
            {
                toolPanel.TogglePanel();
            }
            else if (playerIDPanel.PanelOn)
            {
                playerIDPanel.TogglePanel();
            }

            StartTimer();
        }

        /// <summary>
        /// Disables Map and Tool panels (if on) when PLayerID Panel is selected by player.
        /// </summary>
        public void ShowPlayerIDPanel()
        {
            // Disable tool and Map
            if (toolPanel.PanelOn)
            {
                toolPanel.TogglePanel();
            }
            //else if (mapPanel.PanelOn)
            //{
            //    mapPanel.TogglePanel();
            //}

            StartTimer();
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void ResetTimer()
        {
            _ElapsedTime = 0f;
        }
    }
}
