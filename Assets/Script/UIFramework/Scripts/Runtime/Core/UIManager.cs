using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace Ming_UIFramework
{
    public class UIManager
    {
        private static UIManager m_Instance;

        public static UIManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new UIManager();
                    m_Instance.Init();
                }

                return m_Instance;
            }
        }

        private Camera m_UICamera;
        private Transform m_UIRoot;
        private UIConfig m_UIConfig;

        /// <summary>
        /// UI字典
        /// </summary>
        private Dictionary<string, UIBase> m_UIDic = new Dictionary<string, UIBase>();

        /// <summary>
        /// UI列表
        /// </summary>
        private List<UIBase> m_UIList = new List<UIBase>();

        /// <summary>
        /// 打开的UI列表
        /// </summary>
        private List<UIBase> m_VisableUIList = new List<UIBase>();

        /// <summary>
        /// 队列，管理窗口的循环弹出
        /// </summary>
        public Queue<UIBase> m_UIQueue = new Queue<UIBase>();

        private bool m_isPopingUI = false; //开始弹出堆栈的旗帜，用来处理多种情况比如正在出栈时有其他界面入栈


        public void Init()
        {
            m_UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            m_UIRoot = GameObject.Find("UIRoot").transform;
            m_UIConfig = Resources.Load<UIConfig>("UIConfig");
#if UNITY_EDITOR
            m_UIConfig.GenerateUIConfig();
#endif
        }


        #region 窗口管理

        public void PreloadUI<T>() where T : UIBase, new()
        {
            if(GetUIIsLoaded<T>()) return;
            Type type = typeof(T);
            string uiName = type.Name;
            T uiBase = new T();
            GameObject uiObj = TempLoadUIByResource(uiName);
            if (!uiObj)
            {
                Debug.LogError($"预加载窗口失败：{uiObj.name}");
                return;
            }
            
            //初始化该UI
            uiBase.gameObject = uiObj;
            uiBase.transform = uiObj.transform;
            uiBase.canvas = uiObj.GetComponent<Canvas>();
            uiBase.canvas.worldCamera = m_UICamera;
            uiBase.Name = uiName;   
            uiBase.OnAwake();
            uiBase.SetVisiable(false);
 
  
            RectTransform rectTran = uiObj.GetComponent<RectTransform>();
            rectTran.anchorMax = Vector2.one;
            rectTran.offsetMax = Vector2.zero;
            rectTran.offsetMin = Vector2.zero;

            //写入Manager进行管理
            m_UIDic.Add(uiName, uiBase);
            m_UIList.Add(uiBase);

            //遮罩
            SetUIMaskVisable();
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenUI<T>() where T : UIBase, new()
        {
            string uiName = typeof(T).Name;
            UIBase ui = GetUI(uiName);
            if (ui != null)
            {
                return ShowUI(uiName) as T;
            }

            T t = new T();
            return InitUI(t, uiName) as T;
        }

        public T GetUI<T>() where T : UIBase, new()
        {
            System.Type uiType = typeof(T);
            foreach (UIBase item in m_VisableUIList)
            {
                if (item.Name == uiType.Name)
                {
                    return item as T;
                }
            }

            Debug.LogWarning($"{uiType.Name}获取失败，原因：未打开或未显示");
            return null;
        }

        public bool GetUIIsLoaded<T>()
        {
            string name = typeof(T).Name;
            return m_UIDic.ContainsKey(name);
        }

        public void HideUI<T>() where T : UIBase, new()
        {
            HideUI(typeof(T).Name);
        }

        public void DestroyUI<T>() where T : UIBase, new()
        {
            DestroyUI(typeof(T).Name);
        }

        public void DestroyAllUI(List<string> filterList = null)
        {
            for (int i = m_UIList.Count - 1; i >= 0; i--)
            {
                UIBase ui = m_UIList[i];
                if (ui == null || filterList != null && filterList.Contains(m_UIList[i].Name))
                {
                    continue;
                }

                DestroyUI(ui);
            }

            Resources.UnloadUnusedAssets();
        }

        public UIBase OpenUI(UIBase uiBase)
        {
            Type t = uiBase.GetType();
            string uiName = t.Name;
            UIBase ui = GetUI(uiName);
            if (ui != null)
            {
                return ShowUI(uiName);
            }

            return InitUI(uiBase, uiName);
        }

        public void DestroyUI(string uiName)
        {
            UIBase ui = GetUI(uiName);
            DestroyUI(ui);
        }

        private void DestroyUI(UIBase uiBase)
        {
            if (uiBase != null)
            {
                if (m_UIDic.ContainsKey(uiBase.Name))
                {
                    m_UIDic.Remove(uiBase.Name);
                    m_UIList.Remove(uiBase);
                    m_VisableUIList.Remove(uiBase);
                }

                uiBase.SetVisiable(false);
                SetUIMaskVisable();
                uiBase.OnHide();
                uiBase.OnDestroy();
                GameObject.Destroy(uiBase.gameObject);
            }
        }

        public void HideUI(string uiName)
        {
            UIBase ui = GetUI(uiName);
            HideUI(ui);
        }

        private void HideUI(UIBase ui)
        {
            if (ui != null && ui.Visible)
            {
                ui.SetVisiable(false);
                SetUIMaskVisable();
                ui.OnHide();
                m_VisableUIList.Remove(ui);
                
                PopNextStackUI(ui);
                
            }
            else
            {
                Debug.LogWarning($"尝试关闭不存在或者未显示的UI：{ui.Name}");
            }
        }

        private UIBase InitUI(UIBase uiBase, string uiName)
        {
            //初始化该UI
            GameObject newUI = TempLoadUIByResource(uiName);
            uiBase.gameObject = newUI;
            uiBase.transform = newUI.transform;
            uiBase.canvas = newUI.GetComponent<Canvas>();
            uiBase.canvas.worldCamera = m_UICamera;
            uiBase.Name = uiName;
            uiBase.transform.SetAsLastSibling();
            uiBase.OnAwake();
            uiBase.SetVisiable(true);
  
            RectTransform rectTran = newUI.GetComponent<RectTransform>();
            rectTran.anchorMax = Vector2.one;
            rectTran.offsetMax = Vector2.zero;
            rectTran.offsetMin = Vector2.zero;

            //写入Manager进行管理
            m_UIDic.Add(uiName, uiBase);
            m_UIList.Add(uiBase);
            m_VisableUIList.Add(uiBase);

            //遮罩
            SetUIMaskVisable();

            uiBase.OnShow();
            return uiBase;
        }

        private UIBase ShowUI(string uiName)
        {
            if (m_UIDic.ContainsKey(uiName))
            {
                UIBase ui = m_UIDic[uiName];
                if (ui.gameObject != null && ui.Visible == false)
                {
                    m_VisableUIList.Add(ui);
                    ui.gameObject.transform.SetAsLastSibling();
                    ui.SetVisiable(true);
                    SetUIMaskVisable();
                    ui.OnShow();
                }

                return ui;
            }
            else
            {
                Debug.LogError($"字典中未含该UI ：{uiName}，请使用OpenUI");
                return null;
            }
        }

        private UIBase GetUI(string uiName)
        {
            if (m_UIDic.ContainsKey(uiName))
            {
                return m_UIDic[uiName];
            }

            return null;
        }

        private void SetUIMaskVisable()
        {
            if (!UISetting.Instance.SINGMASK_SYSTEM) return;

            UIBase maxOrderUIBase = null; //最大渲染层级的窗口
            int maxOrder = 0; //最大层级
            int maxIndex = 0; //最大排序下标
            for (int i = 0; i < m_VisableUIList.Count; i++)
            {
                UIBase ui = m_VisableUIList[i];
                if (ui != null && ui.gameObject != null)
                {
                    ui.SetMaskVisible(false);
                    if (maxOrderUIBase == null)
                    {
                        maxOrderUIBase = ui;
                        maxOrder = ui.canvas.renderOrder;
                        maxIndex = ui.transform.GetSiblingIndex();
                    }
                    else
                    {
                        //找到最大渲染层级的UI
                        if (maxOrder < maxOrderUIBase.canvas.renderOrder)
                        {
                            maxOrderUIBase = ui;
                            maxOrder = ui.canvas.renderOrder;
                        }
                        //渲染层级一样时，通过位置下标来排序前后
                        else if (maxOrder == maxOrderUIBase.canvas.renderOrder)
                        {
                            if (maxIndex < ui.transform.GetSiblingIndex())
                            {
                                maxOrderUIBase = ui;
                                maxIndex = ui.transform.GetSiblingIndex();
                            }
                        }
                    }
                }
            }

            if (maxOrderUIBase != null)
            {
                maxOrderUIBase.SetMaskVisible(true);
            }
        }
        

        private GameObject TempLoadUIByResource(string uiName)
        {
            string path = m_UIConfig.GetUIPath(uiName);
            GameObject uiPrefab = Resources.Load<GameObject>(path);
            if (uiPrefab == null) Debug.LogError($"该路径未含对应预制体，请检查路径：{path}");

            GameObject uiObj = GameObject.Instantiate(uiPrefab, m_UIRoot);
            // uiObj.transform.SetParent(m_UIRoot);
            uiObj.transform.localScale = Vector3.one;
            uiObj.transform.localPosition = Vector3.zero;
            uiObj.transform.localRotation = Quaternion.identity;
            uiObj.name = uiName;
            return uiObj;
        }

        #endregion

        #region 堆栈系统
        
        /// <summary>
        /// 压栈并开始弹出第一个UI
        /// </summary>
        /// <param name="popCallback"></param>
        /// <typeparam name="T"></typeparam>
        public void PushAndPopFirstUI<T>(Action<UIBase> popCallback=null) where T:UIBase ,new()
        {
            PushUIToStack<T>(popCallback);
            StartPopFirstUI();
        }

        /// <summary>
        /// 压栈
        /// </summary>
        /// <param name="popCallback"></param>
        /// <typeparam name="T"></typeparam>
        public void PushUIToStack<T>(Action<UIBase> popCallback = null) where T : UIBase, new()
        {
            T uiBase = new T();
            // uiBase.PopStack = true;
            uiBase.PopStackListerner = popCallback;
            m_UIQueue.Enqueue(uiBase);
        }

        /// <summary>
        /// 弹出堆栈第一个UI
        /// </summary>
        public void StartPopFirstUI()
        {
            if (m_isPopingUI) return;
            m_isPopingUI = true;
            PopStackUI();
        }

        /// <summary>
        /// 弹出堆栈中的下一个窗口
        /// </summary>
        /// <param name="curUI">当前关闭的UI</param>
        public void PopNextStackUI(UIBase curUI)
        {
            if(curUI==null) return;
            if(m_isPopingUI||!curUI.IsPopStack) return;
            curUI.IsPopStack = false;
            PopStackUI();
        }

        /// <summary>
        /// 弹出堆栈UI内部方法
        /// </summary>
        /// <returns></returns>
        private bool PopStackUI()
        {
            if (m_UIQueue.Count == 0)
            {
                m_isPopingUI=false;
                return false;
            }
            UIBase uiBase = m_UIQueue.Dequeue();
            UIBase popUI = OpenUI(uiBase);
            popUI.PopStackListerner = uiBase.PopStackListerner;
            popUI.IsPopStack = true;
            popUI.PopStackListerner?.Invoke(popUI);
            popUI.PopStackListerner = null;
            m_isPopingUI = false;
            return true;
        }

        public void ClearStack()
        {
            m_UIQueue.Clear();
            m_isPopingUI = false;
        }

        #endregion
        
    }
}