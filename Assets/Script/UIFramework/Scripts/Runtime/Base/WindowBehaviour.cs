using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowBehaviour 
{
    /// <summary>
    /// 当前窗口物体
    /// </summary>
    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }
    public Canvas canvas { get; set; }
    public string Name { get; set; }
    public bool Visible { get; set; }
    
    public bool IsPopStack { get; set; }//是否是通过堆栈系统弹出的弹窗
    
    public Action<UIBase> PopStackListerner { get; set; }
    
    
    
    //生命周期参考Monobehaviour
    
    
    public virtual void OnAwake(){} 
    public virtual void OnShow(){}
    public virtual void OnUpdate(){}
    public virtual void OnHide(){}
    public virtual void OnDestroy(){}
    
    public virtual void SetVisiable(bool _visiable){}

}
