using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class CardTexture
{
    public Queue<Card> cardsToBeTextured {get;} = new Queue<Card>();
    public Card card {get;}

    public CardTexture(Card card)
    {
        this.card = card;
        cardsToBeTextured.Enqueue(card);
    }
}
