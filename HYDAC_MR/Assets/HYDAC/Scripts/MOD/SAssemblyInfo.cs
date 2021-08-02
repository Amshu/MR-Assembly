using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class SAssemblyInfo : ASInfo
    {
        //public IAssemblyModule[] AssemblyModules => assemblyModules;
        protected override void ChangeFileName()
        {
#if UNITY_EDITOR
            string newFileName = "AInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
#endif
        }
    }
}
