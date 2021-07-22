using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.PUN
{
    /// <summary>
    /// This class is responsible for allowing a fingertip game object to act as a collider, triggering an interactive event with various UI elements.
    /// </summary>
    public class FingertipInteraction : MonoBehaviour
    {
        // Public Attributes
        public GameObject player;

        // Private Attributes
        private bool _WasHit;


        #region OnTrigger Event
        // Create OnTrigger Events to interact with UI Panels
        private void OnTriggerEnter(Collider other)
        {
            // Determine appropriate response according to the colliding trigger
            switch (other.tag)
            {
                case "Keyboard Button":
                    if (!_WasHit)
                    {
                        other.gameObject.GetComponentInParent<PlayerIDPanelController>().SelectKeyboardButton(other.gameObject.name);
                        _WasHit = true;
                    }
                    break;
                case "Colour Swatch":
                    if (!_WasHit)
                    {
                        other.gameObject.GetComponentInParent<PlayerIDPanelController>().SelectColourSwatch(other.gameObject.name);
                        _WasHit = true;
                    }
                    break;
                case "Tool Button":
                    if (!_WasHit)
                    {
                        other.gameObject.GetComponentInParent<ToolPanelController>().SelectTool(other.gameObject.name, player, gameObject.transform);
                        _WasHit = true;
                    }
                    break;
            }

            if (_WasHit)
            {
                StartCoroutine("DelayNextHit");
            }
        }
        #endregion

        #region Co-Routine Method
        IEnumerator DelayNextHit()
        {
            const float DELAY = 0.5f;

            yield return new WaitForSeconds(DELAY);

            _WasHit = false;
        }
        #endregion
    }
}