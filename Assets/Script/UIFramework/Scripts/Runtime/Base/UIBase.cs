using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Ming_UIFramework;


// ReSharper disable InconsistentNaming

public class UIBase : WindowBehaviour
{
    // all button list
    private List<Button> m_AllButtonList = new List<Button>();

    // all Toggle list
    private List<Toggle> m_ToggleList = new List<Toggle>();

    // all InputField list
    private List<InputField> m_InputList = new List<InputField>();

    protected CanvasGroup m_UIMaskCG;
    protected CanvasGroup m_CanvasGroup;
    protected Transform m_UIContent;
    protected UIAnimationType m_AnimType = UIAnimationType.Scale;

    private void InitBaseComponent()
    {
        m_UIMaskCG = transform.Find("UIMask").GetComponent<CanvasGroup>();
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
        m_UIContent = transform.Find("UIContent").GetComponent<Transform>();
    }

    #region 生命周期

    public override void OnAwake()
    {
        base.OnAwake();
        InitBaseComponent();
    }

    public override void OnShow()
    {
        base.OnShow();
        ShowAnimation();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveAllButtonListerners();
        RemoveAllToggleListeners();
        RemoveAllInputFieldListeners();
        m_AllButtonList.Clear();
        m_ToggleList.Clear();
        m_InputList.Clear();
    }

    #endregion

    #region 事件管理

    public void AddButtonClickListener(Button btn, UnityAction action)
    {
        if (btn != null)
        {
            if (!m_AllButtonList.Contains(btn))
            {
                m_AllButtonList.Add(btn);
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }

    public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
    {
        if (toggle != null)
        {
            if (!m_ToggleList.Contains(toggle))
            {
                m_ToggleList.Add(toggle);
            }

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((bool ison) => action?.Invoke(ison, toggle));
        }
    }

    public void AddInputFieldListener(InputField inputField, UnityAction<string> onChange,
        UnityAction<string> endAction)
    {
        if (inputField != null)
        {
            if (!m_InputList.Contains(inputField))
            {
                m_InputList.Add(inputField);
            }

            inputField.onValueChanged.RemoveAllListeners();
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onValueChanged.AddListener(onChange);
            inputField.onEndEdit.AddListener(endAction);
        }
    }

    public void RemoveAllButtonListerners()
    {
        foreach (var btn in m_AllButtonList)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAllToggleListeners()
    {
        foreach (var toggle in m_ToggleList)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }

    public void RemoveAllInputFieldListeners()
    {
        foreach (var inputField in m_InputList)
        {
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onValueChanged.RemoveAllListeners();
        }
    }

    #endregion

    public override void SetVisiable(bool _visiable)
    {
        // gameObject.SetActive(_visiable); //临时写法(已弃用)
        m_CanvasGroup.alpha = _visiable ? 1 : 0;
        m_CanvasGroup.blocksRaycasts = _visiable;
        Visible = _visiable;
    }

    public void SetMaskVisible(bool _visiable)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM) return;

        m_UIMaskCG.alpha = _visiable ? 1 : 0;
    }

    public void HideSelf()
    {
        HideAnimation();
        //UIManager.Instance.HideUI(Name);
    }

    #region 动画管理

    protected virtual void ShowAnimation()
    {
        if (canvas.sortingOrder < 99||m_AnimType==UIAnimationType.None) return;

        m_UIMaskCG.alpha = 0;
        m_UIMaskCG.DOFade(1, 0.2f);

        m_UIContent.localScale = Vector3.one * 0.8f;
        m_UIContent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    
    protected virtual void HideAnimation()
    {
        void hideCAllback()
        {
            UIManager.Instance.HideUI(Name);
        }

        if (canvas.sortingOrder < 99||m_AnimType==UIAnimationType.None) 
        {
            hideCAllback();
            return;
        }
        m_UIContent.DOScale(Vector3.one*1.1f, 0.3f).SetEase(Ease.OutBack).OnComplete(hideCAllback);
        
    }


    #endregion
}