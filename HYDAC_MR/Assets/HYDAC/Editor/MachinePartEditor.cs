using System;

using UnityEngine;
using UnityEditor;

using HYDAC.Scripts.MOD;

namespace HYDAC_EView.Editor
{
    [CustomEditor(typeof(SubModule))]
    public class MachinePartEditor : UnityEditor.Editor
    {
        public string partInfo = "Part Description:";
        public string partAssemblyPosition = "00";
        
        private const string SubmoduleInfosPath = "Assets/Resources/SubModuleInfos";

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

                info.assemblyPosition = Convert.ToInt32(partAssemblyPosition);
                info.partName = myScript.gameObject.name;
                info.partInfo = partInfo;
                
                EditorUtility.SetDirty(info);


                string parentName = myScript.gameObject.transform.parent.name;
                string directoryPath = SubmoduleInfosPath + "/" + parentName;

                // Check if directory doesn't exit, if not then create it
                if (!AssetDatabase.IsValidFolder(directoryPath))
                {
                    string guid = AssetDatabase.CreateFolder(SubmoduleInfosPath, parentName);
                    string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
                    
                    //directoryPath = AssetDatabase.CreateFolder(SubmoduleInfosPath, parentName);
                    
                    Debug.Log("Parent directory does not exist, created folder at: " + newFolderPath);
                }

                string fileName = info.assemblyPosition + "_INFO_" + info.partName + ".asset";
                string fileURL = directoryPath + "/"+ fileName;
                
                AssetDatabase.CreateAsset(info, fileURL);

                myScript.SetPartInfo(info);
            }
        }
    }
}