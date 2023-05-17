#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public class CreateAssetByTemplate
    {
        #region SRP

#if UNITY_PIPELINE_URP || UNITY_PIPELINE_HDRP
        [MenuItem("Assets/Create/Shader Graph/HLSL Function")]
        private static void CreateHLSL()
        {
            Texture2D cgIcon =
                EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;

            HlslCreateScriptAsset endNameEditAction =
                ScriptableObject.CreateInstance<HlslCreateScriptAsset>();

            string path = AssetDatabase.GUIDToAssetPath("0ed59b28d34eaa341982d19d2865cb2f");
        
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction,
                "DefaultNewHLSLName.hlsl", cgIcon, path);
        }
#endif

        #endregion

        #region URP

#if UNITY_PIPELINE_URP
        [MenuItem("Assets/Create/Shader/Unlit (URP) Shader", priority = 84)]
        private static void CreateURPUnlit()
        {
            string path = AssetDatabase.GUIDToAssetPath("25a604543cd6b124988f666b50b12a06");
            CreateScriptAsset(path, "NewUnlitShader.shader");
        }
#endif

        #endregion

        [MenuItem("Assets/Create/C# Script (SO)", priority = 80)]
        private static void CreateSOScript()
        {
            string path = AssetDatabase.GUIDToAssetPath("02e9f3b7e4db83249bb3b60ea3362e9f");
            CreateScriptAsset(path, "DefaultNewSOScriptName.cs");
        }

        // Example
        // [MenuItem("Assets/Create/Name")]
        // private static void CreateName()
        // {
        //     string path = "Assets/../file.txt";
        //     CreateScriptAsset(path, "DefaultNewSOScriptName.cs");
        // }
        
        #region Core

        public static void CreateScriptAsset(string templatePath, string destName)
        {
#if UNITY_2019_1_OR_NEWER
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, destName);
#else
	typeof(UnityEditor.ProjectWindowUtil)
		.GetMethod("CreateScriptAsset", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
		.Invoke(null, new object[] { templatePath, destName });
#endif
        }

        #endregion
    }
}
#endif