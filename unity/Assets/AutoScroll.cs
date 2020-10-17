using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect sr;
    public float Normal = 0;

    public void ScrollDetected()
    {
        MMONetwork.scrolling = true;
    }

    public void ScrollStopped()
    {
        MMONetwork.scrolling = false;
    }

    void Update()
    {
        Normal = sr.verticalNormalizedPosition;
        if (!MMONetwork.scrolling)
        {
            Canvas.ForceUpdateCanvases();
            sr.verticalScrollbar.value = 0f;
            Canvas.ForceUpdateCanvases();
        }
    }
}
