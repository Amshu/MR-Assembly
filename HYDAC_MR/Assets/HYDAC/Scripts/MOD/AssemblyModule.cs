using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;

namespace HYDAC.Scripts.MOD
{
    public interface IAssemblyModule : IBaseModule
    {
        public event Action<AssemblyModule> OnModuleFocused;

        int GetAssemblyPosition();
        
        void Assemble();
        void Disassemble();
        
        void ChangePosition(int step);
    }
    
    public sealed class AssemblyModule : BaseModule, IAssemblyModule
    {
        private const string MachinePartInfoFolderPath = "SubModuleInfos";
        
        [Header("Assembly Members")]
        [SerializeField] private SocMainSettings mUnitSettings;

        [Header("Exploded View Members")]
        [SerializeField] private int mNoOfSteps = 0;
        public int NoOfSteps => mNoOfSteps;

        private ObjectManipulator _objectManipulator = null;
        private MoveAxisConstraint _moveAxisConstraint = null;
        private BoundsControl _boundsControl = null;
        
        private ISubModule[] _subModules;
        
        [Header("Debug")]
        public int currentModNo;
        public int startingPosition;

        protected override void Awake()
        {
            base.Awake();
            
            _objectManipulator = GetComponent<ObjectManipulator>();
            _moveAxisConstraint = GetComponent<MoveAxisConstraint>();
            _boundsControl = GetComponent<BoundsControl>();
            
            GetSubModules();

            currentModNo = startingPosition;
        }

        private void OnEnable()
        {
            _objectManipulator.OnManipulationStarted.AddListener(OnManipulationStarted);
        }

        private void OnDisable()
        {
            _objectManipulator.OnManipulationStarted.RemoveListener(OnManipulationStarted);
        }

        private void OnManipulationStarted(ManipulationEventData arg0)
        {
            if (isInFocus) return;

            //Debug.Log("#AssemblyModule#-------------OnManipulationStarted: " + name);
            
            // Raise OnFocused event
            RaiseOnFocused(this);
            
            (this as IBaseModule).ToggleFocus(true);
        }


        #region PARENT CLASS OVERRIDDEN METHODS ---------------

        protected override void OnFocused()
        {
            base.OnFocused();
            
            ToggleComponents(false);
        }

        protected override void OnUnfocused()
        {
            base.OnUnfocused();
            
            ToggleComponents(true);
        }

        protected override void OnReset()
        {
            base.OnReset();
            
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

        private void GetSubModules()
        {
            // Load from Resources
            var machinePartInfos = Resources.LoadAll(MachinePartInfoFolderPath, typeof(ISubModule));
            if (machinePartInfos.Length < 1)
            {
                Debug.LogError("No sub-module infos found or loaded. Exiting Application");
                Application.Quit();
                return;
            }

            // Cast each loaded object to IMachinePart
            List<ISubModule> parts = new List<ISubModule>();
            foreach(object ogj in machinePartInfos)
            {
                parts.Add(ogj as ISubModule);
            }

            // Set to main array
            _subModules = parts.ToArray();
            if (_subModules.Length < 1)
            {
                Debug.LogError("Error in casting to IMachinePart[]. Exiting Application");
                Application.Quit();
                return;
            }

            // Sort all parts by their assembly position
            _subModules = _subModules.OrderBy(x => x.GetUnitPosition()).ToArray();

            // Get total number of assemblies
            mNoOfSteps = _subModules[_subModules.Length - 1].GetUnitPosition();
        }

        private void ToggleComponents(bool toggle)
        {
            // Switch components
            _moveAxisConstraint.enabled = !toggle;
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
                        part.ChangeMaterial(true, mUnitSettings.previousUnitMaterial);
                    }
                    // Highlight current part
                    else if(partPosition == unitPosition)
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                }
                // If its greater than the passed unit position => Implode
                else
                {
                    part.Disassemble(mUnitSettings.positionTimeChange);

                    // Highlight next part
                    if(partPosition == unitPosition + 1)
                    {
                        part.ChangeMaterial(true, mUnitSettings.nextUnitMaterial);
                    }
                    else
                    {
                        part.ChangeMaterial(false, mUnitSettings.currentUnitMaterial);
                    }
                }
            }
        }
    }
}
