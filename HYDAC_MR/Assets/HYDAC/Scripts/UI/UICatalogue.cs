using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

using HYDAC.Scripts.INFO;
using HYDAC.Scripts.MOD;

namespace HYDAC.Scripts.UI
{
    public class UICatalogue : MonoBehaviour
    {
        [SerializeField] private string catalogueLabel;

        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private SocAssemblyUI assemblyUI;

        [Space] [SerializeField] private AssetReference catalogueButtonPrefab;
        [SerializeField] private Transform catalogueButtonParent;

        private Transform[] _buttonTransforms;


        private void Awake()
        {
            // Load catalogue
            InstantiateCatalogueUI();
        }

        private void OnEnable()
        {
            assemblyUI.EToggleCatalogueUI += OnToggleUI;
        }

        private void OnDisable()
        {
            assemblyUI.EToggleCatalogueUI -= OnToggleUI;

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

        private void OnToggleUI(bool toggle)
        {
            //Debug.Log("Test");

            if (toggle)
            {
                transform.GetChild(0).gameObject.SetActive(true);

                
            }
            else
                transform.GetChild(0).gameObject.SetActive(false);
        }

        private async Task InstantiateCatalogueUI()
        {
            // Load Catalogue
            List<SCatalogueInfo> catalogue = new List<SCatalogueInfo>();

            await Addressables.LoadAssetsAsync<SCatalogueInfo>(catalogueLabel, (result) =>
            {
                Debug.Log("#UICatalogue#-------------Catalogue found: " + result.iname);
                catalogue.Add(result);
            }).Task;

            assemblyEvents.SetCatalogue(catalogue.ToArray());

            List<Transform> buttonTransforms = new List<Transform>();

            for (int i = 0; i < catalogue.Count; i++)
            {
                // Create UI button and fill in info
                var button = await Addressables.InstantiateAsync(catalogueButtonPrefab, catalogueButtonParent).Task;

                buttonTransforms.Add(button.transform);
                button.GetComponentInChildren<UIBtnCatalogue>().Initialize(catalogue[i], transform);
            }

            _buttonTransforms = buttonTransforms.ToArray();
        }
    }
}