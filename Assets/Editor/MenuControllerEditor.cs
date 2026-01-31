using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuController))]
public class MenuControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var controller = (MenuController)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Recargar Play Hover"))
        {
            controller.RefreshPlayHoverFromEditor();
            EditorUtility.SetDirty(controller);
        }

        if (GUILayout.Button("Debug Play Hover"))
        {
            controller.DebugPlayHoverSetup();
        }
    }
}
