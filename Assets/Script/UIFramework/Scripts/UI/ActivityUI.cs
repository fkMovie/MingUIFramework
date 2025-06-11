/*------------------------------------
 *Title: UI表现层自动化生成脚本工具  
 *Author: ZiMing  
 *Date:  2025/6/10 5:49:29
 *Description: 该层只负责界面的交互、表现相关的代码，不允许编写业务逻辑代码 
 *注意：以下代码均由脚本生成，再次生成不会覆盖原有的代码，可放心使用
 -------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


namespace Ming_UIFramework
{

public class ActivityUI : UIBase
{
    public ActivityUIDataComponent uicompt ;

  
#region 生命周期函数  
    public override void OnAwake()
    {
        uicompt= gameObject.GetComponent<ActivityUIDataComponent>();
        uicompt.InitComponent(this);
        base.OnAwake();
    }

    public override void OnShow()
    {
        base.OnShow();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

#endregion    

#region UI组件事件  


	public void OnCloseButtonClick()
	{
	 HideSelf();
		
	}

    
#endregion 
}
}    
