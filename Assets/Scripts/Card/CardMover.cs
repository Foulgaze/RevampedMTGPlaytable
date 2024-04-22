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
        Debug.Log("Starting");
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
        if(handManager.IsHoldingCard() || !GameManager.Instance.gameInteractable || !card.interactable)
        {
            return;
        }
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            if(!handManager.CardInHand(card))
            {
                GameManager.Instance._uiManager.SpawnCardOnFieldMenu(this.card);
            }
            return;
        }
        if(HelperFunctions.IsHoldingCTRL() && !handManager.CardInHand(card))
        {
            card.TapUntap();
            return;
        }
        handManager.BeginCardDrag(card, eventData.position - (Vector2)transform.position);
 

    }

}
