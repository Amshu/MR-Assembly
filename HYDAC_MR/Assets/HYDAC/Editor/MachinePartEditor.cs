using System;
using HYDAC.Scripts.MOD;
using MAC;
using UnityEngine;
using UnityEditor;

namespace HYDAC_EView.Editor
{
    [CustomEditor(typeof(SubModule))]
    public class MachinePartEditor : UnityEditor.Editor
    {
        public string partInfo = "Part Description:";
        public string partAssemblyPosition = "00";
        
        private const string MachinePartInfosPath = "Assets/Resources/MachinePartInfos/";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Use the below GUI create a MachinePartInfo if not created");
            
            SubModule myScript = (SubModule)target;

            partAssemblyPosition = GUILayout.TextField(partAssemblyPosition, 2);
            partInfo = GUILayout.TextArea(partInfo, 500);
            


            if (GUILayout.Button("Create Part Info"))
            {
                SSubModule info = ScriptableObject.CreateInstance<SSubModule>();

                info.unitPosition = Convert.ToInt32(partAssemblyPosition);
                info.partName = myScript.gameObject.name;
                info.partInfo = partInfo;
                
                EditorUtility.SetDirty(info);

                string fileName = info.unitPosition + "_INFO_" + info.partName + ".asset";
                string fileURL = MachinePartInfosPath + fileName;
                
                AssetDatabase.CreateAsset(info, fileURL);

                myScript.SetPartInfo(info);
            }
        }
    }
}