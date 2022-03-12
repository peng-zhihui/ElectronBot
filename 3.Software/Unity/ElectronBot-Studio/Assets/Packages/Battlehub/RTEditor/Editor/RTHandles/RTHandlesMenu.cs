#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

using Battlehub.RTCommon;
using Battlehub.Utils;



namespace Battlehub.RTHandles
{
    public static class RTHandlesMenu
    {
        public static GameObject InstantiatePrefab(string name)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(BHRoot.PackageRuntimeContentPath + "/RTHandles/Prefabs/" + name, typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }

        [MenuItem("Tools/Runtime Handles/Create")]
        public static void CreateTransformHandles()
        {
            GameObject go = InstantiatePrefab("TransformHandles.prefab");
           // PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            Undo.RegisterCreatedObjectUndo(go, "Create Transform Handles");
        }

        [MenuItem("Tools/Runtime Handles/Enable Editing", validate = true)]
        private static bool CanEnableEditing()
        {
            return Selection.gameObjects != null 
                && Selection.gameObjects.Length > 0 
                && Selection.gameObjects.Any(g => !g.GetComponent<ExposeToEditor>() && !g.IsPrefab())
                && Object.FindObjectOfType<RuntimeSelectionComponent>();
        }

        [MenuItem("Tools/Runtime Handles/Enable Editing")]
        private static void EnableEditing()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                ExposeToEditor exposeToEditor = go.GetComponent<ExposeToEditor>();
                if (!exposeToEditor && !go.IsPrefab())
                {
                    Undo.RegisterCreatedObjectUndo(go.AddComponent<ExposeToEditor>(), "Enable Object Editing");
                }   
            }
        }

        [MenuItem("Tools/Runtime Handles/Disable Editing", validate = true)]
        private static bool CanDisableEditing()
        {
            return Selection.gameObjects != null
                && Selection.gameObjects.Length > 0
                && Selection.gameObjects.Any(g => g.GetComponent<ExposeToEditor>() && !g.IsPrefab());
        }

        [MenuItem("Tools/Runtime Handles/Disable Editing")]
        private static void DisableEditing()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                ExposeToEditor exposeToEditor = go.GetComponent<ExposeToEditor>();
                if (exposeToEditor && !go.IsPrefab())
                {
                    Undo.DestroyObjectImmediate(exposeToEditor);
                }
            }
        }
    }
}
#endif
