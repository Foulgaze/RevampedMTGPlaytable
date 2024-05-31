using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class CardManager
{
    private static int cardID = 0;
    private static List<string> twoSidedCardLayouts = new List<string>(){"meld", "transform", "modal_dfc"};
    private static Dictionary<int, Card> idToCard = new Dictionary<int, Card>();

    /// <summary>
    /// Loads a list of cards based on the provided card names.
    /// </summary>
    /// <param name="cardNames">A list of card names to load information for.</param>
    /// <returns>A list of <see cref="Card"/> objects created from the provided card names.</returns>
    public static List<Card> LoadCardNames(List<string> cardNames)
    {
        List<Card> cards = new List<Card>();
        foreach(string cardName in cardNames)
        {
            CardInfo? info = CardData.GetCardInfo(cardName);
            string frontName = "";
            string? backName = null;
            if(info == null)
            {
                // To do
                // Add logger
                continue;
            }
            if(twoSidedCardLayouts.Contains(info.layout))
            {
                (frontName, backName) = GetFrontBackNames(info.name);
            }
            else
            {
                frontName = info.name;
            }
            Card newCard = new Card(cardID++, CardData.GetCardInfo(frontName), CardData.GetCardInfo(backName));
            idToCard[newCard.Id] = newCard;
        }
        return cards;
    }

    private static (string, string?) GetFrontBackNames(string fullName)
    {
        fullName = fullName.Trim();

        int doubleSlashIndex = fullName.IndexOf("//");
        if(doubleSlashIndex == -1)
        {
            return (fullName, null);
        }
        string frontName = fullName.Substring(0, doubleSlashIndex);
        string backName = fullName.Substring(doubleSlashIndex);
        return (frontName, backName);


    }
}
