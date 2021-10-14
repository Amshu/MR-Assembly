using HYDAC.Scripts.INFO;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
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


#if UNITY_EDITOR
        // For Editor Script
        public Transform RootTransform => rootTransform;
        public SubModule[] SubModules => _subModules;
        public int SubModulesCount => _subModulesCount;
#endif

        private void Start()
        {
            BoundsControl boundsControl = GetComponentInParent<BoundsControl>();
            boundsControl.BoundsOverride = GetComponent<BoxCollider>();

            UpdateSubModules();
        }


        private void OnEnable()
        {
            assemblyEvents.EModuleExplode += OnExplosionToggleRequest;
            assemblyEvents.ESubModuleSelected += OnSubModuleSelected;
        }


        private void OnDisable()
        {
            assemblyEvents.EModuleExplode -= OnExplosionToggleRequest;
            assemblyEvents.ESubModuleSelected -= OnSubModuleSelected;
        }

        public void ToggleExplosion(bool toggle)
        {
            OnExplosionToggleRequest(toggle);
        }


        private void OnExplosionToggleRequest(bool toggle)
        {
            if (_isBusy || (info as SModuleInfo).isStatic) return;

            StopAllCoroutines();
            StartCoroutine(ToggleExplosion(toggle, -1));

            _isBusy = true;
        }


        IEnumerator ToggleExplosion(bool toggle, int subMododuleID = -1)
        {
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / settings.ExplodeTime;

                for (int i = 0; i < _subModulesCount; i++)
                {
                    var currentTransform = rootTransform.GetChild(i);
                    var currentPos = currentTransform.localPosition;

                    Vector3 endPos = new Vector3();

                    // If there was no sub module selected then toggle explosion
                    if(subMododuleID == -1)
                    {
                        endPos = (toggle) ? _disassembledPositions[i] : _assembledPositions[i];
                    }
                    // If a submodule was selected then explode only the selected module
                    else
                    {
                        endPos = (_subModules[i].Info.ID == subMododuleID) ? _disassembledPositions[i] : _assembledPositions[i];
                    }

                    // Lerp
                    currentTransform.localPosition = Vector3.Lerp(currentPos, endPos, t);
                }

                yield return null;
            }

            _isAssembled = toggle;
            _isBusy = false;

            yield return null;
        }



        private void OnSubModuleSelected(SSubModuleInfo selectedSubModule)
        {
            if (_isBusy || (info as SModuleInfo).isStatic) return;

            StopAllCoroutines();
            StartCoroutine(ToggleExplosion(false, selectedSubModule.ID));

            _isBusy = true;
        }


        /// <summary>
        /// Get all the submodules and their exploded/imploded transforms
        /// </summary>
        /// <returns></returns>
        public bool UpdateSubModules()
        {
            if ((info as SModuleInfo).isStatic) return false;

            // Get the count of sub modules
            _subModulesCount = rootTransform.childCount;

            List<Vector3> implodedPos = new List<Vector3>();
            List<Vector3> explodedPos = new List<Vector3>();

            List<SubModule> subModules = new List<SubModule>();

            // Ensure each sub module has an exploded transform
            if (explodedRootTransform.childCount != _subModulesCount)
            {
                Debug.LogError("#FocusedModule#--------Error: HIERARCHY - Children count not same");
                return false;
            }

            // Iterate through each submodule
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


                // Check if child already has component
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