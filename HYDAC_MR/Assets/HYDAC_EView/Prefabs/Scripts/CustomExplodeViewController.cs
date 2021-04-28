using System.Collections.Generic;
using UnityEngine;

namespace HYDAC_EView.Scripts
{
    public class ExplodeViewController : MonoBehaviour
    {
        public delegate void ExplodeViewControllerDelegate();

        [SerializeField] private float speed = 0.1f;
        [SerializeField] private List<GameObject> defaultPositions = default;
        [SerializeField] private List<GameObject> explodedPositions = default;

        private readonly List<Vector3> explodedPos = new List<Vector3>();
        private readonly List<Vector3> startingPos = new List<Vector3>();

        private bool isInDefaultPosition;
        private int currentAssemblyPosition;

        private void Start()
        {
            // Cache references
            foreach (var item in defaultPositions) startingPos.Add(item.transform.localPosition);
            foreach (var item in explodedPositions) explodedPos.Add(item.transform.localPosition);
        }
        
        
        private void UpdatePositions()
        {
            // Reverse position based on the current position state
            if (isInDefaultPosition)
                // Move objects to exploded positions
                for (var i = 0; i < defaultPositions.Count; i++)
                    defaultPositions[i].transform.localPosition = Vector3.Lerp(
                        defaultPositions[i].transform.localPosition,
                        explodedPos[i], speed);
            else
                // Move objects to default positions
                for (var i = 0; i < defaultPositions.Count; i++)
                    defaultPositions[i].transform.localPosition = Vector3.Lerp(
                        defaultPositions[i].transform.localPosition,
                        startingPos[i], speed);
        }


        /// <summary>
        /// Updates assembly position 
        /// </summary>
        public void ChangeAssemblyPosition(float value)
        {
            currentAssemblyPosition = (int) value;
            
        }
        
        /// <summary>
        ///     Triggers the exploded view feature.
        ///     Hooked up in Unity.
        /// </summary>
        public void ToggleExplodedView()
        {
            // if (isPunEnabled)
            //     OnToggleExplodedView?.Invoke();
            // else
                Toggle();
        }

        /// <summary>
        ///     Toggles the exploded view.
        /// </summary>
        public void Toggle()
        {
            // Toggle the current position state
            isInDefaultPosition = !isInDefaultPosition;
            UpdatePositions();
        }

        /// <summary>
        ///     Raised when ToggleExplodedView is called and PUN is enabled.
        /// </summary>
        public event ExplodeViewControllerDelegate OnToggleExplodedView;
    }
}
