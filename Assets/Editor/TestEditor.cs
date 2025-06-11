using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestEditor : Editor
{
    [MenuItem("Tools/TestEditor")]
    public static void Editor2Png()
    {
        string path = Application.dataPath + "/Art/图片";
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.webp", SearchOption.AllDirectories);
            foreach (string webpFile in files)
            {
                string pngFile = Path.ChangeExtension(webpFile, ".png");
                
                if (!File.Exists(pngFile))
                {
                    File.Move(webpFile, pngFile);
                    Debug.Log($"Renamed: {webpFile} -> {pngFile}"); 
                }
                else
                {
                    Debug.LogWarning($"Skipped (file exists): {pngFile}");
                }
            }
        }
    }
}
