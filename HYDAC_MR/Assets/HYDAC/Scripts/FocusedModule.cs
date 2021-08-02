using System;
using HYDAC.Scripts.MOD;
using HYDAC.SOCS;
using UnityEngine;

public class FocusedModule : MonoBehaviour
{
    [SerializeField] private SocModuleUI socUI = null;
    [SerializeField] private SubModule[] subModules;

    private bool _isAssembled = true;

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
}
