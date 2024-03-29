using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CardMover : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Card card {get;set;}

    float previousYValue;
    HandManager handManager;

    void Start()
    {
        handManager = GameManager.Instance.handManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if(handManager.IsHoldingCard() || !handManager.CardInHand(card) || !GameManager.Instance.gameInteractable)
        {
            return;
        }
        HoverCard();
    }

    public void HoverCard()
    {
        previousYValue = card.GetInHandRect().anchoredPosition.y;
        card.GetInHandRect().anchoredPosition = new Vector2(card.GetInHandRect().anchoredPosition.x,card.GetInHandRect().sizeDelta.y/4 );
        transform.SetAsLastSibling();
        handManager.UpdateHoverCardPosition(card.GetInHandRect());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(handManager.IsHoldingCard() || !handManager.CardInHand(card) || !GameManager.Instance.gameInteractable)
        {
            return;
        }
        handManager.UpdateCardPositions();
        card.GetInHandRect().anchoredPosition = new Vector2(card.GetInHandRect().anchoredPosition.x,previousYValue);
    }

      public void OnPointerDown(PointerEventData eventData)
    {
        if(handManager.IsHoldingCard() || !GameManager.Instance.gameInteractable || eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        
        handManager.BeginCardDrag(card, eventData.position - (Vector2)transform.position);
 

    }


    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     Debug.Log("Letting Go!");
    //     if(!isDragging)
    //     {
    //         return;
    //     }
    //     isDragging = false;
    //     handManager.ReleaseCard();
    //     HoverCard();
    // }

    public void ReleaseCard()
    {
    }

    public void Disable()
    {

    }
    public void Enable()
    {
 
    }

}
