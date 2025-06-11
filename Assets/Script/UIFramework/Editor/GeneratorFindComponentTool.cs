using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;

public class GeneratorFindComponentTool : Editor
{
    /// <summary>
    /// key 为物体的insid  value为物体查找路径
    /// </summary>
    private static Dictionary<int, string> m_objFindPathDic;
    /// <summary>
    /// 查找对象的数据
    /// </summary>
    private static List<EditorObjectData> m_objDataList;

    [MenuItem("GameObject/UI工具/生成组件查找脚本(shift+F) #F",false,0)]
    static void GreateFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject;
        if (obj == null)
        {
            Debug.Log("<Color=Yellow> 需要选择GameOject </Color>");
            return;
        }
        m_objDataList=new List<EditorObjectData>();
        m_objFindPathDic=new Dictionary<int, string>();

        if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
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
        string csPath = $"{GeneratorConfig.FindComponentGeneratorPath}/{obj.name}Component.cs";
        GeneratorConfig.eGenType = EnumGeneratorType.Find;
        UICodePreviewWindow.ShowWindow(csContent,csPath);  
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
            if (name.Contains("[")&&name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fileName = name.Substring(index);
                string fileType=name.Substring(1,index-2);
                m_objDataList.Add(new EditorObjectData(){insID = obj.GetInstanceID(),fileName = fileName,
                    fileType = fileType});
                
                //计算该节点的查找路径
                string objPath = name;
                bool isFindOver=false;
                Transform parent=obj.transform;
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k <=j; k++)
                    {
                        if (k==j)
                        {
                            parent=parent.parent;
                            //如果父节点是当前UI，说明查找结束
                            if (string.Equals(parent.name,uiName))
                            {
                                isFindOver=true;
                                break;
                            }
                            else
                            {
                                objPath=objPath.Insert(0,parent.name+"/");
                            }
                        }
                    }
                    if (isFindOver) break;
                }
                m_objFindPathDic.Add(obj.GetInstanceID(),objPath); 
                Debug.Log($"名字：{obj.name} 路径： {objPath}");
                
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
            if (GeneratorConfig.TagArr.Contains(obj.tag))
            {
                string fileName = obj.name;
                string fileType=obj.tag;
                m_objDataList.Add(new EditorObjectData(){insID = obj.GetInstanceID(),fileName = fileName,
                    fileType = fileType});
                
                //计算该节点的查找路径
                string objPath = obj.name;
                bool isFindOver=false;
                Transform parent=obj.transform;
                for (int j = 0; j < 20; j++)
                {
                    for (int k = 0; k <=j; k++)
                    {
                        if (k==j)
                        {
                            parent=parent.parent;
                            //如果父节点是当前UI，说明查找结束
                            if (string.Equals(parent.name,uiName))
                            {
                                isFindOver=true;
                                break;
                            }
                            else
                            {
                                objPath=objPath.Insert(0,parent.name+"/");
                            }
                        }
                    }
                    if (isFindOver) break;
                }
                m_objFindPathDic.Add(obj.GetInstanceID(),objPath); 
                Debug.Log($"名字：{obj.name} 路径： {objPath}");
                
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
        sb.AppendLine(" *Description: 组件需以 [Text]组件名 格式命名，右键窗口物体--->生成组件查找脚本 ");
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

        sb.AppendLine($"\tpublic class {className}Component");
        sb.AppendLine("\t{");

        foreach (var obj in m_objDataList)
        {
            sb.AppendLine($"\t\t public {obj.fileType} {obj.fileName}{obj.fileType};\n");
        }

        //初始化组件
        sb.AppendLine("\t\tpublic void InitComponent(UIBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t        //组件查找");
        foreach (var obj in m_objFindPathDic)
        {
            EditorObjectData data = GetEditorObjectData(obj.Key);
            if (string.Equals(data.fileType, "GameObject"))
                sb.AppendLine($"\t\t        {data.fileName}{data.fileType} = ({data.fileType})target.transform.Find(\"{obj.Value}\").gameObject;");
            else if (string.Equals(data.fileType, "Transform"))
                sb.AppendLine($"\t\t        {data.fileName}{data.fileType} = ({data.fileType})target.transform.Find(\"{obj.Value}\").transform;");
            else 
                sb.AppendLine($"\t\t        {data.fileName}{data.fileType} = target.transform.Find(\"{obj.Value}\").GetComponent<{data.fileType}>();");   
        }
        
        sb.AppendLine("\t");  
        sb.AppendLine("\t");  
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
}

public class EditorObjectData
{
    public int insID;
    public string fileName;
    public string fileType;
}