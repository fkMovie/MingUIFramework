using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "UIConfig", menuName = "UIConfig",order = 1)]
public class UIConfig :ScriptableObject
{
    private string[] uiRootArr = new string[] { "Game", "Hall", "UI" };
    public List<UIData> uiDataList = new List<UIData>();

    public void GenerateUIConfig()
    {
        uiDataList.Clear();
        foreach (var item in uiRootArr)
        {
            string foler = Application.dataPath + "/Script/UIFramework/Resources/" + item;
            string[] fileArr=Directory.GetFiles(foler, "*.prefab", SearchOption.AllDirectories);
            foreach (var fileFullPath in fileArr)
            {
                if(fileFullPath.EndsWith(".meta")) continue;
                //获取预制体名字
                string fileName = Path.GetFileNameWithoutExtension(fileFullPath);
                string filePath=item+"/"+fileName;
                UIData uiData = new UIData(){name = fileName,path = filePath};
                uiDataList.Add(uiData);
            }
        }
    }

    public string GetUIPath(string uiName)
    {
        foreach (var item in uiDataList)
        {
            if (string.Equals(item.name,uiName))
            {
                return item.path;
            }
        }
        Debug.LogError($"配置文化不存在该预制体: {uiName}，请检查UIConfig");
        return null;
    }
}

[System.Serializable]
public class UIData
{
    public string name;
    public string path;
}

public enum UIAnimationType
{
    None,
    Scale
}