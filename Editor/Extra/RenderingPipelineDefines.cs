#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;

namespace NKStudio
{
    [InitializeOnLoad]
    public class RenderingPipelineDefines
    {
        private enum PipelineType
        {
            Unsupported,
            BuiltInPipeline,
            UniversalPipeline,
            HighDefinitionPipeline
        }

        static RenderingPipelineDefines() => 
            UpdateDefines();

        /// <summary>
        /// Update the unity pipeline defines for URP
        /// </summary>
        private static void UpdateDefines()
        {
            PipelineType pipeline = GetPipeline();

            if (pipeline == PipelineType.UniversalPipeline)
                AddDefine("UNITY_PIPELINE_URP");
            else
                RemoveDefine("UNITY_PIPELINE_URP");
            if (pipeline == PipelineType.HighDefinitionPipeline)
                AddDefine("UNITY_PIPELINE_HDRP");
            else
                RemoveDefine("UNITY_PIPELINE_HDRP");
        }


        /// <summary>
        /// Returns the type of renderpipeline that is currently running
        /// </summary>
        /// <returns></returns>
        private static PipelineType GetPipeline()
        {
#if UNITY_2019_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset == null)
                return PipelineType.BuiltInPipeline;
        
            // SRP
            string srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
            
            if (srpType.Contains("HDRenderPipelineAsset"))
                return PipelineType.HighDefinitionPipeline;

            if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
                return PipelineType.UniversalPipeline;
            
            return PipelineType.Unsupported;
#elif UNITY_2017_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null) {
            // SRP not supported before 2019
            return PipelineType.Unsupported;
        }
#endif
            // no SRP
        }

        /// <summary>
        /// Add a custom define
        /// </summary>
        /// <param name="define"></param>
        private static void AddDefine(string define)
        {
            List<string> definesList = GetDefines();
            if (definesList.Contains(define)) return;
        
            definesList.Add(define);
            SetDefines(definesList);
        }

        /// <summary>
        /// Remove a custom define
        /// </summary>
        private static void RemoveDefine(string define)
        {
            List<string> definesList = GetDefines();
            if (!definesList.Contains(define)) return;
            definesList.Remove(define);
            SetDefines(definesList);
        }

        private static List<string> GetDefines()
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Split(';').ToList();
        }

        private static void SetDefines(List<string> definesList)
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            string defines = string.Join(";", definesList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }
    }
}
#endif