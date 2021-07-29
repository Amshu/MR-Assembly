using System;
using UnityEditor;
using UnityEngine;

namespace HYDAC.Scripts.MOD.Editor
{
    [CustomEditor(typeof(SubModule))]
    public class SubModuleEditor : UnityEditor.Editor
    {
        public string ID = "ID:";
        public string Description = "Description:";
        
        private DefaultAsset _folderToSaveTo = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Use the below GUI create a SInfo if not created");
            
            SubModule myScript = (SubModule)target;

            ID = GUILayout.TextField(ID, 2);
            Description = GUILayout.TextArea(Description, 500);
            
            OnGUI();

            if (GUILayout.Button("Create SubModule Info"))
            {
                SSubModuleInfo info = ScriptableObject.CreateInstance<SSubModuleInfo>();

                info.ID = Convert.ToInt32(ID);
                info.iname = myScript.gameObject.name;
                info.description = Description;
                
                EditorUtility.SetDirty(info);

                string fileName =  "SInfo_" + info.ID + "_" + info.iname + ".asset";
                string folderURL = AssetDatabase.GetAssetPath(_folderToSaveTo.GetInstanceID());
                string fileURL = folderURL + fileName;
                
                AssetDatabase.CreateAsset(info, fileURL);

                myScript.SetPartInfo(info);
            }
        }
        
        
        [MenuItem ("Window/Folder Selection Example")]
        public static void  ShowWindow () 
        {
            EditorWindow.GetWindow(typeof(ASInfo));
        }
     
        void OnGUI () 
        {
            _folderToSaveTo = (DefaultAsset)EditorGUILayout.ObjectField(
                "Folder to save to", 
                _folderToSaveTo, 
                typeof(DefaultAsset), 
                false);
 
            if (_folderToSaveTo != null) {
                EditorGUILayout.HelpBox(
                    "Valid folder! Name: " + _folderToSaveTo.name, 
                    MessageType.Info, 
                    true);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Not valid!", 
                    MessageType.Warning, 
                    true);
            }
        }
    }
}