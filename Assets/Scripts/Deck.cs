using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Deck : MonoBehaviour, CardContainer
{
    public Piletype deckID {get;set;}
    public List<Card> cards = new List<Card>();
    public PileController physicalDeck {set;get;}

    public int owner {get;set;}
    public bool _revealTopCard {get; set;} = true ;

    public Card? DrawCard()
    {
        if(cards.Count == 0)
        {
            return null;
        }
        
        Card drawnCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        UpdatePhysicalDeck();
        return drawnCard;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        UpdatePhysicalDeck();
    }

    public bool RemoveCard(Card card) 
    {
        bool removed = cards.Remove(card);
        UpdatePhysicalDeck();
        return removed;
    }

    
    public void UpdatePhysicalDeck()
    {
        physicalDeck.SetDeck(cards, _revealTopCard);
    }

    public void AddCardToContainer(Card card, int? position)
    {
        int insertPosition = position == null ? cards.Count : (int)position;
        cards.Insert(insertPosition, card);
        card.EnableRect();
        UpdatePhysicalDeck();
    }

    public DeckDescriptor GetDeckDescription()
    {
        int topCard = -1;
        if(cards.Count != 0)
        {
            topCard = cards[cards.Count - 1].id;
        }
        return new DeckDescriptor(cards.Count, topCard,(int) deckID, _revealTopCard);
    }

    public void RemoveCardFromContainer(Card card)
    {
        cards.Remove(card);
        UpdatePhysicalDeck();
    }

    public int GetOwner()
    {
        return owner;
    }

    public void UpdateDeck(DeckDescriptor newDeck)
    {
        cards.Clear();
        
        for(int i = 0; i < newDeck.cardCount - 1; ++i)
        {
            cards.Add(null);
        }
        if(newDeck.cardCount != 0)
        {
            cards.Add(GameManager.Instance.idToCard[newDeck.topCard]);
        }
        _revealTopCard = newDeck.revealTop;
        UpdatePhysicalDeck();

    }
}