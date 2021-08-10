using System;
using UnityEditor;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD.Editor
{
    [CustomEditor(typeof(FocusedModule))]
    public class ModuleEditor : UnityEditor.Editor
    {
        public string ID = "ID:";
        public string Description = "Description:";
        
        private DefaultAsset _folderToSaveTo = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            FocusedModule myScript = (FocusedModule)target;

            if (myScript.Info != null)
                return;
            
            GUILayout.Space(20);
            
            GUILayout.Label("Use the below GUI create a MInfo if not created");

            ID = GUILayout.TextField(ID, 2);
            Description = GUILayout.TextArea(Description, 500);
            
            OnGUI();

            if (GUILayout.Button("Create Module Info"))
            {
                SModuleInfo info = ScriptableObject.CreateInstance<SModuleInfo>();

                info.ID = Convert.ToInt32(ID);
                info.iname = myScript.gameObject.name;
                info.description = Description;
                
                EditorUtility.SetDirty(info);

                string fileName = "MInfo_" + info.ID + "_" + info.iname + ".asset";
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