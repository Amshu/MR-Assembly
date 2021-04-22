using System;
using UnityEngine;
using UnityEditor;
using HYDAC_EView.Scripts.MPart;

namespace HYDAC_EView.Editor
{
    [CustomEditor(typeof(MachinePart))]
    public class MachinePartEditor : UnityEditor.Editor
    {
        public string partName = "Part Name:";
        public string partInfo = "Part Description:";
        public string partAssemblyPosition = "00";
        
        const string Path = "Assets/HYDAC_EView/Resources/MachinePartInfos/";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Use the below GUI create a MachinePartInfo if not created");
            
            MachinePart myScript = (MachinePart)target;

            partAssemblyPosition = GUILayout.TextField(partAssemblyPosition, 2);
            partName = GUILayout.TextField(partName, 50);
            partInfo = GUILayout.TextArea(partInfo, 500);
            
            if (GUILayout.Button("Create Part Info"))
            {
                SocMachinePartInfo info = ScriptableObject.CreateInstance<SocMachinePartInfo>();

                info.assemblyPosition = Convert.ToInt32(partAssemblyPosition);
                info.partName = partName;
                info.partInfo = partInfo;
                
                EditorUtility.SetDirty(info);

                string fileName = partAssemblyPosition + "_INFO_" + partName + ".asset";
                string fileURL = Path + fileName;
                
                AssetDatabase.CreateAsset(info, fileURL);

                myScript.SetPartInfo(info);
            }
        }
    }
}