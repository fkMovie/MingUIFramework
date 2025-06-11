using UnityEngine;

public enum　EnumGeneratorType
{
    Find,
    Bind
}
public enum　EnumParseType
{
    Name,
    Tag
}
public class GeneratorConfig
{
    public static string FindComponentGeneratorPath=Application.dataPath+"/Script/UIFramework/Scripts/FindComponent";
    public static string BindComponentGeneratorPath=Application.dataPath+"/Script/UIFramework/Scripts/BindComponent";
    public static string uiGeneratorPath=Application.dataPath+"/Script/UIFramework/Scripts/UI";
    //模板
    public static string uiTemplatePath=Application.dataPath+"/Script/UIFramework/ScriptTemplate/UITemplate.cs.txt";
    
    
    public static string OBJDATALIST_KEY = "ObjDataList";
    public static string UI_NAMESPACE = "Ming_UIFramework"; 
    
    public static EnumGeneratorType eGenType = EnumGeneratorType.Bind;
    public static EnumParseType eParseType = EnumParseType.Name;
    public static string[] TagArr={"Image","GameObject","Text","Button","InputField","Toggle","Canvas","Slider","Dropdown"};
    
}
