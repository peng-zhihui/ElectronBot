using System.IO;
using UnityEditor;
using UnityEngine;

namespace Battlehub
{
    internal class BHRoot : BHRoot<BHRoot> { }

    public class BHRoot<T> : ScriptableObject where T : BHRoot<T>
    {
#if UNITY_EDITOR
        private static string m_packagePath;
        public static string PackagePath
        {
            get
            {
                if(string.IsNullOrEmpty(m_packagePath))
                {
                    T script = CreateInstance<T>();
                    
                    MonoScript monoScript = MonoScript.FromScriptableObject(script);
                    m_packagePath = AssetDatabase.GetAssetPath(monoScript);
                    m_packagePath = Path.GetDirectoryName(m_packagePath);
                    m_packagePath = Path.GetDirectoryName(m_packagePath);
                    m_packagePath = Path.GetDirectoryName(m_packagePath);

                    DestroyImmediate(script);
                }

                return m_packagePath;
            }
        }

        public static string PackageRuntimeContentPath
        {
            get
            {
                string packagePath = PackagePath;
                return Path.Combine(Path.Combine(packagePath, "Content"), "Runtime");
            }
        }

        public static string PackageEditorContentPath
        {
            get 
            {
                string packagePath = PackagePath;
                return Path.Combine(Path.Combine(packagePath, "Content"), "Editor");
            }
        }

        public static string AssetsPath
        {
            get
            {
                return "Assets/Battlehub";
            }
        }
#endif

        public static readonly string[] Assemblies =
        {
            "Assembly-CSharp",
            "Battlehub.RTEditor",
            "Battlehub.RTExtensions"
        };
    }
}
