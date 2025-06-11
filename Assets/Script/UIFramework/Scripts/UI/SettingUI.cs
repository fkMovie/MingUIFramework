/*------------------------------------
 *Title: UI表现层自动化生成脚本工具
 *Author: ZiMing
 *Date:  2025/6/9 9:12:11
 *Description: 该层只负责界面的交互、表现相关的代码，不允许编写业务逻辑代码
 *注意：以下代码均由脚本生成，再次生成不会覆盖原有的代码，可放心使用
 -------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;


namespace Ming_UIFramework
{
    public class SettingUI : UIBase
    {
        public SettingUIComponent uicompt = new SettingUIComponent();


        #region 生命周期函数

        public override void OnAwake()
        {
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

        public void OnmuteToggleChange(bool isOn, Toggle toggle)
        {
            Debug.Log(isOn?"静音已开启":"静音已关闭");
        }
        public void OnCloseButtonClick()
        {
            HideSelf(); 
        }

        #endregion
 
        // protected override void ShowAnimation()
        // {
        //     m_UIMaskCG.alpha = 0;
        //     m_UIMaskCG.DOFade(1, 0.2f);
        //     m_UIContent.localScale = new Vector3(1, 0, 1);
        //     m_UIContent.DOScaleY(1, 0.3f);
        // }
    }
}