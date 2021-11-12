using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

using TMPro;

using HYDAC.Scripts.ADD;


public class MainLoader : MonoBehaviour
{
    [SerializeField] private SocAddressablesSettings settings;
    [SerializeField] private GameObject loadingSphere;

    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Slider loadingSlider;

    private IList<IResourceLocation> _onStartLoadedAssetsLocations = new List<IResourceLocation>();

    private void Awake()
    {
        // Initialise Addressables
        Addressables.InitializeAsync();
        Addressables.InitializeAsync().Completed += OnAddressablesInitialised;
    }

    /// <summary>
    /// ON ADDRESSABLES INITIALISED
    /// ---------------------------
    ///     -> Set initialised to true
    ///     -> Load catalogue
    /// </summary>
    /// <param name="obj"></param>
    private void OnAddressablesInitialised(AsyncOperationHandle<IResourceLocator> obj)
    {
        Debug.Log("#MainLoader#-------------Initialised");

        StartCoroutine(LoadAssets());
    }


    IEnumerator LoadAssets()
    {
        var labelsToLoad = settings.LoadAssets_OnStart;

        // First load all the assets
        for(int i = 0; i < labelsToLoad.Length ; i++)
        {
            loadingText.text = "Loading " + labelsToLoad[i] + " assets";

            yield return StartCoroutine(LoadAssetsFromLabel(labelsToLoad[i]));
        }

        var loadedScene = AddressablesSceneLoader.LoadScene(settings.MainScene, true);
        yield return new WaitUntil(() => loadedScene.IsCompleted);

        SceneManager.SetActiveScene(loadedScene.Result.Scene);

        yield return new WaitForSeconds(5f);

        loadingSphere.SetActive(false);
    }

    private IEnumerator LoadAssetsFromLabel(string label)
    {
        var isDone = false;

        var download = Addressables.LoadResourceLocationsAsync(label);
        download.Completed += (operation) =>
        {
            isDone = true;
            loadingSlider.value = loadingSlider.maxValue;

            //_onStartLoadedAssetsLocations. (operation.Result);
        };

        while (!isDone)
        {
            loadingSlider.value = download.PercentComplete;
            yield return 0f;
        }

        yield return new WaitUntil(() => isDone);
    }

}
