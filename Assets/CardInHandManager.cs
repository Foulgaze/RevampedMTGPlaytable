using UnityEngine;
using UnityEngine.EventSystems;

public class CardInHandManager : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    bool isDragging = false;

    public HandManager _handManager;
    Vector2 offset;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Image Clicked");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        offset = eventData.position - (Vector2)transform.position;
        Debug.Log("Mouse Down on Image");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        Debug.Log("Mouse Up on Image");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _handManager.ReorderHand();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            transform.position = eventData.position - offset;
        }
    }
}
