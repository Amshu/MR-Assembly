using System;
using HYDAC.SOCS;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine;

namespace HYDAC.Scripts.MOD
{
    public interface IFocusedModule : IBaseModule
    {
        void ChangePosition(int step);
    }

    public class FocusedModule : AUnit
    {
        [SerializeField] private SocModuleUI socUI = null;
        [SerializeField] private SubModule[] subModules;

        private bool _isAssembled = true;

        private void Start()
        {
            BoundsControl _boundsControl = GetComponentInParent<BoundsControl>();
            _boundsControl.BoundsOverride = GetComponent<BoxCollider>();
        }


        private void OnEnable()
        {
            socUI.EUIRequestModuleToggle += OnUIRequestModuleToggle;
        }

        private void OnDisable()
        {
            socUI.EUIRequestModuleToggle -= OnUIRequestModuleToggle;
        }

        private void OnUIRequestModuleToggle()
        {
            if (_isAssembled)
            {
                foreach (var subModule in subModules)
                {
                    subModule.OnDisassemble(0.3f);
                }
            }
            else
            {
                foreach (var subModule in subModules)
                {
                    subModule.OnAssembled(0.3f);
                }
            }

            _isAssembled = !_isAssembled;
        }

        //void IFocusedModule.ChangePosition(int step)
        //{
        //int x = Mathf.Clamp(currentModNo + step, 0, mNoOfSteps);
        //ChangeCurrentUnitPosition((x));
        //}


        // private void ToggleAssembly(bool toBeAssembled, float positionTimeChange)
        //     {
        //         currentModNo = (toBeAssembled) ?  0 : _subModules.Length - 1;
        //         
        //         foreach(ISubModule subModule in _subModules)
        //         {
        //             // If the current state is exploded then: 
        //             // Assemble
        //             if (toBeAssembled)
        //             {
        //                 subModule.Assemble(positionTimeChange);
        //             }
        //             // Disassemble
        //             else
        //             {
        //                 subModule.Disassemble(positionTimeChange);
        //             }
        //         }
        //     }
        //     
        //     
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