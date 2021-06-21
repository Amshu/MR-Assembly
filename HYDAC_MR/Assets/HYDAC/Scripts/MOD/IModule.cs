using System;

namespace HYDAC.Scripts.MOD
{
    public interface IModule
    {
        event Action<Module> OnFocused;

        void ToggleFocus(bool toggle);
        
        /// <summary>
        ///  This is for the part in focus 
        /// </summary>
        void Reset();
        
        void ToggleExplode();
        
        void ChangePosition(int step);
    }
}