using UnityEngine;

namespace HYDAC_EView._Scripts
{
    [CreateAssetMenu(fileName = "MainSettings", menuName = "SOCKS/MainSettings", order = 0)]
    public class SocMainSettings : ScriptableObject
    {
        [Range(0.1f, 2.0f)]
        public float positionTimeChange = 5.0f;

        public Color previousAssemblyColor;
        public Color currentAssemblyColor;
        public Color nextAssemblyColor;
    }
}