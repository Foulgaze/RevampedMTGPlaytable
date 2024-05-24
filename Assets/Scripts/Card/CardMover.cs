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
        handManager.hoveredCard = card;
        GameManager.Instance._uiManager.SetHoveredCard(card);
        if(!handManager.CardInHand(card) || !card.interactable || handManager.IsHoldingCard() || !GameManager.Instance.gameInteractable)
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
        if(handManager.hoveredCard == this.card)
        {
            handManager.hoveredCard = null;
        }
        if(handManager.IsHoldingCard() || !handManager.CardInHand(card) || !GameManager.Instance.gameInteractable ||  !card.interactable)
        {
            return;
        }
        handManager.UpdateContainer();
        card.GetInHandRect().anchoredPosition = new Vector2(card.GetInHandRect().anchoredPosition.x,previousYValue);
    }

      public void OnPointerDown(PointerEventData eventData)
    {
        if(handManager.IsHoldingCard() || !GameManager.Instance.gameInteractable || !card.interactable || card.currentLocation.GetOwner() != GameManager.Instance.clientPlayer.id)
        {
            return;
        }
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            if(!handManager.CardInHand(card))
            {
                GameManager.Instance._uiManager.SpawnCardOnFieldMenu(this.card);
            }
            else
            {
                GameManager.Instance._uiManager.SpawnCardInHandMenu(this.card);
            }
            return;
        }
        if(HelperFunctions.IsHoldingCTRL() && !handManager.CardInHand(card))
        {
            GameManager.Instance.SendTapUntap(card);
            return;
        }
        handManager.BeginCardDrag(card, eventData.position - (Vector2)transform.position);
 

    }

}
