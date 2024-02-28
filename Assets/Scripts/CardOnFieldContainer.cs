using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public class CardOnFieldContainer : MonoBehaviour
{
    Deck deck;
    RectTransform hoveredCard;

    public void SetValues(Deck deck)
    {
        this.deck = deck;
    }

    public void EnterMouseOver()
    {
        Debug.Log("Entered Pile");
        if(GameManager.Instance.clientPlayer.hand.heldCard == null)
        {
            return;
        }
        hoveredCard = GameManager.Instance.clientPlayer.hand.heldCard;
        hoveredCard.transform.GetComponent<CardMover>().Disable();
        CardInfo info = hoveredCard.GetComponent<CardMover>().info;
        // deck.AddCard(info.name);
    }
    public void ExitMouseOver()
    {
        Debug.Log("Exiting!");
        if(hoveredCard == null)
        {
            return;
        }
        CardInfo info = GameManager.Instance.clientPlayer.hand.heldCard.GetComponent<CardMover>().info;
        deck.RemoveCardName(info.name);
        hoveredCard.transform.GetComponent<CardMover>().Enable();

    }
}
