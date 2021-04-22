using System.Collections;
using UnityEngine;

namespace HYDAC_EView.Scripts.MPart
{
    public class MachinePart : MonoBehaviour
    {
        [SerializeField] private SocMachinePartInfo mPartInfo = null;
        public SocMachinePartInfo PartInfo { get => mPartInfo; }

        [SerializeField] private Transform mImplodedTransform = null;
        [SerializeField] private Transform mExplodedTransform = null;

        private bool _mLock = false;

        private IEnumerator LerpPosition(Transform trans, Vector3 position, float timeTakenToDest)
        {
            var currentPos = trans.position;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / timeTakenToDest;
                trans.position = Vector3.Lerp(currentPos, position, t);
                yield return null;
            }

            _mLock = false;
        }


        #region IMachinePart interface methods

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        int GetAssemblyPosition()
        {
            return mPartInfo.assemblyPosition;
        }


        void Implode(float timeTakenToDest)
        {
            Debug.Log("#MachinePart#-------------------------Implode :");
            mPartInfo.PrintInfo();

            if (_mLock) return;
            
            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, mImplodedTransform.position, timeTakenToDest));
        }


        void Explode(float timeTakenToDest)
        {
            Debug.Log("#MachinePart#-------------------------Explode");

            if (_mLock) return;
            
            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, mExplodedTransform.position, timeTakenToDest));
        }
    

        void HighlightPart(bool toggle, Material highlightMaterial)
        {

        }

        public string GetPartName()
        {
            return mPartInfo.partName;
        }

        #endregion

        public void SetPartInfo(SocMachinePartInfo info)
        {
#if UNITY_EDITOR
            mPartInfo = info;
#endif
        }

    }
}
