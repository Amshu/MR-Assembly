using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SocAssemblySettings : MonoBehaviour
{
    [SerializeField] private AssetReference focusedModule;
    private AssetReference FocusedModule => focusedModule;

    [SerializeField] private AssetReference[] uiObjects;
    private AssetReference[] UIObjects => UIObjects;
}
