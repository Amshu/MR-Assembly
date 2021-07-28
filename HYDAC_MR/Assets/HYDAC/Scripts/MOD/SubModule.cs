using System.Collections;
//using Normal.Realtime;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    //[RequireComponent(typeof(RealtimeTransform))]
    public class SubModule : MonoBehaviour
    {
        [SerializeField] private SSubModule mSubModule = null;
        public SSubModule subModule => mSubModule;

        [SerializeField] private Transform mDisassembledTransform = null;

        private bool _mLock = false;
        private Vector3 _mAssembledPosition = Vector3.zero;

        //private RealtimeTransform _realtimeTransform = null;
        
        private float _mLastOwnershipChangeTime = 0f;
        private bool _mHasOwnership = false;

        private void Awake()
        {
            _mAssembledPosition = transform.localPosition;
            
            //_realtimeTransform = GetComponent<RealtimeTransform>();
        }


        private void OnEnable()
        {
            // Subscribe to event in machine part info
            mSubModule.OnInitialize += OnInitialized;
            mSubModule.OnAssemble += OnAssembled;
            
            mSubModule.OnDisassemble += OnDisassemble;
            mSubModule.OnHighlight += OnHighlighted;
        }
        private void OnDisable()
        {
            // Subscribe to event in machine part info
            mSubModule.OnInitialize += OnInitialized;
            mSubModule.OnAssemble += OnAssembled; 

            mSubModule.OnDisassemble += OnDisassemble;
            mSubModule.OnHighlight += OnHighlighted;
        }


        #region MachinePartInfo event callbacks

        private void OnInitialized(int unitPosition, string partName)
        {
            throw new System.NotImplementedException();
        }

        private void OnAssembled(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#SubModule#-------------------------Implode :");
            mSubModule.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, _mAssembledPosition, timeTakenToDest));
        }


        private void OnDisassemble(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#SubModule#-------------------------Explode");
            mSubModule.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, mDisassembledTransform.localPosition, timeTakenToDest));
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
                
                //_realtimeTransform.RequestOwnership();
                
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

        
        private void Update()
        {
            if (!CheckOwnership()) return;

            // If we just reset objects, we should wait before affecting ownership
            if (Time.time - _mLastOwnershipChangeTime < 1f) return;

            // If object is not being manipulated, or it is at rest, clear ownership
            //_realtimeTransform.ClearOwnership();
        }
        
        private bool CheckOwnership()
        {
            // If there are any inconsistency update - when focus is toggled on or off
            // if (_mHasOwnership != _realtimeTransform.isOwnedLocallySelf)
            // {
            //     _mHasOwnership = _realtimeTransform.isOwnedLocallySelf;
            //     ResetOwnershipTime();
            // }
            return _mHasOwnership;
        }
        
        private void ResetOwnershipTime()
        {
            _mLastOwnershipChangeTime = Time.time;
        }
    }
}
