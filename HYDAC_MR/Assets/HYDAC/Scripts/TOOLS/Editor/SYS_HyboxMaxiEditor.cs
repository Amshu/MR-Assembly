using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SYS_HyboxMaxi))]
public class SYS_HyboxMaxiEditor : Editor
{
    public Texture HydacLogo;
    [TextArea]
    public string Documentation;

    private SYS_HyboxMaxi myScript;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        myScript = (SYS_HyboxMaxi)target;

        GUILayout.Space(20);

        GUILayout.Label(new GUIContent(HydacLogo, Documentation));
        GUILayout.Label(new GUIContent("DEBUG CONTROLS\n=============="));

        GUILayout.Space(20);

        if (GUILayout.Button("\nON\n-------\n"))
        {
            myScript.PowerOnMachine(true);
        }
        if (GUILayout.Button("\nOFF\n-------\n"))
        {
            myScript.PowerOnMachine(false);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("\nEXTEND\n-------\n"))
        {
            //myScript.Extend();
        }
        if (GUILayout.Button("\nRETRACT\n-------\n"))
        {
            //myScript.Retract();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("\nRAISE\n-------\n"))
        {
            //myScript.Raise();
        }
        if (GUILayout.Button("\nLOWER\n-------\n"))
        {
            //myScript.Lower();
        }
    }
}
