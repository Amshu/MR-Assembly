using UnityEngine;

namespace HYDAC_EView.Scripts.MPart
{
    public interface IMachinePart
    {
        void Initialize();
        int GetAssemblyPosition();
        string GetPartName();
        void Implode(float timeToDest);
        void Explode(float timeToDest);
        void HighlightPart(bool toggle, Material highlightMaterial);
    }
}
