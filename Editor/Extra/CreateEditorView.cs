#if UNITY_EDITOR && USECUSTOMTEMPALTE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NKStudio
{
    public class CreateEditorView : Editor
    {
        [MenuItem("Assets/Create/UI Toolkit/Editor Inspector", priority = 700 + 3)]
        public static void CreateEditorViewScript()
        {
            Object selectedObject = Selection.objects[0];
            string filePath = AssetDatabase.GetAssetPath(selectedObject);
            string className = Path.GetFileNameWithoutExtension(filePath);

            AssetImporter importer = AssetImporter.GetAtPath(filePath);
            if (importer is MonoImporter)
            {
                string csPath = AssetDatabase.GUIDToAssetPath("0d3458fe14fac214097e69fcd91c0a7e");
                string uxmlPath = AssetDatabase.GUIDToAssetPath("ae45bfd12211b5542a9c513f6aecb458");
                string nkUSSPath = AssetDatabase.GUIDToAssetPath("6a25e899d15eb994b85241dddfd90559");
                string odinUSSPath = AssetDatabase.GUIDToAssetPath("8ee2195f99d140f43ae4fd9b456629f9");
                string toggleUSSPath = AssetDatabase.GUIDToAssetPath("2e3e5c2da0c15f741a97092246c349e6");

                string directoryPath = Path.GetDirectoryName(filePath); // 파일이 위치한 디렉토리 경로를 가져옴

                CreateEditorFolder(directoryPath);

                Action($"{directoryPath}/Editor/{className}.cs", csPath,
                    $"{directoryPath}/Editor/{className}.uxml",
                    uxmlPath, nkUSSPath, odinUSSPath, toggleUSSPath);
            }
            else
                Debug.LogWarning(Application.systemLanguage == SystemLanguage.Korean
                    ? "선택한 파일은 C# 스크립트 파일이 아닙니다."
                    : "It is not a C# script file.");
        }

        private static void Action(string csPathName, string resourceFile, string uxmlPathName,
            string uxmlResourceFile, string nkUSSPath, string odinUSSPath, string toggleUSSPath)
        {
            string csText = File.ReadAllText(resourceFile);
            string uxmlText = File.ReadAllText(uxmlResourceFile);

            string className = Path.GetFileNameWithoutExtension(csPathName);

            // 반각 스페이스를 제외
            string changeFileName = $"{className}Editor.cs";
            string directoryPath = Path.GetDirectoryName(csPathName); // 파일이 위치한 디렉토리 경로를 가져옴

            if (directoryPath != null)
                csPathName = Path.Combine(directoryPath, changeFileName); // 새로운 파일 경로 생성

            csText = csText.Replace("#SCRIPTNAME#", className);
            csText = csText.Replace("#scriptname#", ChangeCamelCase(className));

            // 네임스페이스가 있는지 파악합니다.
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == className);

            csText = type != null && !string.IsNullOrWhiteSpace(type.Namespace)
                ? csText.Replace("#Namespace#", type.Namespace)
                : UpdateStringSkipRow(csText, "using #Namespace#;");

            // UTF8 에 BOM 붙여서 저장
            UTF8Encoding encoding = new UTF8Encoding(true, false);

            // 커스텀 Editor C#파일을 생성합니다.
            File.WriteAllText(csPathName, csText, encoding);

            // USS를 연결해줍니다.
            TryUSSPathToGuidAndFileID(nkUSSPath, out string nkUSSGuid, out long nkUSSFileID);
            TryUSSPathToGuidAndFileID(odinUSSPath, out string odinUSSGuid, out long odinUSSFileID);
            TryUSSPathToGuidAndFileID(toggleUSSPath, out string toggleUSSGuid, out long toggleUSSFileID);

            uxmlText = uxmlText.Replace("#NKUSSPath#", nkUSSPath);
            uxmlText = uxmlText.Replace("#NKUSSFileID#", nkUSSFileID.ToString());
            uxmlText = uxmlText.Replace("#NKUSSGUID#", nkUSSGuid);

            uxmlText = uxmlText.Replace("#OdinUSSPath#", nkUSSPath);
            uxmlText = uxmlText.Replace("#OdinUSSFileID#", odinUSSFileID.ToString());
            uxmlText = uxmlText.Replace("#OdinUSSGUID#", odinUSSGuid);

            uxmlText = uxmlText.Replace("#ToggleUSSPath#", nkUSSPath);
            uxmlText = uxmlText.Replace("#ToggleUSSFileID#", toggleUSSFileID.ToString());
            uxmlText = uxmlText.Replace("#ToggleUSSGUID#", toggleUSSGuid);

            // UXML을 생성합니다.
            File.WriteAllText(uxmlPathName, uxmlText, encoding);

            // 유니티에 생성한 파일들을 불러옵니다.
            AssetDatabase.ImportAsset(csPathName);
            AssetDatabase.ImportAsset(uxmlPathName);

            // 커스텀 에디터 C#파일에 VisualTree를 기본으로 베이스로 연결해줍니다.
            MonoImporter monoImporter = AssetImporter.GetAtPath(csPathName) as MonoImporter;
            Object uxml = AssetDatabase.LoadAssetAtPath<Object>(uxmlPathName);
            var names = new string[] { "TreeAsset" };
            var values = new Object[] { uxml };
            if (monoImporter != null)
                monoImporter.SetDefaultReferences(names, values);

            // 다시 임포트 해줍니다.
            AssetDatabase.ImportAsset(csPathName);

            // 타겟팅합니다.
            MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(csPathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }

        /// <summary>
        /// Get the GUID and FileID of the USS file.
        /// </summary>
        private static void TryUSSPathToGuidAndFileID(string ussPath, out string guid, out long fileID)
        {
            string findGuid = AssetDatabase.AssetPathToGUID(ussPath);
            string assetFilePath = AssetDatabase.GUIDToAssetPath(findGuid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetFilePath);

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out fileID);
        }

        /// <summary>
        /// Create the Editor folder if it does not exist in that path.
        /// </summary>
        private static void CreateEditorFolder(string directoryPath)
        {
            if (!Directory.Exists($"{directoryPath}/Editor"))
                Directory.CreateDirectory($"{directoryPath}/Editor");
        }

        /// <summary>
        /// Change it to a camel style name.
        /// </summary>
        /// <returns></returns>
        private static string ChangeCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return $"_{char.ToLower(str[0]) + str[1..]}";
        }

        /// <summary>
        /// Delete the row with that keyword.
        /// </summary>
        /// <returns></returns>
        private static string UpdateStringSkipRow(string sources, string key)
        {
            string[] lines = sources.Split('\n');
            int pos = Array.FindIndex(lines, row => row.Contains(key));
            if (pos < 0) return sources;
            List<string> list = new(lines);
            list.RemoveAt(pos);
            return string.Join("\n", list.ToArray());
        }
    }
}
#endif