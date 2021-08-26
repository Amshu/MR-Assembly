using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HYDAC.Scripts.ADD
{
    [CreateAssetMenu(menuName = "Socks/Settings/Addressables", fileName = "SOC_AddSettings")]
    class SocAddressablesSettings : ScriptableObject
    {
        [Tooltip("List of all the labels of main assets to be loaded at the start of the application")]
        [SerializeField] private string[] assetList;
        public string[] AssetsToLoadOnStart => assetList;

        [Tooltip("List of all the scenes in order they are required to be loaded")]
        [SerializeField] private AssetReference[] sceneList;
        public AssetReference[] SceneList => sceneList;

        [SerializeField] private AssetReference defaultEnvironment;
        public AssetReference DefaultEnvironment => defaultEnvironment;


        [Tooltip("Label of the Catalogue asset group")]
        [SerializeField] private string catalogueLabel;
        public string CatalogueLabel => catalogueLabel;

        [Space]
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] private AssetReference localPlayerPrefab;
        public AssetReference LocalPlayerPrefab => localPlayerPrefab;
    }
}
