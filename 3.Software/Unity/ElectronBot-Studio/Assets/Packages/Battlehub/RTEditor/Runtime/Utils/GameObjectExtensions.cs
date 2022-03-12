using UnityEngine;

namespace Battlehub.Utils
{
    public static class GameObjectExtensions
    {
        public static bool IsPrefab(this GameObject go)
        {   
            if(go == null)
            {
                return false;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                #if UNITY_EDITOR
                UnityEditor.PrefabAssetType assetType = UnityEditor.PrefabUtility.GetPrefabAssetType(go);
                return assetType == UnityEditor.PrefabAssetType.Regular || assetType == UnityEditor.PrefabAssetType.Model;
                #else
                throw new System.InvalidOperationException("Does not work in edit mode");
                #endif
            }
            return go.scene.buildIndex < 0 && go.scene.path == null;
        }

        public static Bounds CalculateBounds(this GameObject g)
        {
            return g.transform.CalculateBounds();
        }
    }

}
