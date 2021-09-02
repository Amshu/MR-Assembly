using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HYDAC.Scripts.NET.Editor
{
    [CustomEditor(typeof(SocNetSettings))]
    public class NetSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SocNetSettings myScript = (SocNetSettings)target;
        }
    }
}