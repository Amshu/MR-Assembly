using System.Collections;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
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
        private bool _mLock = false;
        private Vector3 _mAssembledPosition = Vector3.zero;

        private void Awake()
        {
            _mAssembledPosition = transform.localPosition;
        }

        public void OnAssemble(float timeTakenToDest)
        {
            Debug.Log("#SubModule#-------------------------Implode: " + this.name);

            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, _mAssembledPosition, timeTakenToDest));
        }


        public void OnDisassemble(float timeTakenToDest, Vector3 dissambledPos)
        {
            Debug.Log("#SubModule#-------------------------Explode: " + this.name);

            StopAllCoroutines();
            StartCoroutine(LerpPosition(this.transform, dissambledPos, timeTakenToDest));
        }
        
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
    }
}
