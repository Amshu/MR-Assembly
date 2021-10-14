using HYDAC.Scripts.INFO;
using HYDAC.Scripts.MOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBtnSubModule : MonoBehaviour
{
    [SerializeField] private SocAssemblyUI assemblyUI;
    [SerializeField] private TextMeshProUGUI subModuleTitleTxt;
    [SerializeField] private Button button;

    private SSubModuleInfo _info; 

    public void Intitialise(SSubModuleInfo info)
    {
        _info = info;

        // Set name
        subModuleTitleTxt.text = string.Format("{0} - {1}", _info.ID.ToString("D2"), _info.iname);

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        assemblyUI.InvokeUISubModuleSelect(_info);
    }
}
