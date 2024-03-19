using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class CardTexture
{
    public Queue<Card> cardsToBeTextured {get;} = new Queue<Card>();
    public CardInfo cardInfo {get;}

    public CardTexture(Card card)
    {
        this.cardInfo = card.info;
        cardsToBeTextured.Enqueue(card);
    }
}
