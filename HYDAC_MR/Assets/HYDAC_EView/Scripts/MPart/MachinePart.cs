using System.Collections;
using UnityEngine;

namespace HYDAC_EView._Scripts.MPart
{
    [RequireComponent(typeof(Outline))]
    public class MachinePart : MonoBehaviour, IMachinePart
    {
        private MainManager m_manager;
        
        [SerializeField] private SocMachinePartInfo mPartInfo = null;
        public SocMachinePartInfo PartInfo { get => mPartInfo; }

        [SerializeField] private Transform mImplodedTransform = null;
        [SerializeField] private Transform mExplodedTransform = null;

        private bool _mLock = false;
        private Outline _mOutline = null;

        private void Awake()
        {
            _mOutline = GetComponent<Outline>();
            _mOutline.enabled = false;
        }


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

        int IMachinePart.GetAssemblyPosition()
        {
            return mPartInfo.assemblyPosition;
        }


        void IMachinePart.Implode(float timeTakenToDest)
        {
            Debug.Log("#MachinePart#-------------------------Implode :");
            mPartInfo.PrintInfo();

            if (_mLock) return;
            
            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, mImplodedTransform.position, timeTakenToDest));
        }


        void IMachinePart.Explode(float timeTakenToDest)
        {
            Debug.Log("#MachinePart#-------------------------Explode");

            if (_mLock) return;
            
            _mLock = true;
            StartCoroutine(LerpPosition(this.transform, mExplodedTransform.position, timeTakenToDest));
        }
    

        void IMachinePart.HighlightPart(bool toggle, Color highlightColor)
        {
            _mOutline.enabled = toggle;

            if (toggle)
                _mOutline.OutlineColor = highlightColor;
        }

        public string GetPartName()
        {
            return mPartInfo.partName;
        }

        #endregion

    }
}
