using System;
using UnityEditor;
using UnityEngine;

using HYDAC.Scripts.INFO;

namespace HYDAC.Scripts.MOD
{
    [CustomEditor(typeof(FocusedModule))]
    public class ModuleEditor : UnityEditor.Editor
    {
        public Texture HydacLogo;
        [TextArea]
        public string Documentation;

        private DefaultAsset _folderToSaveTo = null;
        private FocusedModule myScript;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            myScript = (FocusedModule)target;

            GUILayout.Label(new GUIContent(HydacLogo, Documentation));
            GUILayout.Label(new GUIContent("DEBUG CONTROLS\n=============="));

            ModuleIntialization();

            GUILayout.Space(20);

            if (GUILayout.Button("\nEXPLODE\n-------\n"))
            {
                myScript.ToggleExplosion(true);
            }
            if (GUILayout.Button("\nIMPLODE\n-------\n"))
            {
                myScript.ToggleExplosion(false);
            }
        }
        
        
        [MenuItem ("Window/Folder Selection Example")]
        public static void  ShowWindow () 
        {
            EditorWindow.GetWindow(typeof(ASInfo));
        }
     
        private void ModuleIntialization() 
        {
            if (_folderToSaveTo == null)
            {
                EditorGUILayout.HelpBox(
                    "Folder not Specified!", 
                    MessageType.Error, 
                    true);
            }

            _folderToSaveTo = (DefaultAsset)EditorGUILayout.ObjectField(
                "Save Location:",
                _folderToSaveTo,
                typeof(DefaultAsset),
                false);
            
            if (_folderToSaveTo == null) return;

            GUILayout.Space(5);

            InitializeModule();

            UpdateSubModule();
        }

        private void InitializeModule()
        {
            if (GUILayout.Button("\nINITIALISE MODULE\n-----------------\n"))
            {
                if (myScript.Info != null) return;

                // Create Module Info
                SModuleInfo modInfo = ScriptableObject.CreateInstance<SModuleInfo>();

                modInfo.ID = Convert.ToInt32(myScript.transform.name.Substring(0, 2));
                modInfo.iname = myScript.name.Substring(3);
                modInfo.description = "Module Description";

                //EditorUtility.SetDirty(modInfo);

                string fileName = "MInfo_" + modInfo.ID + "_" + modInfo.iname + ".asset";
                modInfo.name = fileName;

                string folderURL = AssetDatabase.GetAssetPath(_folderToSaveTo.GetInstanceID());
                string fileURL = folderURL + "/" + fileName;

                AssetDatabase.CreateAsset(modInfo, fileURL);

                myScript.SetPartInfo(modInfo);
            }
        }

        private void UpdateSubModule()
        {
            if (GUILayout.Button("\nUPDATE MODULE\n-------------\n"))
            {
                if (!myScript.UpdateSubModules()) return;

                for (int i = 0; i < myScript.SubModulesCount; i++)
                {
                    var subModTransform = myScript.RootTransform.GetChild(i);
                    var subModule = myScript.SubModules[i];

                    int id;
                    string name;
                    SSubModuleInfo subModInfo;

                    // Get id of submodule
                    try
                    {
                        id = Convert.ToInt32(subModTransform.name.Substring(0, 2));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("FocusedModuleEditor#--------Error: " + subModTransform.name + " - ID");
                        return;
                    }

                    // Get name of submodule
                    name = subModTransform.name.Substring(3);

                    subModInfo = ScriptableObject.CreateInstance<SSubModuleInfo>();
                    subModInfo.ID = id;
                    subModInfo.iname = name;

                    // Check if submodule info is assigned
                    if (subModule.Info == null)
                    {
                        subModInfo.description = "To be filled later";
                    }
                    else
                    {
                        // Copy description of the sub module info
                        subModInfo.description = subModule.Info.description;

                        // Destroy and delete previous one if already present
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(subModule.Info.GetInstanceID()));
                    }

                    // Save submodule info to location and set property
                    string fileName = "SInfo_" + subModInfo.ID + "_" + subModInfo.iname + ".asset";
                    subModInfo.name = fileName;

                    string folderURL = AssetDatabase.GetAssetPath(_folderToSaveTo.GetInstanceID());
                    string fileURL = folderURL + "/" + fileName;

                    AssetDatabase.CreateAsset(subModInfo, fileURL);

                    subModule.SetPartInfo(subModInfo);
                }
            }
        }
    }
}