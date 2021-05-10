using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.MAC
{
    public class MachinePart : MonoBehaviour
    {
        [SerializeField] private SocMachinePartInfo mPartInfo = null;
        public SocMachinePartInfo PartInfo { get => mPartInfo; }

        [SerializeField] private Transform mExplodedTransform = null;

        private bool _mLock = false;
        private Vector3 _mImplodedPosition = Vector3.zero;

        private MeshRenderer _mMesh = null;
        private Material _mDefaultMaterial = null;

        private void Awake()
        {
            _mImplodedPosition = transform.localPosition;
            //_mMesh = GetComponent<MeshRenderer>();
            //_mDefaultMaterial = _mMesh.material;
        }


        private void OnEnable()
        {
            // Subscribe to event in machine part info
            mPartInfo.OnInitialize += OnInitialized;
            mPartInfo.OnImplode += OnImploded;
            mPartInfo.OnExplode += OnExploded;
            mPartInfo.OnHighlightPart += OnHighlighted;
        }
        private void OnDisable()
        {
            // Subscribe to event in machine part info
            mPartInfo.OnInitialize += OnInitialized;
            mPartInfo.OnImplode += OnImploded; 

            mPartInfo.OnExplode += OnExploded;
            mPartInfo.OnHighlightPart += OnHighlighted;
        }


        #region MachinePartInfo event callbacks

        private void OnInitialized(int assemblyPosition, string partName)
        {
            throw new System.NotImplementedException();
        }

        private void OnImploded(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#MachinePart#-------------------------Implode :");
            mPartInfo.PrintInfo();

            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, _mImplodedPosition, timeTakenToDest));
        }


        private void OnExploded(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#MachinePart#-------------------------Explode");
            mPartInfo.PrintInfo();

            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, mExplodedTransform.localPosition, timeTakenToDest));
        }
    

        void OnHighlighted(bool toggle, Material highlightMaterial)
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



        public void SetPartInfo(SocMachinePartInfo info)
        {
#if UNITY_EDITOR
            mPartInfo = info;
#endif
        }

    }
}
