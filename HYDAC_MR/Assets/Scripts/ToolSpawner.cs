using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for allowing the user to select and spawn / destroy an interactive VR tool in the scene.
    /// When a tool is spawned, an RPC call is made to show / hide the local player's tool in networked players' instances.
    /// </summary>
    public class ToolSpawner : MonoBehaviour
    {
        // Public Attributes
        [Tooltip("Parented to Right Hand")]
        public Transform toolSpawnPoint;
        [Tooltip("NOTE: Tools must be ordered according to the UI Tool Panel (Left to Right, Top To Bottom)")]
        public GameObject[] tools;
        [Header("Audio Sources")]
        public AudioSource spawnPop;

        // Private Attributes
        private int _Index;
        private PlayerMgrPUN _PlayerMGR;

        private void Awake()
        {
            _PlayerMGR = GetComponentInParent<PlayerMgrPUN>();
            _Index = 0;
        }

        #region Tool Related Methods
        /// <summary>
        /// Spawns (shows) selected tool at right hand vr position
        /// </summary>
        private void ShowTool(Transform VRHand)
        {
            // Show tool in local player's instance
            tools[_Index].transform.position = toolSpawnPoint.position;
            tools[_Index].transform.rotation = toolSpawnPoint.rotation;
            tools[_Index].SetActive(true);
            spawnPop.Play();

            if (tools[_Index].CompareTag("Networked Tool"))
            {
                // Show Local Player's tool in networked players' instances
                NetworkedToolSpawner nts = tools[_Index].GetComponent<NetworkedToolSpawner>();
                nts.DisplayTool(true);
            }
        }

        /// <summary>
        /// Hides all interactive tools in scene
        /// </summary>
        public void HideTools()
        {
            //if (!_PlayerMGR.CurrentlyGrabbingTool)
            //{
                // TO DO: Put in controls to stop attached bolts (children) from being hidden along with drill

                foreach (GameObject tool in tools)
                {
                    // Hide Local Player's tools
                    tool.SetActive(false);

                    if (tool.CompareTag("Networked Tool"))
                    {
                        // Hide Local Player's tools in networked players' instances
                        NetworkedToolSpawner nts = tool.GetComponent<NetworkedToolSpawner>();
                        nts.DisplayTool(false);
                    }
                }

                spawnPop.Play();
            //}
        }

        /// <summary>
        /// Allows selection of tool from Tool UI Panel and spawns tool near right hand.  A safety check is carried out to make sure the player is not 
        /// currently holding a tool (prevents OVR Grab bug). 
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="VRHand"></param>
        public void SelectTool(ToolBox tool, Transform VRHand)
        {
            //if (!_PlayerMGR.CurrentlyGrabbingTool)
            //{
                // Update index
                _Index = (int)tool;

                ShowTool(VRHand);
            //}
        }
        #endregion
    }
}