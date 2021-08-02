using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace HYDAC.Scripts.MOD
{
    public class SModuleInfo : ASInfo
    {
        public GameObject prefab = null;
        [SerializeField] private Image image = null;
        [SerializeField] private VideoClip video = null;
        
        
        protected override void ChangeFileName()
        {
#if UNITY_EDITOR
            string newFileName = "MInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
#endif
        }
    }
}
