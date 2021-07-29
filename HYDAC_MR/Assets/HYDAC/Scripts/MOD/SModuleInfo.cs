using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    [CreateAssetMenu(menuName = "Assembly Infos/Assembly Module Info", fileName = "MInfo_X_Y")]
    public class SModuleInfo : ASInfo
    {
        protected override void ChangeFileName()
        {
            string newFileName = "MInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
        }
    }
}
