using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInHandManager : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    bool isDragging = false;

    public HandManager _handManager;
    Vector2 offset;

    RectTransform _card;


    void Start()
    {
        _card = transform.GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("Image Clicked");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_handManager.IsHoldingCard())
        {
            return;
        }
        isDragging = true;
        _handManager.DragCard(_card);
        offset = eventData.position - (Vector2)transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isDragging)
        {
            return;
        }
        isDragging = false;
        // _handManager.ReleaseCardSprite(_card);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // _handManager.ReorderHand();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = eventData.position - offset;
        }
    }
}
