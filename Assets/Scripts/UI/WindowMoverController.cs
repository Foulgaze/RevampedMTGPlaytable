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
        if (IsReleasingWindow())
        {
            ReleaseWindow();
        }

        if (IsDraggingWindow())
        {
            Vector2 newPosition = (Vector2)Input.mousePosition - offset;
            newPosition = ClampToScreen(newPosition);
            currentWindow.position = newPosition;
        }
    }

    Vector2 ClampToScreen(Vector2 position)
    {
        Vector2 minPosition = currentWindow.rect.size * currentWindow.pivot;
        Vector2 maxPosition = new Vector2(Screen.width, Screen.height) - (currentWindow.rect.size * (Vector2.one - currentWindow.pivot));

        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);

        return position;
    }
}
