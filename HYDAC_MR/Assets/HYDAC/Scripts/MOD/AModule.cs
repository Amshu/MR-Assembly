using System;
using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public abstract class AModule: AUnit
    {
        protected abstract void OnReset();
        protected abstract void OnFocused();
        protected abstract void OnUnfocused();
    }
}