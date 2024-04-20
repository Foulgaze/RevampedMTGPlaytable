using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMover : MonoBehaviour, IPointerDownHandler
{
    WindowMoverController controller;
    RectTransform rt;

    public void OnPointerDown(PointerEventData eventData)
    {
        controller.BeginWindowDrag(rt);
    }

    void Start()
    {
        controller = GameManager.Instance.windowController;
        rt = transform.GetComponent<RectTransform>();
    }

}
