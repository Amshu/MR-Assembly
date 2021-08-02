using System.Collections.Generic;
using UnityEngine;

using HYDAC.Scripts.MOD;
using UnityEngine.Video;

namespace HYDAC.Scripts
{
    public class MainManager : MonoBehaviour
    {
        private bool _inFocus;

        //[SerializeField]
        //[Tooltip("Delay before ownership is reset after ownership is changed.")]
        //private float ownershipTimeOutTime = 1f;
        
        [SerializeField] private GameObject buttons;

        [Space] [Header("To be refactored")] 
        [SerializeField] private AssemblyModule _accumulator = null;
        [SerializeField] private Canvas _accumulatorUI = null;
        [SerializeField] private VideoPlayer _videoPlayer = null;
        [SerializeField] private GameObject _videoUI = null;

        private IAssemblyModule[] _assemblyModules;
        private IAssemblyModule _localCurrentAssemblyModule;

        private float lastOwnershipChangeTime = 0f;
        private bool hasOwnership = false;

        private void Awake()
        {
            // TO BE REFACTORED
            buttons.SetActive(false);
            _videoUI.SetActive(false);
            _accumulatorUI.enabled = false;
        }
        
        
        
        /// <summary>
        /// ON NETWORK MODEL INITIALIZED OR REPLACED
        /// ----------------------------------------
        /// </summary>
        // protected override void OnRealtimeModelReplaced(RoomStateNCModel previousModel, RoomStateNCModel currentModel)
        // {
        //     if (previousModel != null)
        //     {
        //         // Unregister from NormCore events
        //         previousModel.currentAssemblyNameDidChange -= OnCurrentAssemblyNameChange;
        //         previousModel.isAssembledDidChange -= OnAssemblyStateChange;
        //     }
        //
        //     if (currentModel != null)
        //     {
        //         // If this is a model that has no data set on it, populate it with the current filed lines script
        //         if (currentModel.isFreshModel)
        //         {
        //             currentModel.currentAssemblyName = "";
        //             currentModel.isAssembled = true;
        //         }
        //
        //         // Register for NormCore events
        //         currentModel.currentAssemblyNameDidChange += OnCurrentAssemblyNameChange;
        //         currentModel.isAssembledDidChange += OnAssemblyStateChange;
        //     }
        // }


        /// <summary>
        ///  ON LOCAL USER MODULE SELECT
        /// ----------------------------
        ///
        /// - Check current system state
        /// - Request ownership
        /// - Set property on network
        /// 
        /// </summary>
        /// <param name="focusedModule"> Selected module</param>
        private void OnAssemblyModuleManipulationStart(AssemblyModule focusedModule)
        {
            if (_inFocus) return;
            
            Debug.Log("#MainManager#-------------OnManipulationStarted: " + name);
            
            //realtimeView.ClearOwnership();
            //realtimeView.RequestOwnership();

            //model.currentAssemblyName = focusedModule.transform.name;
        }
        
        /// <summary>
        ///  ON LOCAL USER UI REQUEST RESET
        /// -------------------------------
        ///
        /// - Check current system state
        /// - Request ownership
        /// - Set property on network
        /// 
        /// </summary>
        public void OnUIRequestFocusExit()
        {
            if (!_inFocus) return;

            Debug.Log("#MainManager#-------------OnUIRequestExitFocus");
            
            //realtimeView.ClearOwnership();
            //realtimeView.RequestOwnership();

            //model.currentAssemblyName = "";
        }
        
        /// <summary>
        ///  ON NETWORK _currentAssemblyName PROPERTY CHANGED
        /// -------------------------------------------------
        ///
        /// - If newValue = null
        ///     - If disassembled then assemble
        ///     - Set local currentAssembly as null
        ///     - Set the system mode to 'UNFOCUSED'
        ///     - Disable the UI for the 'Focused' mode
        ///     - If realtimeView owned locally -> Exit Focus
        /// 
        /// - Else
        ///     - Set local currentAssembly according to newValue
        ///     - Set the system mode to 'Focused'
        ///     - Enable the UI for the 'Focused' mode
        ///     - If realtimeView owned locally -> Enter Focus
        ///  
        /// </summary>
        /// <param name="roomStateNcModel"></param>
        /// <param name="newValue"></param>
        // private void OnCurrentAssemblyNameChange(RoomStateNCModel roomStateNcModel, string newValue)
        // {
        //     Debug.Log("#MainManager#-------------OnCurrentAssemblyNameChange");
        //
        //     if (newValue.Equals(""))
        //     {
        //         if (!model.isAssembled)
        //         {
        //             _localCurrentAssemblyModule.Assemble();
        //             model.isAssembled = true;
        //         }
        //         
        //         _localCurrentAssemblyModule = null;
        //         _inFocus = false;
        //
        //         buttons.SetActive(false);
        //
        //         if (realtimeView.isOwnedLocallySelf)
        //         {
        //             Debug.Log("#MainManager#-------------ExitFocus");
        //             ExitFocus();
        //         }
        //         
        //         // TO BE REFACTORED
        //         _videoPlayer.Stop();
        //         _videoUI.SetActive(false);
        //         _accumulatorUI.enabled = false;
        //
        //     }
        //     else
        //     {
        //         foreach (var module in modules)
        //         {
        //             if (module.name.Equals(newValue))
        //             {
        //                 _localCurrentAssemblyModule = module as IAssemblyModule;
        //                 
        //                 Debug.Log("#MainManager#-------------Set _localCurrentAssemblyModule = " + _localCurrentAssemblyModule.GetName());
        //                 
        //                 break;
        //             }
        //         }
        //         _inFocus = true;
        //
        //         buttons.SetActive(true);
        //
        //         if (realtimeView.isOwnedLocallySelf)
        //         {
        //             Debug.Log("#MainManager#-------------EnterFocus");
        //             EnterFocus();
        //         }
        //         
        //         // TO BE REFACTORED
        //         if (_localCurrentAssemblyModule.Equals(_accumulator))
        //         {
        //             _videoPlayer.Play();
        //             _videoUI.SetActive(true);
        //             _accumulatorUI.enabled = true;
        //         }
        //     }
        // }

        
        /// <summary>
        ///  ON LOCAL USER UI REQUEST ASSEMBLY TOGGLE
        /// -----------------------------------------
        ///
        /// - Check current system state
        /// - Request ownership
        /// - Set property on network
        /// 
        /// </summary>
        public void OnUIRequestToggleAssembly()
        {
            Debug.Log("#MainManager#-------------OnUIRequestToggleAssembly");
            
            //realtimeView.ClearOwnership();
            //realtimeView.RequestOwnership();

            //model.isAssembled = !model.isAssembled;
        }
        
        // private void OnAssemblyStateChange(RoomStateNCModel roomStateNcModel, bool value)
        // {
        //     Debug.Log("#MainManager#-------------OnAssemblyStateChange: " + value);
        //
        //     if (!realtimeView.isOwnedLocallySelf || _localCurrentAssemblyModule == null) return;
        //     
        //     if(value)
        //         _localCurrentAssemblyModule.Assemble();
        //     else
        //         _localCurrentAssemblyModule.Disassemble();
        // }
        
        private void Update()
        {
            if (!CheckOwnership()) return;

            // If we just reset objects, we should wait before affecting ownership
            if (Time.time - lastOwnershipChangeTime < 1f) return;

            // If object is not being manipulated, or it is at rest, clear ownership
            //realtimeView.ClearOwnership();
        }
        
        private bool CheckOwnership()
        {
            // // If there are any inconsistency update - when focus is toggled on or off
            // if (hasOwnership != realtimeView.isOwnedLocallySelf)
            // {
            //     hasOwnership = realtimeView.isOwnedLocallySelf;
            //     ResetOwnershipTime();
            // }
            return hasOwnership;
        }
        
        private void ResetOwnershipTime()
        {
            lastOwnershipChangeTime = Time.time;
        }


        #region UI EVENT METHODS-------------------------

        public void ChangePositionStep(int step)
        {
            _localCurrentAssemblyModule?.ChangePosition(step);
        }

        public void OnUIToggleVideo()
        {
            if(_videoPlayer.isPlaying)
                _videoPlayer.Pause();
            else
                _videoPlayer.Play();
        }

        #endregion
    }
}
