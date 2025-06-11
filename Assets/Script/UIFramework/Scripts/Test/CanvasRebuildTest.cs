using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRebuildTest : MonoBehaviour
{
    private IList<ICanvasElement> mLayoutRebuildQueue;
    private IList<ICanvasElement> mGraphciRebuildQueue;

    void Start()
    {
        Type type = typeof(CanvasUpdateRegistry);
        FieldInfo field = type.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        mLayoutRebuildQueue = field.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;

        field = type.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
        mGraphciRebuildQueue = field.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;
    }


    void Update()
    {
        for (int i = 0; i < mLayoutRebuildQueue.Count; i++)
        {
            var rebuild = mLayoutRebuildQueue[i];
            if (ObjectVaildForUpdate(rebuild))
            {
                Debug.Log($"{rebuild.transform.name}引起了{rebuild.transform.GetComponent<Graphic>().canvas.name}重绘");
            }
        }

        for (int i = 0; i < mGraphciRebuildQueue.Count; i++)
        {
            var rebuild = mGraphciRebuildQueue[i];
            if (ObjectVaildForUpdate(rebuild))
            {
                Debug.Log($"{rebuild.transform.name}引起了{rebuild.transform.GetComponent<Graphic>().canvas.name}");
            }
        }
    }

    private bool ObjectVaildForUpdate(ICanvasElement element)
    {
        bool vaild = element != null;
        bool isUnityObject = element is UnityEngine.Object;
        if (isUnityObject)
        {
            vaild = (element as object) != null;
        }

        return vaild;
    }
}