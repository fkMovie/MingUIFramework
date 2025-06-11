using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using LitJson;


public class GeneratorUITool : Editor
{
    static Dictionary<string, string> methodDic = new Dictionary<string, string>();
    [MenuItem("GameObject/UI工具/生成UI表现层脚本(shift+V) #V", false, 1)]
    static void GreateUIScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;
        if (obj == null)
        {
            Debug.Log("<Color=Yellow> 需要选择GameOject </Color>");
            return;
        }

        if (!Directory.Exists(GeneratorConfig.uiGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.uiGeneratorPath);
        }

        string csContent = CreateCS(obj.name);
        Debug.Log($"csContent:\n{csContent}");
        string csPath = $"{GeneratorConfig.uiGeneratorPath}/{obj.name}.cs";
        UICodePreviewWindow.ShowWindow(csContent,csPath,methodDic);
        
    }

    static string CreateCS(string name)
    {
        string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        List<EditorObjectData> objDataList = JsonMapper.ToObject<List<EditorObjectData>>(dataListJson);
        methodDic.Clear();
        string nameSpace =GeneratorConfig.UI_NAMESPACE;
        string template = File.ReadAllText(GeneratorConfig.uiTemplatePath);
        StringBuilder sb = new StringBuilder(template);
        StringBuilder tmp = new StringBuilder();

        tmp.Clear();
        sb.Replace("#CREATEDATE#", System.DateTime.Now.ToString());
        if(!string.IsNullOrEmpty(nameSpace))
        {
            tmp.AppendLine($"namespace {nameSpace}");
            tmp.AppendLine("{");
        }
        
        sb.Replace("#NameSpaceStart#", tmp.ToString());
        sb.Replace("#ScriptName#", name);
        
        //替换获取ui组建类
        tmp.Clear();
        if (GeneratorConfig.eGenType == EnumGeneratorType.Bind)
        {
            tmp.AppendLine($"public {name}DataComponent uicompt ;");
        }
        else
        {
            tmp.AppendLine($"public {name}Component uicompt = new {name}Component();");
        }
        sb.Replace("#获取UI组件类#",tmp.ToString());

        sb.Replace("#UI组件类GetComponent#", GeneratorConfig.eGenType == EnumGeneratorType.Bind ?
            $"uicompt= gameObject.GetComponent<{name}DataComponent>();":"");

        tmp.Clear();
        foreach (var item in objDataList)
        {
            string type = item.fileType;
            string methodName;
            if (type.Contains("Button"))
            {
                methodName = $"On{item.fileName}ButtonClick";
                CreateBindMethod(tmp, ref methodDic, methodName);
            }
            else if (type.Contains("Toggle"))
            {
                methodName = $"On{item.fileName}ToggleChange";
                CreateBindMethod(tmp, ref methodDic, methodName,"bool isOn,Toggle toggle");
            }
            else if (type.Contains("InputField"))
            {
                methodName = $"On{item.fileName}InputChange";
                CreateBindMethod(tmp, ref methodDic, methodName,"string text");
                methodName = $"On{item.fileName}InputEnd";
                CreateBindMethod(tmp, ref methodDic, methodName,"string text");
            }
        }
        
        sb.Replace("#ComponentMethod#",tmp.ToString());
        sb.Replace("#NameSpaceEnd#", !string.IsNullOrEmpty(nameSpace)?"}":"");

        return sb.ToString();
    }

    static string CreateBindMethod(StringBuilder _sb, ref Dictionary<string, string> methodDic, 
        string _methodName, string parmas = null)
    {
        //声明周期
        // _sb.AppendLine($"\t\tpublic void {_methodName}({parmas})");
        // _sb.AppendLine("\t\t{");
        // if (_methodName == "OnCloseButtonClick")
        // {
        //     _sb.AppendLine("\t\t\tHideSelf();");
        // }
        // _sb.AppendLine("\t\t");
        // _sb.AppendLine("\t\t}");
        //
        // //存储UI组件事件，提供给后续新增代码使用
        // StringBuilder builder = new StringBuilder();
        // builder.AppendLine($"\t\tpublic void {_methodName}({parmas})");
        // builder.AppendLine("\t\t{");
        // builder.AppendLine("\t\t");
        // builder.AppendLine("\t\t}");
        // methodDic.Add(_methodName, builder.ToString());
        //
        // return _sb.ToString();
        
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"\tpublic void {_methodName}({parmas})");
        builder.AppendLine("\t{");
        if (_methodName == "OnCloseButtonClick")
        {
            builder.AppendLine("\t HideSelf();");
        }
        builder.AppendLine("\t\t");
        builder.AppendLine("\t}");
        methodDic.Add(_methodName, builder.ToString());
        _sb.AppendLine();
        _sb.Append(builder);
        return _sb.ToString();

    }
}