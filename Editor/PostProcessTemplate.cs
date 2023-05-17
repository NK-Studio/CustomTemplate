#if UNITY_EDITOR && HaveCore
using UnityEditor;

namespace NKStudio
{
    public static class PostProcessTemplate
    {
#if UNITY_PIPELINE_URP
        [MenuItem("Assets/Create/Shader/C# Script (Custom Volume)")]
        private static void CreateCustomVolume()
        {
            string path = AssetDatabase.GUIDToAssetPath("65ad06bbd91a44c44a4f1d14e83c8ad6");
            CreateAssetByTemplate.CreateScriptAsset(path, "DefaultNewVolumeScriptName.cs");
        }
        
        [MenuItem("Assets/Create/Shader/Post Process (URP) Shader")]
        private static void CreateCustomVolumeShader()
        {
            string path = AssetDatabase.GUIDToAssetPath("1c72d4b7f333a204f88f92aa19655700");
            CreateAssetByTemplate.CreateScriptAsset(path, "NewPostProcessShader.shader");
        }
        
#endif
    }
}
#endif