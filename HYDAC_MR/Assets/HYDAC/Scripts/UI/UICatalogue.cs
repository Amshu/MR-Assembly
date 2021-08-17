using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.UI
{
    public class UICatalogue : MonoBehaviour
    {
        [SerializeField] private string catalogueLabel;
        [Space] [SerializeField] private AssetReference catalogueButtonPrefab;
        [SerializeField] private Transform catalogueButtonParent;

        private List<ASInfo> _catalogue = new List<ASInfo>();

        private void Awake()
        {
            // Load catalogue
            StartCoroutine(LoadCatalogueFromRemote());
        }

        IEnumerator LoadCatalogueFromRemote()
        {
            //This will load the assets that match the given keys and populate the Result
            //with only objects that match both of the provided keys
            AsyncOperationHandle<IList<SCatalogueInfo>> intersectionWithMultipleKeys =
                Addressables.LoadAssetsAsync<SCatalogueInfo>("Catalogue",
                    AddToCatalogue);

            yield return intersectionWithMultipleKeys;

            //Use this only when the objects are no longer needed
            //Addressables.Release(intersectionWithMultipleKeys);
        }

        private void AddToCatalogue(SCatalogueInfo info)
        {
            Debug.Log("UICatalogue#------------Loaded info of: " + info.iname);

            // Add to list
            _catalogue.Add(info);

            // Create UI button and fill in info
            Addressables.InstantiateAsync(catalogueButtonPrefab, catalogueButtonParent).Completed += handle =>
            {
                handle.Result.GetComponentInChildren<UIBtnCatalogue>().Initialize(info, transform); 
            };
        }
    }
}