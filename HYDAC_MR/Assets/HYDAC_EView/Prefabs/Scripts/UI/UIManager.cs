using UnityEngine;
using OculusSampleFramework;

namespace HYDAC_EView.Scripts.UI
{
    [RequireComponent(typeof(MainManager))]
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private string selectableTag = "MachinePart";

        private MainManager _mMainManager = null;

        private void Awake()
        {
            _mMainManager = GetComponent<MainManager>();
        }

        public void OnImplodeAll(InteractableStateArgs obj)
        {
            //if (obj.NewInteractableState == InteractableState.ActionState)
                //_mMainManager.ChangeCurrentAssemblyPosition(0);
        }
    
        public void OnExplodeAll(InteractableStateArgs obj)
        {
            //if (obj.NewInteractableState == InteractableState.ActionState)
               // _mMainManager.ChangeCurrentAssemblyPosition(_mMainManager.NoOfAssemblies);
        }
        
        // private void CreateUIButtons(IMachinePart[] parts)
        // {
        //     foreach (var part in parts)
        //     {
        //         var temp = Instantiate(mPartButtonPrefab, mPartButtonParent);
        //         temp.GetComponent<BTN_MachinePart>().Initialize(part.GetAssemblyPosition(), part.GetPartName());
        //     }
        // }
        
        // private void Update()
        // {
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         if (Camera.main is null) return;
        //         var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //
        //         if (!Physics.Raycast(ray, out var hit)) return;
        //         
        //         var selection = hit.transform;
        //         if (selection.CompareTag(selectableTag))
        //         {
        //             _mCurrentSelectedPart = selection.GetComponent<MachinePart>();
        //             //m_TimelineSlider.value = m_currentSelectedPart.PartInfo.AssemblyPosition;
        //         }
        //     }
        // }
    }        
}
