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
    public string name {get;set;}
    public int owner {get;set;}
    public bool isClientDeck {get;set;}
    public bool _revealTopCard {get; set;} = true ;

    public Card? DrawCard()
    {
        if(cards.Count == 0)
        {
            return null;
        }
        
        Card drawnCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        UpdateContainer();
        return drawnCard;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
        UpdateContainer();
    }
    public void Shuffle()
    {
        HelperFunctions.ShuffleList<Card>(cards);
    }

    public bool RemoveCard(Card card) 
    {
        bool removed = cards.Remove(card);
        UpdateContainer();
        return removed;
    }

    public List<Card> GetCards()
    {
        return cards;
    }

    public string GetName()
    {
        return name;
    }

    
    public void UpdateContainer()
    {
        physicalDeck.SetDeck(cards, _revealTopCard, isClientDeck);
        DisplayContainerController libraryController = GameManager.Instance._uiManager.cardContainerDisplayHolder.GetComponent<DisplayContainerController>();
        if(libraryController.gameObject.activeInHierarchy && libraryController.currentContainer == this)
        {
            libraryController.UpdateHolder();
        }
    }

    public void DeleteDeckOnField()
    {
        physicalDeck.SetDeck(new List<Card>(), false, isClientDeck);
    }

    public void AddCardToContainer(Card card, int? position)
    {
        card.ClearStats();
        card.currentLocation = this;
        int insertPosition = position == null ? cards.Count : (int)position;
        insertPosition = Math.Clamp(insertPosition,0, cards.Count);
        cards.Insert(insertPosition, card);
        card.EnableRect();
        UpdateContainer();
    }

    public DeckDescriptor GetDeckDescription()
    {
        int topCard = -1;
        if(cards.Count != 0)
        {
            topCard = cards[cards.Count - 1].Id;
        }
        return new DeckDescriptor(cards.Count, topCard,(int) deckID, _revealTopCard, null);
    }

    public void LateConstructor(Piletype type, int id, string name, bool isClientDeck,bool revealTop = true)
    {
        this.deckID = type;
        this.owner = id;
        this.name = name;
        this._revealTopCard = revealTop;
        this.isClientDeck = isClientDeck;
    }

    public bool MoveTopCardToBottom()
    {
        if(cards.Count < 2)
        {
            return false;
        }
        Card topCard = cards[cards.Count-1];
        cards.Remove(topCard);
        cards.Insert(0,topCard);
        UpdateContainer();
        return true;
    }

    public void RemoveCardFromContainer(Card card)
    {
        cards.Remove(card);
        UpdateContainer();
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
        UpdateContainer();
    }

    public void ReleaseCardInBox(Card card)
    {
        return;
    }
}