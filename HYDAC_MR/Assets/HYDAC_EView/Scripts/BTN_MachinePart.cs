using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BTN_MachinePart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_AssemblyPositionText = null;
    [SerializeField] private TextMeshProUGUI m_PartNameText = null;

    private Button m_button = null;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    public void Initialize(int _assemblyNumber, string _partName)
    {
        m_AssemblyPositionText.text = _assemblyNumber.ToString();
        m_PartNameText.text = _partName;
    }
}
