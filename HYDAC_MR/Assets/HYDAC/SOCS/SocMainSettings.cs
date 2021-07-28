using UnityEngine;
using UnityEngine.Serialization;

namespace HYDAC.SOC.Settings
{
    [CreateAssetMenu(fileName = "MainSettings", menuName = "SOCKS/MainSettings", order = 0)]
    public class SocMainSettings : ScriptableObject
    {
        [Range(0.1f, 2.0f)]
        public float positionTimeChange = 5.0f;

        public Material previousUnitMaterial;
        public Material currentUnitMaterial;
        public Material nextUnitMaterial;
        public Material fadeAssemblyMaterial;
    }
}