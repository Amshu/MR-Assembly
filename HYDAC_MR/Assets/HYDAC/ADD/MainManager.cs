using UnityEngine;

using UnityEngine.ResourceManagement.ResourceProviders;


namespace HYDAC.Scripts.ADD
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private SocAddressablesSettings settings;

        Transform _mainManager;

        private SceneInstance _currentEnvironment = default;

        private void Awake()
        {
            LoadScene();
        }

        private async void LoadScene()
        {
            // Then load environment
            _currentEnvironment = await AddressablesSceneLoader.LoadScene(settings.Env_Default, true);

            var loadedManager = await AddressableLoader.LoadFromReference(settings.NetManager);
            _mainManager = Instantiate(loadedManager).transform;
        }
    }
}
