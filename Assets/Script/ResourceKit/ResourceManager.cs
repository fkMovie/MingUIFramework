using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public enum LoadResourceType
{
    Resource,
    AssetBundle
}

public class ResourceManager 
{
    public string AbFolder=Application.streamingAssetsPath+"/"+EditorUserBuildSettings.activeBuildTarget;
    private LoadResourceType loadResourceType=LoadResourceType.Resource;
    
    private static ResourceManager m_Instance;

    public static ResourceManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new ResourceManager();
                m_Instance.Init();
            }

            return m_Instance;
        }
    }
    private UIConfig m_UIConfig;
    //todo 新增一个ABManager

    public void Init()
    {
        m_UIConfig = Resources.Load<UIConfig>("UIConfig");
    }

    public GameObject LoadUI(string uiName,Transform parent)
    {
        if (loadResourceType==LoadResourceType.Resource)
        {
          return  LoadUIByResource(uiName,parent);
        }
        else
        {
            //TODO 
           return LoadUIByResource(uiName,parent);
        }
    }
    
    public GameObject LoadUIByResource(string uiName,Transform parent)
    {
        string path = m_UIConfig.GetUIPath(uiName);
        GameObject uiPrefab = Resources.Load<GameObject>(path);
        if (uiPrefab == null) Debug.LogError($"该路径未含对应预制体，请检查路径：{path}");

        GameObject uiObj = GameObject.Instantiate(uiPrefab, parent);
        // uiObj.transform.SetParent(m_UIRoot);
        uiObj.transform.localScale = Vector3.one;
        uiObj.transform.localPosition = Vector3.zero;
        uiObj.transform.localRotation = Quaternion.identity;
        uiObj.name = uiName;
        return uiObj;
    }

    //Test
    // public AssetBundle LoadAB(string abName)
    // {
    //     string abPath=Path.Combine(AbFolder, abName+".ab");
    //     AssetBundle ab = AssetBundle.LoadFromFile(abPath);
    //     Debug.Log(ab!=null);
    //     return ab;
    // }

    // public void LoadUI(string uiName)
    // { 
    //     LoadAB("texture");
    //    AssetBundle uiAssetBundle= LoadAB("ui");
    //    GameObject go= uiAssetBundle.LoadAsset<GameObject>(uiName);
    //    GameObject.Instantiate(go);
    // }
    
}
