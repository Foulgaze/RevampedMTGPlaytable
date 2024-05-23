using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowMover : MonoBehaviour, IPointerDownHandler
{
    WindowMoverController controller;
    [SerializeField] RectTransform rt;

    public void OnPointerDown(PointerEventData eventData)
    {
        controller.BeginWindowDrag(rt);
    }

    void Start()
    {
        controller = GameManager.Instance.windowController;
        if(rt == null)
        {
            rt = transform.GetComponent<RectTransform>();
        }
    }

}
