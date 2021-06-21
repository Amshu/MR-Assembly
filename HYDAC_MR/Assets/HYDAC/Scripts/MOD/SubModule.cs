using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace HYDAC.Scripts.MOD
{
    public class SubModule : MonoBehaviour
    {
        [FormerlySerializedAs("mPart")] [FormerlySerializedAs("mPartInfo")] [SerializeField] private SSubModule mSubModule = null;
        public SSubModule subModule => mSubModule;

        [SerializeField] private Transform mExplodedTransform = null;

        private bool _mLock = false;
        private Vector3 _mImplodedPosition = Vector3.zero;

        //private MeshRenderer _mesh = null;
        //private Material _defaultMaterial = null;

        private void Awake()
        {
            _mImplodedPosition = transform.localPosition;
            //_mMesh = GetComponent<MeshRenderer>();
            //_mDefaultMaterial = _mMesh.material;
        }


        private void OnEnable()
        {
            // Subscribe to event in machine part info
            mSubModule.OnInitialize += OnInitialized;
            mSubModule.OnImplode += OnImploded;
            
            mSubModule.OnExplode += OnExploded;
            mSubModule.OnHighlightPart += OnHighlighted;
        }
        private void OnDisable()
        {
            // Subscribe to event in machine part info
            mSubModule.OnInitialize += OnInitialized;
            mSubModule.OnImplode += OnImploded; 

            mSubModule.OnExplode += OnExploded;
            mSubModule.OnHighlightPart += OnHighlighted;
        }


        #region MachinePartInfo event callbacks

        private void OnInitialized(int unitPosition, string partName)
        {
            throw new System.NotImplementedException();
        }

        private void OnImploded(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#MachinePart#-------------------------Implode :");
            mSubModule.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, _mImplodedPosition, timeTakenToDest));
        }


        private void OnExploded(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#MachinePart#-------------------------Explode");
            mSubModule.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, mExplodedTransform.localPosition, timeTakenToDest));
        }
    

        void OnHighlighted(bool toggle, Material highlightMaterial = null)
        {
            //_mMesh.material = (toggle)? highlightMaterial : _mDefaultMaterial;
        }

        #endregion



        private IEnumerator LerpPosition(Transform trans, Vector3 position, float timeTakenToDest)
        {
            var currentPos = trans.localPosition;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / timeTakenToDest;
                trans.localPosition = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }

            _mLock = false;
        }



        public void SetPartInfo(SSubModule info)
        {
#if UNITY_EDITOR
            mSubModule = info;
#endif
        }

    }
}
