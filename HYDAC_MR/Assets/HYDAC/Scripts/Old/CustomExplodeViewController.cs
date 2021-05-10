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

        private readonly List<Vector3> _explodedPos = new List<Vector3>();
        private readonly List<Vector3> _startingPos = new List<Vector3>();

        private bool _isInDefaultPosition;
        private int _currentAssemblyPosition;

        private void Start()
        {
            // Cache references
            foreach (var item in defaultPositions) _startingPos.Add(item.transform.localPosition);
            foreach (var item in explodedPositions) _explodedPos.Add(item.transform.localPosition);
        }
        
        
        private void UpdatePositions()
        {
            // Reverse position based on the current position state
            if (_isInDefaultPosition)
                // Move objects to exploded positions
                for (var i = 0; i < defaultPositions.Count; i++)
                    defaultPositions[i].transform.localPosition = Vector3.Lerp(
                        defaultPositions[i].transform.localPosition,
                        _explodedPos[i], speed);
            else
                // Move objects to default positions
                for (var i = 0; i < defaultPositions.Count; i++)
                    defaultPositions[i].transform.localPosition = Vector3.Lerp(
                        defaultPositions[i].transform.localPosition,
                        _startingPos[i], speed);
        }


        /// <summary>
        /// Updates assembly position 
        /// </summary>
        public void ChangeAssemblyPosition(float value)
        {
            _currentAssemblyPosition = (int) value;
            
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
            _isInDefaultPosition = !_isInDefaultPosition;
            UpdatePositions();
        }

        /// <summary>
        ///     Raised when ToggleExplodedView is called and PUN is enabled.
        /// </summary>
        public event ExplodeViewControllerDelegate OnToggleExplodedView;
    }
}
