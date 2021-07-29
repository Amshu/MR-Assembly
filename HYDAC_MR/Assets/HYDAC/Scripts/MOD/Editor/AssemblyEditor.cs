using System;
using UnityEditor;
using UnityEngine;

namespace HYDAC.Scripts.MOD.Editor
{
    [CustomEditor(typeof(BaseAssembly))]
    public class AssemblyEditor : UnityEditor.Editor
    {
        public string ID = "ID:";
        public string Description = "Description:";
        
        private DefaultAsset _folderToSaveTo = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.Space(20);
            
            GUILayout.Label("Use the below GUI create AInfo if not created");
            
            BaseAssembly myScript = (BaseAssembly)target;

            ID = GUILayout.TextField(ID);
            Description = GUILayout.TextArea(Description, 500);
            
            OnGUI();

            if (GUILayout.Button("Create Assembly Info"))
            {
                SAssemblyInfo info = ScriptableObject.CreateInstance<SAssemblyInfo>();

                info.ID = Convert.ToInt32(ID);
                info.iname = myScript.gameObject.name;
                info.description = Description;
                
                EditorUtility.SetDirty(info);

                string fileName = "AInfo_" + info.ID + "HYDAC_" + info.iname + ".asset";
                string folderURL = AssetDatabase.GetAssetPath(_folderToSaveTo.GetInstanceID()) + "/" + _folderToSaveTo.name;
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