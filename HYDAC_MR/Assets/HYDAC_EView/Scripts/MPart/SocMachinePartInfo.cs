using UnityEngine;

namespace HYDAC_EView._Scripts.MPart
{
    /// <summary>
    /// <c>SocMachinePartInfo</c> is a scriptable object class that contains all the main details
    /// of a given machine part such as:
    /// <c>partName</c><value>This is the name of the part in, partName.</value>
    /// </summary>
    [CreateAssetMenu(fileName = "00_INFO_PartName", menuName = "SOCKS/Info", order = 1)]
    public class SocMachinePartInfo : ScriptableObject
    {
        public string partName = "";
        public int assemblyPosition = 0;

        [TextArea]
        public string partInfo = "";

        public void PrintInfo()
        {
            Debug.LogFormat("#SOC_MachinePartInfo#-------------------------{0}{1}\nPartInfo: {2}", 
                assemblyPosition, partName , partInfo);
        }
    }
}