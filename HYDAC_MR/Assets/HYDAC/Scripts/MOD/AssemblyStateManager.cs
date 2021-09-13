using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

using Photon.Pun;

using HYDAC.Scripts.ADD;
using HYDAC.Scripts.INFO;


namespace HYDAC.Scripts.MOD
{
    public class AssemblyStateManager : MonoBehaviour
    {
        [SerializeField] private SocAssemblyEvents events_Assembly;
        [SerializeField] private SocAssemblyUI assemblyUI;

        private PhotonView _photonView;
        private bool _isMasterClient;

        private IList<IResourceLocation> _assemblyAssetsLocations = new List<IResourceLocation>();
        private Transform _assembly;

        private void Awake()
        {
            _isMasterClient = PhotonNetwork.IsMasterClient;
            _photonView = GetComponent<PhotonView>();

            // This scenario if the master client had already loaded the model in the local scene
            if (PhotonNetwork.IsMasterClient && events_Assembly.IsInitialised)
            {
                int currentAssemblyID = events_Assembly.CurrentCatalogue.ID;
                _photonView.RPC("OnAssemblySelectedRPC", RpcTarget.All, new object[] { currentAssemblyID });

                // Disable catalogue UI
                assemblyUI.InvokeToggleCatalogueUI(false);
            }
            else
            {
                // Enable catalogue UI
                assemblyUI.InvokeToggleCatalogueUI(true);
            }
        }

        private void OnEnable()
        {
            assemblyUI.EUIRequestAssemblySelect += OnUIAssemblySelect;
        }

        private void OnDisable()
        {
            assemblyUI.EUIRequestAssemblySelect -= OnUIAssemblySelect;
        }


        private void OnUIAssemblySelect(SCatalogueInfo assemblyInfo)
        {
            _photonView.RPC("OnAssemblySelectedRPC", RpcTarget.AllBuffered, new object[] { assemblyInfo.ID });
        }


        [PunRPC]
        void OnAssemblySelectedRPC(int assemblyID)
        {
            Debug.Log("#AssemblyStateManager#---------RPC reveived - Assembly Selected: " + assemblyID);

            foreach (var catalogueEntry in events_Assembly.Catalogue)
            {
                if (catalogueEntry.ID == assemblyID)
                {
                    OnAssemblySelected(catalogueEntry);
                }

                // Disable catalogue UI
                assemblyUI.InvokeToggleCatalogueUI(false);
            }
        }


        private void OnAssemblySelected(SCatalogueInfo assemblyInfo)
        {
            LoadAssemblyAssets(new string[] { assemblyInfo.AssemblyFolderKey }, assemblyInfo.AssemblyPrefab);
        }

        private async Task LoadAssemblyAssets(string[] label, AssetReference assemblyPrefab)
        {
            // Load assembly dependencies
            await AddressableLoader.LoadLabels(label, _assemblyAssetsLocations);

            // Instantiate Assembly
            var result = await Addressables.InstantiateAsync(assemblyPrefab, Vector3.zero, Quaternion.identity).Task;
            _assembly = result.transform;
        }
    }
}