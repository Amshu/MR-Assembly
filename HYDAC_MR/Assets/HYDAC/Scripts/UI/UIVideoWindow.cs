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

            if (moduleInfo.VideoReference != null)
                LoadVideo(moduleInfo.VideoReference);
        }

        private async void LoadVideo(AssetReference assetRef)
        {
            Debug.Log("#VideoPlayer#----------Video found. Playing....");

            videoPlayer.clip = await assetRef.LoadAssetAsync<VideoClip>().Task;

            videoPlayer.Play();
        }
    }        
}
