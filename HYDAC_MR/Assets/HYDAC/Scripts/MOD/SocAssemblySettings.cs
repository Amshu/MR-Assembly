using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    [CreateAssetMenu(menuName = "Socks/Assembly/Settings", fileName = "SOC_AssemblySettings")]
    public class SocAssemblySettings : ScriptableObject
    {
        [Range(0.1f, 5.0f)]
        [SerializeField] private float explodeTime;
        public float ExplodeTime => explodeTime;
    }
}