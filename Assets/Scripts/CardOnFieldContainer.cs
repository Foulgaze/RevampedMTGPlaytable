using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class CardOnFieldContainer : MonoBehaviour
{
    public Deck deck;
    Card hoveredCard;

    public void SetValues(Deck deck)
    {
        this.deck = deck;
    }

    public void EnterMouseOver()
    {
        if(GameManager.Instance.clientPlayer.hand.heldCard == null || GameManager.Instance.clientPlayer.id != deck.owner)
        {
            return;
        }
        hoveredCard = GameManager.Instance.clientPlayer.hand.heldCard;
        deck.AddCardToContainer(hoveredCard, null);
        // deck.AddCard(info.name);
    }
    public void ExitMouseOver()
    {
        
        if(GameManager.Instance.clientPlayer.hand.heldCard == null || GameManager.Instance.clientPlayer.id != deck.owner)
        {
            return;
        }
        hoveredCard = GameManager.Instance.clientPlayer.hand.heldCard;
        deck.RemoveCardFromContainer(this.hoveredCard);
        RectTransform rt = this.hoveredCard.GetInHandRect();
        GameManager.Instance.handManager.SetupRectForHand(rt, this.hoveredCard);
        hoveredCard = null;

    }
}
