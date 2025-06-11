using UnityEngine;
using UnityEngine.UI;

public static class UGUIAgent
{
    public static void SetVisable(this GameObject go, bool active)
    {
        go.transform.localScale = active ? Vector3.one : Vector3.zero;
    }
    
    public static void SetVisable(this Transform tran, bool active)
    {
        tran.localScale = active ? Vector3.one : Vector3.zero;
    }  
    
    // public static void SetVisable(this Button btn, bool active)
    // {
    //     btn.transform.localScale = active ? Vector3.one : Vector3.zero;
    // }
    public static void SetVisable(this ICanvasElement element, bool active)
    { 
        element.transform.localScale = active ? Vector3.one : Vector3.zero;
    }
    
}