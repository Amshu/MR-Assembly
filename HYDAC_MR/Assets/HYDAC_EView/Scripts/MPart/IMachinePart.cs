using UnityEngine;

namespace HYDAC_EView._Scripts.MPart
{
    public interface IMachinePart
    {
        int GetAssemblyPosition();
        string GetPartName();
        void Implode(float timeToDest);
        void Explode(float timeToDest);
        void HighlightPart(bool toggle, Color highlightColor);
    }
}
