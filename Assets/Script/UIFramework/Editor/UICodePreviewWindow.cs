using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UICodePreviewWindow : EditorWindow
{
    private string content;
    private string filePath;
    
    private Vector2 scroll=new Vector2();
    /// <summary>
    /// 显示代码预览窗口
    /// </summary>
    /// <param name="content"></param>
    /// <param name="filePath"></param>
    /// <param name="insterDic"></param>
    public static void ShowWindow(string _content, string _filePath, Dictionary<string, string> insterDic  = null)
    {
        UICodePreviewWindow window = GetWindowWithRect(typeof(UICodePreviewWindow), 
                new Rect(100, 50, 800, 700), true, "UI代码预览") as UICodePreviewWindow;
        window.content=_content;
        window.filePath = _filePath;
        
        //处理代码新增
        if (File.Exists(_filePath) && insterDic != null)
        {
            string oldContent = File.ReadAllText(_filePath);
            window.content=oldContent;
            foreach (var item in insterDic)
            {
                if (!oldContent.Contains(item.Key))
                {
                   int index= window.GetInserIndex(window.content);
                   window.content= window.content.Insert(index, item.Value+"\n\n");
                   
                   // int index= window.GetInserIndex(oldContent);
                   // oldContent= window.content= oldContent.Insert(index, item.Value+"\n\n");
                }
            }
        }
        window.Show();
    }

    private void OnGUI()
    {
        scroll=EditorGUILayout.BeginScrollView(scroll,GUILayout.Height(600),GUILayout.Width(800));
        EditorGUILayout.TextArea(content);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        
        //绘制脚本生成路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("脚本生成路径："+filePath);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成脚本",GUILayout.Height(30)))
        {
            Create();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void Create(Action _callback = null)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        // StreamWriter writer= File.CreateText(csPath);
        // writer.Write(csContent);
        // writer.Close();
        
        File.WriteAllText(filePath,content);
        _callback?.Invoke();
        AssetDatabase.Refresh();
        
        
        if (EditorUtility.DisplayDialog("UI自动生成脚本", "脚本生成成功", "确定"))
        {
            Close();
        }
    }

    /// <summary>
    /// 获取插入代码的下标
    /// </summary>
    /// <param name="_content"></param>
    /// <returns></returns>
    public int GetInserIndex(string _content)
    {
        Regex regex = new Regex("UI组件事件");
        Match match = regex.Match(_content);
        
        Regex regex1 = new Regex("public");
        MatchCollection matchCollection=regex1.Matches(_content);
        foreach (Match item in matchCollection)
        {
            if (item.Index>match.Index)
            {
                return item.Index;
            }
        }
        return -1;
    }
}