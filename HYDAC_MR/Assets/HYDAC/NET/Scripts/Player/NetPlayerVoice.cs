using UnityEngine;

using Photon.Voice.Unity;

namespace Assets.HYDAC.Scripts.NET
{
    class NetPlayerVoice: MonoBehaviour
    {
        public Transform mouth;

        [Range(1,1000)]
        public float AvgAmpOffset;

        private Recorder _voice;
        private float _mouthSize;

        void Awake()
        {
            // Get a reference to the RealtimeAvatarVoice component
            _voice = GetComponent<Recorder>();
        }

        void LateUpdate()
        {
            // Use the current voice volume (a value between 0 - 1) to calculate the target mouth size (between 0.1 and 1.0)
            float targetMouthSize = Mathf.Lerp(0.1f, 1.0f, _voice.LevelMeter.CurrentAvgAmp * 250);

            //Debug.Log(_voice.LevelMeter.CurrentPeakAmp + "   " + targetMouthSize);

            // Animate the mouth size towards the target mouth size to keep the open / close animation smooth
            _mouthSize = Mathf.Lerp(_mouthSize, targetMouthSize, 30.0f * Time.deltaTime);

            // Apply the mouth size to the scale of the mouth geometry
            Vector3 localScale = mouth.localScale;
            localScale.y = _mouthSize;
            mouth.localScale = localScale;
        }
    }
}
