using HYDAC.Scripts.MAC;
using UnityEngine;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private MacUnit[] units;
        [SerializeField] private GameObject buttons;
        private IMacUnit _currentMacUnit;
        private IMacUnit[] _iMacUnits;

        private bool _inFocus;

        private void Awake()
        {
            GetAllAssemblies();
        }

        private void GetAllAssemblies()
        {
            _iMacUnits = new IMacUnit[units.Length];
            for(int i = 0; i < _iMacUnits.Length; i++)
            {
                _iMacUnits[i] = units[i] as IMacUnit;
                _iMacUnits[i].OnFocused += OnUnitFocused;
            }
        }

        private void OnUnitFocused(MacUnit targetMacUnit)
        {
            if (_inFocus) return;
            
            // When Focused
            // - Set inFocus to true
            // - Set current MacUnit
            // - Enable Explode UI
            // - ToggleFocus callback on all MacUnits
            _inFocus = true;
            _currentMacUnit = targetMacUnit;
            
            buttons.SetActive(true);
            
            Debug.Log("#MainManager#--------------Unit Focused");
            
            for (int i = 0; i < _iMacUnits.Length; i++)
            {
                IMacUnit macUnitOnList = _iMacUnits[i];

                if (!_currentMacUnit.Equals(macUnitOnList))
                    macUnitOnList.ToggleFocus(false);
            }
        }

        /// <summary>
        /// Event call from UI to reset 
        /// </summary>
        public void ExitFocus()
        {
            if (!_inFocus) return;

            Debug.Log("#MainManager#--------------Exit Focus");

            for (int i = 0; i < _iMacUnits.Length; i++)
            {
                IMacUnit macUnitOnList = _iMacUnits[i];

                macUnitOnList.Reset(false);
            }
            
            _inFocus = false;
            _currentMacUnit = null;
        }

        
        #region Assembly Interface Calls

        public void ToggleUnitExplode()
        {
            _currentMacUnit?.ToggleExplode();
        }

        public void ChangeUnitStepPosition(int position)
        {
            _currentMacUnit?.ChangeUnitPosition(position);
        }

        #endregion
    }
}
