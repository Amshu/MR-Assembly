using System;
using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public abstract class AModule: MonoBehaviour
    {
        protected abstract void OnReset();
        protected abstract void OnFocused();
        protected abstract void OnUnfocused();

        protected abstract void ResetScale();
        protected abstract void ResetPositionNRotation();

        protected abstract IEnumerator LerpVector3(Vector3 start, Vector3 end, float timeTaken,
            Action<Vector3> updateCall);
    }
}