using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ming_UIFramework;

public class Test : MonoBehaviour
{
    private void Awake()
    {
       // UIManager.Instance.Init();
        //ResourceManager.Instance.Init();
        
        //ResourceManager.Instance.LoadUI("AccountUI");
    }

    void Start()
    {
        UIManager.Instance.OpenUI<LoginUI>();
    }


    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     UIManager.Instance.OpenUI<LoginUI>();
        // }      
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     UIManager.Instance.DestroyUI<LoginUI>();
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     //Resources.UnloadUnusedAssets();
        //     UIManager.Instance.DestroyAllUI(new List<string>(){"LoginUI"});
        // }

        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     bool value= UISetting.Instance.SINGMASK_SYSTEM;
        //     Debug.Log(value);
        // }
        
        //测试遮罩系统
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     UIManager.Instance.OpenUI<AccountUI>();
        // }
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     UIManager.Instance.OpenUI<SignupUI>();
        // }
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     UIManager.Instance.HideUI<AccountUI>();
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     UIManager.Instance.HideUI<SignupUI>();
        // }
        
        //测试生成的脚本是否正常
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //UIManager.Instance.OpenUI<TestUI>();
        }
        
    }
}
