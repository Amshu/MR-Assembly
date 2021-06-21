using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public interface ISubModule
    {
        void Initialize();
        int GetUnitPosition();
        string GetPartName();
        void ToggleExplode(bool toggle, float timeToDest);
        
        void ChangeMaterial(bool toggle, Material highlightMaterial);
    }
}
