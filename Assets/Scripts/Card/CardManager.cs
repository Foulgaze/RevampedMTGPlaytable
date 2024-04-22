using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    static int cardID = 0;

    public static void LoadDeck(List<string> cards, Deck deck)
    {
        foreach(string cardName in cards)
        {
            Card newCard = CreateCard(cardName);
            if(newCard == null)
            {
                Debug.LogError($"Unable to load card {cardName}");
                continue;
            }
            deck.AddCard(newCard);
        }
    }
    public static Card CreateCard(string cardName)
    {
        if(!GameManager.Instance.nameToCardInfo.ContainsKey(cardName))
        {
            return null;
        }
        Card newCard = new Card(cardID++,GameManager.Instance.nameToCardInfo[cardName], GameManager.Instance);
        GameManager.Instance.idToCard[newCard.id] = newCard;
        return newCard;
    }

    public static Card CopyCard(Card card)
    {
        if(!GameManager.Instance.nameToCardInfo.ContainsKey(card.info.name))
        {
            return null;
        }
        Card newCard = new Card(card,cardID++, GameManager.Instance.nameToCardInfo[card.info.name], GameManager.Instance);
        GameManager.Instance.idToCard[newCard.id] = newCard;
        return newCard;


    }
}
