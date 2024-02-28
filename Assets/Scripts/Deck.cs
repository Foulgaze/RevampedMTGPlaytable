using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Deck : MonoBehaviour
{
    public List<Card> _cards = new List<Card>();
    public PileController physicalDeck {get;}
    public bool _revealTopCard {get; set;} = true ;

    public Card? DrawCard()
    {
        if(_cards.Count == 0)
        {
            return null;
        }
        
        Card drawnCard = _cards[_cards.Count - 1];
        _cards.RemoveAt(_cards.Count - 1);
        UpdatePhysicalDeck();
        return drawnCard;
    }

    public void AddCard(Card card)
    {
        _cards.Add(card);
        UpdatePhysicalDeck();
    }

    public bool RemoveCardName(int id) // Work in reverse order to remove card you're probably looking for
    {
        for(int i = _cards.Count - 1; i > -1; --i)
        {
            if(_cards[i].id == id)
            {
                _cards.RemoveAt(i);
                UpdatePhysicalDeck();
                return true;
            }
        }
        return false;
    }

    
    public void UpdatePhysicalDeck()
    {
        if(physicalDeck == null)
        {
            Debug.LogError("Physical Deck is missing");
            return;
        }
        physicalDeck.SetCardCount(_cards.Count);
        if(_cards.Count == 0)
        {
            return;
        }
        if(_revealTopCard)
        {
            // physicalDeck.SetTopper(GameManager.Instance._nameToCardInfo[ _cards[_cards.Count - 1]].CardSprite);
        }
        else
        {
            // physicalDeck.SetTopper(GameManager.Instance._cardBack);
        }
    }
}