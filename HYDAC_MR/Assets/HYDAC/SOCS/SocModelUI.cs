using System;
using UnityEngine;

public class SocModelUI : ScriptableObject
{
    public event Action<string> EUIRequestLoadModel;
    public void OnUIRequestLoadModel(string modelID)
    {
        EUIRequestLoadModel?.Invoke(modelID);
    }
}
