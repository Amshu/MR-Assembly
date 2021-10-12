using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public class FocusedModule : AUnit
    {
        [SerializeField] private SocAssemblySettings settings;
        [SerializeField] private SocAssemblyEvents assemblyEvents;
        [SerializeField] private Transform rootTransform;
        [SerializeField] private Transform explodedRootTransform;

        [Header("Debug")]
        [SerializeField] private int _subModulesCount;
        [SerializeField] private SubModule[] _subModules;
        [SerializeField] private Vector3[] _disassembledPositions;
        [SerializeField] private Vector3[] _assembledPositions;

        [Space]
        [SerializeField] private bool _isAssembled = true;
        [SerializeField] private bool _isBusy = false;




        // For Editor Script
        public Transform RootTransform => rootTransform;
        public SubModule[] SubModules => _subModules;
        public int SubModulesCount => _subModulesCount;



        private void Start()
        {
            BoundsControl boundsControl = GetComponentInParent<BoundsControl>();
            boundsControl.BoundsOverride = GetComponent<BoxCollider>();

            UpdateSubModules();

            Debug.Log("SOCAssemblyEvents: " + assemblyEvents.GetInstanceID());
        }


        private void OnEnable()
        {
            assemblyEvents.EModuleExplode += OnExplosionToggleRequest;
        }

        private void OnDisable()
        {
            assemblyEvents.EModuleExplode -= OnExplosionToggleRequest;
        }

        public void ToggleExplosion(bool toggle)
        {
            OnExplosionToggleRequest(toggle);
        }


        private void OnExplosionToggleRequest(bool toggle)
        {
            if (_isBusy) return;

            StopAllCoroutines();
            StartCoroutine(ExplodeModules(toggle));

            _isBusy = true;
        }


        IEnumerator ExplodeModules(bool toggle)
        {
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / settings.ExplodeTime;

                for (int i = 0; i < _subModulesCount; i++)
                {
                    var currentTransform = rootTransform.GetChild(i);
                    var currentPos = currentTransform.localPosition;

                    var endPos = (toggle) ? _disassembledPositions[i] : _assembledPositions[i];

                    currentTransform.localPosition = Vector3.Lerp(currentPos, endPos, t);
                }

                yield return null;
            }

            _isAssembled = toggle;
            _isBusy = false;

            yield return null;
        }

        /// <summary>
        /// Get all the submodules and their exploded/imploded transforms
        /// </summary>
        /// <returns></returns>
        public bool UpdateSubModules()
        {
            // Get the count of sub modules
            _subModulesCount = rootTransform.childCount;

            List<Vector3> implodedPos = new List<Vector3>();
            List<Vector3> explodedPos = new List<Vector3>();
            List<SubModule> subModules = new List<SubModule>();

            // Ensure each module has an exploded transform
            if (explodedRootTransform.childCount != _subModulesCount)
            {
                Debug.LogError("#FocusedModule#--------Error: HIERARCHY - Children count not same");
                return false;
            }

            for (int i = 0; i < explodedRootTransform.childCount; i++)
            {
                var rootChild = rootTransform.GetChild(i);
                var explodedRootChild = explodedRootTransform.GetChild(i);

                // Add to list only if the disassembled transform has the same name as its counter part
                if (explodedRootChild.name.Contains(rootChild.name))
                {
                    explodedPos.Add(explodedRootChild.localPosition);
                    implodedPos.Add(rootChild.localPosition);
                }
                else
                {
                    Debug.LogError("#FocusedModule#--------Error: HIERARCHY - Check children names");
                    return false;
                }


                // Check if child already has componenet
                rootChild.TryGetComponent<SubModule>(out SubModule subModule);
                if (subModule == null)
                {
                    // Add submodule component
                    subModule = rootChild.gameObject.AddComponent<SubModule>();
                }

                subModules.Add(subModule);
            }

            _assembledPositions = implodedPos.ToArray();
            _disassembledPositions = explodedPos.ToArray();
            _subModules = subModules.ToArray();

            return true;
        }


        //void IFocusedModule.ChangePosition(int step)
        //{
        //int x = Mathf.Clamp(currentModNo + step, 0, mNoOfSteps);
        //ChangeCurrentUnitPosition((x));
        //}

    
        //     private void ChangeCurrentUnitPosition(int unitPosition)
        //     {
        //         Debug.Log("#AssemblyModule#-------------------------Changing assembly position to: " + unitPosition);
        //
        //         currentModNo = unitPosition;
        //
        //         for (var i = 0; i < _subModules.Length; i++)
        //         {
        //             var part = _subModules[i];
        //             var partPosition = part.GetUnitPosition();
        //
        //             // If its less than or equal to the passed unit position => Explode
        //             if (partPosition <= unitPosition)
        //             {
        //                 part.Assemble(mUnitSettings.positionTimeChange);
        //
        //                 // Highlight previous part
        //                 if (partPosition == unitPosition - 1)
        //                 {
        //                 }
        //                 // Highlight current part
        //                 else if(partPosition == unitPosition)
        //                 {
        //                 }
        //                 else
        //                 {
        //                 }
        //             }
        //             // If its greater than the passed unit position => Implode
        //             else
        //             {
        //                 part.Disassemble(mUnitSettings.positionTimeChange);
        //
        //                 // Highlight next part
        //                 if(partPosition == unitPosition + 1)
        //                 {
        //                 }
        //                 else
        //                 {
        //                 }
        //             }
        //         }
        //     }
    }
}