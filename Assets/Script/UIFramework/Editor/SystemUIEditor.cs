using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    [InitializeOnLoadMethod]
    public static void InitEditor()
    {
        EditorApplication.hierarchyChanged += HandleUIRaycaster;
    }

    private static void HandleUIRaycaster()
    {
        GameObject go = Selection.activeGameObject;
        if (go != null)
        {
            if (go.name.Contains("Text") || go.name.Contains("Image"))
            {
                Graphic graphic = go.GetComponent<Graphic>();
                if (graphic != null)
                {
                    graphic.raycastTarget = false;
                }
            }
        }
    }
}