using System;
using UnityEngine;

namespace HYDAC.Scripts.MAC
{
    public interface IAssembly
    {
        event Action<AssemblyManager> OnFocused;

        void ToggleFocus(bool toggle, Material fadeMaterial = null);

        void Reset();
    }
}