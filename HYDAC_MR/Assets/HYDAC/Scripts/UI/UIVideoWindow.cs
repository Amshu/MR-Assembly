using HYDAC.Scripts.INFO;
using HYDAC.Scripts.MOD;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Video;

namespace HYDAC.Scripts.UI
{
    public class UIVideoWindow : MonoBehaviour
    {
        [SerializeField]
        VideoPlayer videoPlayer;

        [SerializeField]
        SocAssemblyEvents assemblyEvents;

        AssetReference currentVideoRef;

        private void OnEnable()
        {
            assemblyEvents.EModuleSelected += OnModuleSelect;
        }
        private void OnDisable()
        {
            assemblyEvents.EModuleSelected -= OnModuleSelect;
        }

        private void OnModuleSelect(SModuleInfo moduleInfo)
        {
            videoPlayer.Stop();
            currentVideoRef?.ReleaseAsset();

            if (!moduleInfo.HasVideo) return;

            PlayVideo(moduleInfo.VideoReference);
        }

        private async void PlayVideo(AssetReference assetRef)
        {
            Debug.Log("#VideoPlayer#----------Video found. Playing....");

            currentVideoRef = assetRef;

            var loadingTask = await assetRef.LoadAssetAsync<VideoClip>().Task;

            if (loadingTask == null) return;

            videoPlayer.clip = loadingTask;
            videoPlayer.Play();
        }
    }        
}
