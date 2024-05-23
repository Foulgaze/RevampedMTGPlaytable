using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CardManager : MonoBehaviour
{
    static int cardID = 0;

    public static void LoadDeck(List<string> cards, Deck deck)
    {
        for(int i = 0; i < cards.Count; ++i)
        {
            string cardName = cards[i]; 
            int doubleSlashIndex = cardName.IndexOf("//");
            if(doubleSlashIndex != -1)
            {
                cardName = cardName.Substring(0, cardName.IndexOf("//")).Trim();
            }
            Card newCard = CreateCard(cardName);
            if(newCard == null)
            {
                Debug.LogError($"Unable to load card {cardName}");
                continue;
            }
            newCard.currentLocation = deck;
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
        if(!GameManager.Instance.nameToCardInfo.ContainsKey(card.name))
        {
            return null;
        }
        Card newCard = new Card(card,cardID++, GameManager.Instance.nameToCardInfo[card.name], GameManager.Instance);
        GameManager.Instance.idToCard[newCard.id] = newCard;
        return newCard;
    }
    // Can be combined with createcard probably :)
    public static Card? CreateRelatedCard(string cardName)
    {
        if(!GameManager.Instance.nameToToken.ContainsKey(cardName) && !GameManager.Instance.nameToCardInfo.ContainsKey(cardName))
        {
            return null;
        }
        Card newCard;
        if(GameManager.Instance.nameToToken.ContainsKey(cardName))
        {
            newCard = new Card(cardID++, GameManager.Instance.nameToToken[cardName], GameManager.Instance);
        }
        else
        {
            newCard = new Card(cardID++, GameManager.Instance.nameToCardInfo[cardName], GameManager.Instance);
        }
        GameManager.Instance.idToCard[newCard.id] = newCard;
        newCard.SetupEtherealCard();
        return newCard;
    }
}
