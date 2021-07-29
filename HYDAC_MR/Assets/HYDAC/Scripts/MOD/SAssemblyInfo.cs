using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    [CreateAssetMenu(menuName = "Assembly Infos/Assembly Info", fileName = "AInfo_X_Y")]
    public class SAssemblyInfo : ASInfo
    {
        [SerializeField] private SModuleInfo[] assemblyModules;
        
        //public IAssemblyModule[] AssemblyModules => assemblyModules;
        public int NoOfAssemblyModules => assemblyModules.Length;
        
        protected override void ChangeFileName()
        {
            string newFileName = "AInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
        }
    }
}
