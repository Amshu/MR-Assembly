using System.Collections;
//using Normal.Realtime;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    //[RequireComponent(typeof(RealtimeTransform))]
    
    public interface ISubModule
    {
        void Initialize();
        int GetUnitPosition();
        string GetPartName();

        void Assemble(float timeToDest);
        void Disassemble(float timeToDest);
    }
    
    
    public class SubModule : AUnit
    {
        [SerializeField] private Transform mDisassembledTransform = null;

        private bool _mLock = false;
        private Vector3 _mAssembledPosition = Vector3.zero;

        //private RealtimeTransform _realtimeTransform = null;

        private void Awake()
        {
            _mAssembledPosition = transform.localPosition;
            
            //_realtimeTransform = GetComponent<RealtimeTransform>();
        }


        private void OnEnable()
        {
            // Subscribe to event in machine part info
            //SSubModuleInfo sInfo = Info as SSubModuleInfo;
            
            // Subscribe to event in machine part info
            //sInfo.OnInitialize += OnInitialized;
            //sInfo.OnAssemble += OnAssembled;
            
            //sInfo.OnDisassemble += OnDisassemble;
            //sInfo.OnHighlight += OnHighlighted;
        }
        private void OnDisable()
        {
            // Subscribe to event in machine part info
            //SSubModuleInfo sInfo = Info as SSubModuleInfo;
            
            //sInfo.OnInitialize += OnInitialized;
            //sInfo.OnAssemble += OnAssembled;
            //sInfo.OnDisassemble += OnDisassemble;
            // mSubModuleInfo.OnHighlight += OnHighlighted;
        }


        #region MachinePartInfo event callbacks

        private void OnInitialized(int unitPosition, string partName)
        {
            throw new System.NotImplementedException();
        }

        public void OnAssembled(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#SubModule#-------------------------Implode :");
            //mSubModuleInfo.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, _mAssembledPosition, timeTakenToDest));
        }


        public void OnDisassemble(float timeTakenToDest)
        {
            // Dont do anything if the part is not free
            if (_mLock) return;

            Debug.Log("#SubModule#-------------------------Explode");
            //mSubModuleInfo.PrintInfo();

            _mLock = true;
            
            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, mDisassembledTransform.localPosition, timeTakenToDest));
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
    }
}
