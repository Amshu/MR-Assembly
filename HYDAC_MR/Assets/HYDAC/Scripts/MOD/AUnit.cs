using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class AUnit : MonoBehaviour
    {
        [SerializeField] private ASInfo info = null;
        public ASInfo Info => info;

        public void SetPartInfo(ASInfo _info)
        {
#if UNITY_EDITOR
            info = _info;
#endif
        }
    }
}
