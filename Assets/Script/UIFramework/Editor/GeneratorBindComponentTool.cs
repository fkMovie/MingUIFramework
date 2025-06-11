using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class GeneratorBindComponentTool : Editor
{

    /// <summary>
    /// 查找对象的数据
    /// </summary>
    private static List<EditorObjectData> m_objDataList;

    [MenuItem("GameObject/UI工具/生成组件数据脚本(shift+B) #B",false,0)]
    static void GreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;
        if (obj == null)
        {
            Debug.Log("<Color=Yellow> 需要选择GameOject </Color>");
            return;
        }
        m_objDataList=new List<EditorObjectData>();

        if (!Directory.Exists(GeneratorConfig.BindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.BindComponentGeneratorPath);
        }
        //解析UI
        if (GeneratorConfig.eParseType == EnumParseType.Name)
            PresUINodeDataByName(obj.transform,obj.name);
        else if (GeneratorConfig.eParseType == EnumParseType.Tag)
            PresUINodeDataByTag(obj.transform,obj.name);
        else
            Debug.LogWarning($"意外的解析类型：{GeneratorConfig.eParseType}");
        
        //储存字段
        string dataListJson=JsonMapper.ToJson(m_objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, dataListJson);
        
       string csContent= CreateCS(obj.name);
       Debug.Log($"csContent:\n{csContent}");
       string csPath = $"{GeneratorConfig.BindComponentGeneratorPath}/{obj.name}DataComponent.cs";
       
        UICodePreviewWindow.ShowWindow(csContent,csPath);  
        EditorPrefs.SetString("GeneratorClassName",$"{obj.name}DataComponent");
        GeneratorConfig.eGenType = EnumGeneratorType.Bind;
        AssetDatabase.Refresh();
        
    } 

    /// <summary>
    /// 通过 名字 解析UI数据
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="uiName"></param>
    public static void PresUINodeDataByName(Transform tran,string uiName)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            GameObject obj = tran.GetChild(i).gameObject;
            string name = obj.name;
            if (name.Contains("[") && name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fileName = name.Substring(index);
                string fileType = name.Substring(1, index - 2);
                m_objDataList.Add(new EditorObjectData()
                {
                    insID = obj.GetInstanceID(), fileName = fileName,
                    fileType = fileType
                });

            }

            PresUINodeDataByName(obj.transform,uiName);
        }
    }
    
    /// <summary>
    /// 通过 tag 解析UI数据
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="uiName"></param>
    public static void PresUINodeDataByTag(Transform tran,string uiName)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            GameObject obj = tran.GetChild(i).gameObject;
            string tag = obj.tag;
            if (GeneratorConfig.TagArr.Contains(tag))
            {
                string fileName = obj.name;
                string fileType = tag;
                m_objDataList.Add(new EditorObjectData()
                {
                    insID = obj.GetInstanceID(), fileName = fileName,
                    fileType = fileType
                });

            }

            PresUINodeDataByTag(obj.transform,uiName);
        }
    }

    /// <summary>
    /// 生成CS脚本
    /// </summary>
    /// <param name="className"></param>
    public static string CreateCS(string className)
    { 
        string nameSpace=GeneratorConfig.UI_NAMESPACE;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("/*------------------------------------");
        sb.AppendLine(" *Title: UI自动化生成脚本工具  ");
        sb.AppendLine(" *Author: ZiMing  ");
        sb.AppendLine($" *Date:  {System.DateTime.Now}");
        sb.AppendLine(" *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成数据组件脚本 ");
        sb.AppendLine(" *注意：以下代码均由脚本生成，任何手动修改将有可能被下次生成覆盖，尽量避免再次生成！");
        sb.AppendLine(" -------------------------------------*/");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using UnityEngine;");    
        sb.AppendLine();

        //生成命名空间
        if (!string.IsNullOrEmpty(nameSpace))
        {
            sb.AppendLine($"namespace {nameSpace}");
            sb.AppendLine("{");
        }

        sb.AppendLine($"\tpublic class {className}DataComponent : MonoBehaviour"); 
        sb.AppendLine("\t{");

        foreach (var obj in m_objDataList)
        {
            sb.AppendLine($"\t\t public {obj.fileType} {obj.fileName}{obj.fileType};\n");
        }

        //初始化组件
        sb.AppendLine("\t\tpublic void InitComponent(UIBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t        //事件绑定");
        //得到逻辑类
        sb.AppendLine($"\t\t        {className} m_UI=({className})target;");
        
        //生成UI事件绑定代码
        foreach (var item in m_objDataList)
        {
            string type=item.fileType;
            string methodName=item.fileName;
            // string suffix = "";
            if (type.Contains("Button"))
            {
                sb.AppendLine($"\t\t        target.AddButtonClickListener({methodName}{type},m_UI.On{methodName}ButtonClick);");
            }
            if (type.Contains("InputField"))
            {
                sb.AppendLine($"\t\t        target.AddInputFieldListener({methodName}{type},m_UI.On{methodName}InputChange,m_UI.On{methodName}InputEnd);");
            }
            if (type.Contains("Toggle"))
            {
                sb.AppendLine($"\t\t        target.AddToggleClickListener({methodName}{type},m_UI.On{methodName}ToggleChange);");
            }
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        if (!string.IsNullOrEmpty(nameSpace))
        {
            sb.AppendLine("}");
        }

       return  sb.ToString();
    }

    public static EditorObjectData GetEditorObjectData(int insId)
    {
        foreach (var obj in m_objDataList)
        {
            if (obj.insID == insId)
            {
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// 编辑脚本后挂载组件
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void BindComponentInUI()
    {
        string className = EditorPrefs.GetString("GeneratorClassName");
        if (string.IsNullOrEmpty(className)) return;
        EditorPrefs.DeleteKey("GeneratorClassName");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Assembly csAssemly= assemblies.First(assemly=>assemly.GetName().Name=="Assembly-CSharp");
        string relName = (string.IsNullOrEmpty(GeneratorConfig.UI_NAMESPACE)?"":GeneratorConfig.UI_NAMESPACE +".")+className;
        Type type= csAssemly.GetType(relName);
        if (type==null) return;

        string objName = className.Replace("DataComponent", "");
        GameObject obj=GameObject.Find(objName);
        if (obj==null) return;
        
        //挂载或者获取组件
        Component cpt=obj.GetComponent(type);
        if (cpt == null)
        {
            cpt=obj.AddComponent(type);
        }
        
        //遍历对象列表
        string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        var objDataList = JsonMapper.ToObject<List<EditorObjectData>>(dataListJson);

        //获取所有字段
        FieldInfo[] fields =type.GetFields();
        foreach (var item in fields)
        {
            foreach (var objData in objDataList)
            {
                if (item.Name==objData.fileName+objData.fileType)
                {
                    //用instanceid拿到游戏对象
                    GameObject uiObj=EditorUtility.InstanceIDToObject(objData.insID)as GameObject;
                    if (string.Equals(objData.fileType,"GameObject"))
                    {
                        item.SetValue(cpt,uiObj); 
                    }
                    else
                    {
                        item.SetValue(cpt,uiObj.GetComponent(objData.fileType)); 
                    }
                     
                    
                }
            }
        }


    }
}

