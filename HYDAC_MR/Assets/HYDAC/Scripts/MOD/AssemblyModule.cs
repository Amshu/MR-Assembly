using System;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

using HYDAC.SOC.Settings;

namespace HYDAC.Scripts.MOD
{
    public interface IBaseModule
    {
        void ToggleFocus(bool toggle);
        
        /// <summary>
        ///  This is for the part in focus 
        /// </summary>
        void Reset();

        string GetName();
    }
    
    public interface IAssemblyModule : IBaseModule
    {
        public event Action<AssemblyModule> OnModuleFocused;

        int GetAssemblyPosition();
        
        void Assemble();
        void Disassemble();
        
        void ChangePosition(int step);
    }
    
    public sealed class AssemblyModule : AModule, IAssemblyModule
    {
        [Header("Assembly Members")]
        [SerializeField] private SocMainSettings mUnitSettings;

        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        public int NoOfSteps => mNoOfSteps;

        private Interactable _interactable = null;
        //private MoveAxisConstraint _moveAxisConstraint = null;
        private BoundsControl _boundsControl = null;
        
        private ISubModule[] _subModules;
        
        [Header("Debug")]
        public int currentModNo;
        public int startingPosition;

        private bool isInFocus = false;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
            
            //_moveAxisConstraint = GetComponent<MoveAxisConstraint>();
            _boundsControl = GetComponent<BoundsControl>();

            //GetSubModules();

            //currentModNo = startingPosition;
        }

        private void OnEnable()
        {
            _interactable.OnClick.AddListener(OnInteractableClicked);
        }
        
        private void OnDisable()
        {
            _interactable.OnClick.RemoveListener(OnInteractableClicked);
        }

        private void OnInteractableClicked()
        {
            if (isInFocus) return;

            Debug.Log("#AssemblyModule#-------------OnClicked: " + name);
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IBaseModule).ToggleFocus(true);
        }


        #region PARENT CLASS OVERRIDDEN METHODS ---------------

        protected override void OnFocused()
        {
            ToggleComponents(false);
        }

        protected override void OnUnfocused()
        {
            ToggleComponents(true);
        }

        protected override void OnReset()
        {
            ToggleComponents(false);
        }

        #endregion

        #region  IASSEMBLYMODULE IMPLEMENTATION ---------------------------------

        public event Action<AssemblyModule> OnModuleFocused;
        private void RaiseOnFocused(AssemblyModule managerRef)
        {
            OnModuleFocused?.Invoke(managerRef);
            
            isInFocus = true;
        }

        public int GetAssemblyPosition()
        {
            return currentModNo;
        }

        public void Assemble()
        {
            ToggleAssembly(true, mUnitSettings.positionTimeChange);
        }

        public void Disassemble()
        {
            ToggleAssembly(false, mUnitSettings.positionTimeChange);
        }

        void IAssemblyModule.ChangePosition(int step)
        {
            if (!isInFocus)
                return;
            
            int x = Mathf.Clamp(currentModNo + step, 0, mNoOfSteps);
            ChangeCurrentUnitPosition((x));
        }

        #endregion
        

        private void ToggleComponents(bool toggle)
        {
            // Switch components
            //_moveAxisConstraint.enabled = !toggle;
            _boundsControl.enabled = toggle;
        }
        
        private void ToggleAssembly(bool toBeAssembled, float positionTimeChange)
        {
            currentModNo = (toBeAssembled) ?  0 : _subModules.Length - 1;
            
            foreach(ISubModule subModule in _subModules)
            {
                // If the current state is exploded then: 
                // Assemble
                if (toBeAssembled)
                {
                    subModule.Assemble(positionTimeChange);
                }
                // Disassemble
                else
                {
                    subModule.Disassemble(positionTimeChange);
                }
            }
        }
        
        
        private void ChangeCurrentUnitPosition(int unitPosition)
        {
            Debug.Log("#AssemblyModule#-------------------------Changing assembly position to: " + unitPosition);

            currentModNo = unitPosition;

            for (var i = 0; i < _subModules.Length; i++)
            {
                var part = _subModules[i];
                var partPosition = part.GetUnitPosition();

                // If its less than or equal to the passed unit position => Explode
                if (partPosition <= unitPosition)
                {
                    part.Assemble(mUnitSettings.positionTimeChange);

                    // Highlight previous part
                    if (partPosition == unitPosition - 1)
                    {
                    }
                    // Highlight current part
                    else if(partPosition == unitPosition)
                    {
                    }
                    else
                    {
                    }
                }
                // If its greater than the passed unit position => Implode
                else
                {
                    part.Disassemble(mUnitSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == unitPosition + 1)
                    {
                    }
                    else
                    {
                    }
                }
            }
        }

        public void ToggleFocus(bool toggle)
        {
            //Debug.Log("#BaseModule#------------------ ToggleFocus: " + toggle + " for: "+ transform.name);

            isInFocus = toggle;

            if(toggle)
                OnFocused();
            else
                OnUnfocused();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }
}
