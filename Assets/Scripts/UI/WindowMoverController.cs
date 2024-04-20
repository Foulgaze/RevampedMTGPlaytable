using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMoverController : MonoBehaviour
{
    RectTransform currentWindow;
    Vector2 offset;

    public bool IsHoldingWindow()
    {
        return currentWindow != null;
    }

    bool IsDraggingWindow()
    {
        return IsHoldingWindow() && Input.GetMouseButton(0);
    }

    bool IsReleasingWindow()
    {
        return IsHoldingWindow() && !Input.GetMouseButton(0);
    }

    void ReleaseWindow()
    {
        currentWindow = null;
    }


    public void BeginWindowDrag(RectTransform rt)
    {
        currentWindow = rt;
        rt.SetAsLastSibling();
        offset = Input.mousePosition - rt.transform.position;
    }

    void Update()
    {
        if(IsReleasingWindow())
        {
            ReleaseWindow();
        }

        if(IsDraggingWindow())
        {
            currentWindow.transform.position = (Vector2) Input.mousePosition - offset;
        }
    }
}
