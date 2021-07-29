using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class SModuleInfo : ASInfo
    {
        [SerializeField] private GameObject prefab = null;
        
        protected override void ChangeFileName()
        {
            string newFileName = "MInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
        }
    }
}
