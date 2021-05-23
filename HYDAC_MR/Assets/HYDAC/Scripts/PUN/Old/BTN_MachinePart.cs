using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

[RequireComponent(typeof(Button))]
public class BtnMachinePart : MonoBehaviour
{
    [FormerlySerializedAs("m_AssemblyPositionText")] [SerializeField] private TextMeshProUGUI mAssemblyPositionText = null;
    [FormerlySerializedAs("m_PartNameText")] [SerializeField] private TextMeshProUGUI mPartNameText = null;

    private Button _mButton = null;

    private void Awake()
    {
        _mButton = GetComponent<Button>();
    }

    public void Initialize(int assemblyNumber, string partName)
    {
        mAssemblyPositionText.text = assemblyNumber.ToString();
        mPartNameText.text = partName;
    }
}
