using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardHolder
{

    List<Transform> _cards = new List<Transform>();
    Transform _cardHolder;
    Vector3 _cardExtents;
    Vector3 _cardHolderExtents;
    int _cardHoldLimit;

    public CardHolder(Vector3 card, Vector3 cardHolder, int cardLimit)
    {
        _cardExtents = card;
        _cardHolderExtents = cardHolder;
        _cardHoldLimit = cardLimit;
    }

    public Vector3 GetNextCardPosition()
    {
        Vector3 returnPosition = _cardHolder.position + new Vector3(0,_cardHolderExtents.y + _cardHolderExtents.y * (_cards.Count + 1),0); 
        return returnPosition;
    }
    public Vector3 GetNextCardPosition(int cardPosition)
    {
        Vector3 returnPosition = _cardHolder.position + new Vector3(0,_cardHolderExtents.y + _cardHolderExtents.y * (cardPosition + 1),0); 
        return returnPosition;
    }

    public bool AddCard(Transform card)
    {
        if(_cards.Count >= _cardHoldLimit)
        {
            return false;
        }
        _cards.Add(card);
        return true;

    }

    public void RemoveCard(Transform card)
    {
        if(_cards.Remove(card))
        {
            UpdateCardPositions();
        }
    }

    public void UpdateCardPositions()
    {
        for(int i = 0; i < _cards.Count; ++i)
        {
            _cards[i].position = GetNextCardPosition(i);
        }
    }
}
