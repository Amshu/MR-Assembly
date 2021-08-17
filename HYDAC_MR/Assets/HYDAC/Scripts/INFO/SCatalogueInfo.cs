using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.INFO
{
    
    public class SCatalogueInfo : ASInfo
    {
        public bool isLoadable;
        
        
        [SerializeField] private Sprite productImage;
        public Sprite ProductImage => productImage;

        [Space]
        [SerializeField] private string assemblyFolderKey;
        public string AssemblyFolderKey => assemblyFolderKey;

        [SerializeField] private AssetReference assemblyPrefab;
        public AssetReference AssemblyPrefab => assemblyPrefab;
        
        protected override void ChangeFileName()
        {
#if UNITY_EDITOR
            string newFileName = "CInfo_" + ID + "_" + iname;
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.GetInstanceID());
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newFileName);
#endif
        }
    }
}
