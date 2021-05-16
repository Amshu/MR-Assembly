using UnityEngine;

namespace MAC
{
    public interface IMacUnitPart
    {
        void Initialize();
        int GetUnitPosition();
        string GetPartName();
        void ToggleExplode(bool toggle, float timeToDest);
        
        void ChangeMaterial(bool toggle, Material highlightMaterial);
    }
}
