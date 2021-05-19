using System;

namespace HYDAC.Scripts.MAC
{
    public interface IMacUnit
    {
        event Action<MacUnit> OnFocused;

        void ToggleFocus(bool toggle);
        
        /// <summary>
        ///  This is for the part in focus 
        /// </summary>
        void Reset(bool unfocusedUnit);
        
        void ToggleExplode();
        
        void ChangeUnitPosition(int step);
    }
}