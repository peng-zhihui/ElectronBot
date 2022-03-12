using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI.ProceduralImage;

[CustomEditor(typeof(FreeModifier), true)]
[CanEditMultipleObjects]
public class FreeModifierEditor : Editor
{
    protected SerializedProperty radiusX;
    protected SerializedProperty radiusY;
    protected SerializedProperty radiusZ;
    protected SerializedProperty radiusW;

    protected void OnEnable()
    {
        radiusX = serializedObject.FindProperty("radius.x");
        radiusY = serializedObject.FindProperty("radius.y");
        radiusZ = serializedObject.FindProperty("radius.z");
        radiusW = serializedObject.FindProperty("radius.w");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.Space(8);
        RadiusGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected void RadiusGUI()
    {
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PropertyField(radiusX, new GUIContent("Upper Left"));
            EditorGUILayout.PropertyField(radiusY, new GUIContent("Upper Right"));
            GUILayout.Space(8);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        {
            EditorGUILayout.PropertyField(radiusW, new GUIContent("Lower Left"));
            EditorGUILayout.PropertyField(radiusZ, new GUIContent("Lower Right"));
            GUILayout.Space(8);
        }
        GUILayout.EndHorizontal();
    }
}
