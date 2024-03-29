using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Image = UnityEngine.UI.Image;
using System.Linq;




public class PileController : MonoBehaviour
{
    Vector3 _cardPileExtents;
    Vector3 _cardTopperExtents;
    Stack<Transform> _cards = new Stack<Transform>();
    void Awake()
    {
        _cardPileExtents = HelperFunctions.GetExtents(transform);
        _cardTopperExtents = HelperFunctions.GetExtents(GameManager.Instance.pileTopper);

    }

    public void SetDeck(List<Card> newCards, bool revealTop)
    {
        foreach(Transform physicalCard in _cards)
        {
            Destroy(physicalCard.gameObject);
        }
        _cards.Clear();
        for(int i = 0; i < newCards.Count; ++i)
        {
            Transform newCard = GenerateNewCard();
            newCard.position = GetIthCardPosition(i);
            _cards.Push(newCard);
        }
        UpdateTopCard(newCards, revealTop);
    }
    public void UpdateTopCard(List<Card> newCards, bool revealTop)
    {
        if(_cards.Count == 0)
        {
            return;
        }
        Card card = newCards[newCards.Count - 1];
        Transform canvasObj = _cards.Peek().GetChild(0);
        canvasObj.gameObject.SetActive(true);
        RectTransform cardTopper;
        if(!revealTop)
        {
            cardTopper = Instantiate(GameManager.Instance.cardBack).GetComponent<RectTransform>();
            cardTopper.GetComponent<CardMover>().card = card;
            card.DisableRect();
        }
        else
        {
            cardTopper = card.GetInHandRect();
        }
        cardTopper.SetParent(canvasObj);
        SetTopperValues(card, cardTopper);
    }

    void SetTopperValues(Card card, RectTransform cardTopper)
    {
        
        cardTopper.localPosition = Vector3.zero;
        cardTopper.localScale = Vector3.one;
        cardTopper.sizeDelta = Vector2.one;
        cardTopper.localRotation = Quaternion.identity;
        cardTopper.gameObject.SetActive(true);
    }
    Transform GenerateNewCard()
    {
        Transform newCard = Instantiate(GameManager.Instance.pileTopper);
        newCard.GetComponent<Renderer>().material = GameManager.Instance.pilemat;
        newCard.localScale = transform.lossyScale;
        return newCard;
    }


    Vector3 GetIthCardPosition(int i)
    {
        return transform.position + new Vector3(0,_cardTopperExtents.y * i * 2f + _cardPileExtents.y*2,0);
    }
}
