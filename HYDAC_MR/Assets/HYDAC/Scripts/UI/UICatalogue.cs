using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.SOCS;

namespace HYDAC.Scripts.UI
{
    public class UICatalogue : MonoBehaviour
    {
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        
        [Space] [SerializeField] private AssetReference catalogueButtonPrefab;
        [SerializeField] private Transform catalogueButtonParent;

        private bool _isInitialised;
        private Transform[] _buttonTransforms;

        private void OnEnable()
        {
            if (_isInitialised && assemblyEvents.Catalogue.Length < 1)
            {
                Debug.LogError("#UICatalogue#---------Catalogue fetch returned empty or Catalogue already initialised");
                return;
            }
                
            // Load catalogue
            StartCoroutine(InstantiateCatalogueUI(assemblyEvents.Catalogue));
        }

        private void OnDisable()
        {
            for (int i = 0; i < _buttonTransforms.Length; i++)
            {
                bool check = Addressables.ReleaseInstance(_buttonTransforms[i].gameObject);
                if (!check)
                {
                    Debug.LogError("#UICatalogue#---------Releasing button failed: " + _buttonTransforms[i].name);
                }
            }

            _buttonTransforms = null;
        }

        IEnumerator InstantiateCatalogueUI(SCatalogueInfo[] catalogue)
        {
            List<Transform> buttonTransforms = new List<Transform>();
            
            for (int i = 0; i < catalogue.Length; i++)
            {
                // Create UI button and fill in info
                AsyncOperationHandle<GameObject> buttonLoadingHandle =
                    Addressables.InstantiateAsync(catalogueButtonPrefab, catalogueButtonParent);

                buttonLoadingHandle.Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        buttonTransforms.Add(handle.Result.transform);
                        buttonLoadingHandle.Result.GetComponentInChildren<UIBtnCatalogue>().Initialize(catalogue[i], transform);
                    }
                    else
                    {
                        Debug.LogError("#UICatalogue#---------Instantiating button failed");
                    }
                };
                
                yield return new WaitUntil(() => buttonLoadingHandle.Task.IsCompleted);
            }

            _buttonTransforms = buttonTransforms.ToArray();
        }
    }
}