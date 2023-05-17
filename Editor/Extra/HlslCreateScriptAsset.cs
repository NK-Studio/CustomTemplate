#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace NKStudio
{
    public class HlslCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string text = File.ReadAllText(resourceFile);

            string className = Path.GetFileNameWithoutExtension(pathName);

            //반각 스페이스를 제외
            className = className.Replace(" ", "");

            //다른 파라메터에 대해서 알고 싶다면
            //15장「ScriptTemplates」 을 참조해주세요
            text = text.Replace("#SCRIPTNAME#", className);
            text = text.Replace("#SCRIPTNAME_UPPERCASE#", className.ToUpper());
            
            //UTF8 에 BOM 붙여서 저장
            var encoding = new UTF8Encoding(true, false);

            File.WriteAllText(pathName, text, encoding);

            AssetDatabase.ImportAsset(pathName);
            MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
#endif