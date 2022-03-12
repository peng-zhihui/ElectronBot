#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Battlehub.Utils
{
    [CustomEditor(typeof(ObjectToTexture))]
    public class ObjectImageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ObjectToTexture t = (ObjectToTexture)target;
            t.objectImageLayer = EditorGUILayout.LayerField("Object Image Layer", t.objectImageLayer);

            if (GUI.changed)
                EditorUtility.SetDirty(target);

            DrawDefaultInspector();
        }

    }
}
#endif
