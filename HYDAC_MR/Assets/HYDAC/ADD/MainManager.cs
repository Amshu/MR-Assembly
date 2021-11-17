using UnityEngine;

using UnityEngine.ResourceManagement.ResourceProviders;


namespace HYDAC.Scripts.ADD
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private SocAddressablesSettings settings;

        private SceneInstance _currentEnvironment = default;

        private Transform _netManager;

        private void Awake()
        {
            LoadManagerAndScene();
        }

        private async void LoadManagerAndScene()
        {
            // Then load environment
            _currentEnvironment = await AddressablesSceneLoader.LoadScene(settings.Env_Default, true);

            var loadedManager = await AddressableLoader.LoadFromReference(settings.NetManager);
            _netManager = Instantiate(loadedManager).transform;
        }
    }
}
