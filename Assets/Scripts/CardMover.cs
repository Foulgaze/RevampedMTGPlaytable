using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class CardMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    RectTransform _rt;

    public CardInfo info;
    float previousYValue;
    public HandManager _handManager;

    bool isDragging = false;
    Vector2 offset;
    [SerializeField]
    List<Transform> derenderTransforms;

    void Start()
    {
        _rt = transform.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_handManager.IsHoldingCard())
        {
            return;
        }
        HoverCard();
    }

    void HoverCard()
    {
        previousYValue = _rt.anchoredPosition.y;
        _rt.anchoredPosition = new Vector2(_rt.anchoredPosition.x,_rt.sizeDelta.y/4 );
        transform.SetAsLastSibling();
        _handManager.UpdateHoverCardPosition(_rt);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(_handManager.IsHoldingCard())
        {
            return;
        }
        _handManager.UpdateCardPositions();
        _rt.anchoredPosition = new Vector2(_rt.anchoredPosition.x,previousYValue);
    }

      public void OnPointerDown(PointerEventData eventData)
    {
        if(_handManager.IsHoldingCard())
        {
            return;
        }
        isDragging = true;
        _handManager.DragCard(_rt);
        offset = eventData.position - (Vector2)transform.position;

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = eventData.position - offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isDragging)
        {
            return;
        }
        isDragging = false;
        _handManager.ReleaseCard(_rt);
        HoverCard();
    }

    public void Disable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void Enable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
