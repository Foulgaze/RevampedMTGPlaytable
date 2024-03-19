using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class HandManager : MonoBehaviour, CardContainer
{
    List<Card> cards = new List<Card>();

    [SerializeField]
    RectTransform _handBox;
    [SerializeField]
    public Transform _handParent;
    [SerializeField]
    RectTransform _checkHandBox;
    [SerializeField]
    Vector2 _boxExtents = Vector2.zero;
    Vector2 _cardDimensions;
    float _cardRatio = 7f/5f;
    int _cardsPerHand = 5;
    int _cardYPositionFraction = 3;

    public Card heldCard = null;   
    Vector2 offset;

    Transform lastHoveredPile = null;

    public void SetValues()
    {
        _boxExtents = new Vector2(_handBox.rect.width, _handBox.rect.height);
        _cardDimensions = new Vector2(_boxExtents.x/_cardsPerHand, _boxExtents.x/_cardsPerHand * _cardRatio );
        _checkHandBox.sizeDelta = new Vector2(_boxExtents.x,_cardDimensions.y);
        _checkHandBox.transform.position = new Vector3(_handBox.transform.position.x,_handBox.transform.position.y -_cardDimensions.y/_cardYPositionFraction,0);

        
    }

    public bool CardInHand(Card checkCard)
    {
        foreach(Card card in cards)
        {
            if(checkCard == card)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsHoldingCard()
    {
        return heldCard != null;
    }

    public void BeginCardDrag(Card card, Vector2 offset)
    {
        bool cardInHand = CardInHand(card);
        heldCard = card;
        this.offset = cardInHand ? offset : Vector2.zero;
        if(cardInHand)
        {
            // SetupRectForHand(card.GetInHandRect(), card);
            RemoveCardFromContainer(card);
        } 
    }
    

    void ReleaseCardInHandBox(Card card)
    {
        for(int i = cards.Count - 1; i > -1; --i)
        {
            if(HelperFunctions.IsPointInRectTransform(Input.mousePosition, cards[i].GetInHandRect(), null))
            {
                i = Input.mousePosition.x > cards[i].GetInHandRect().transform.position.x ? i + 1 : i;
                AddCardToContainer(card,Mathf.Max(i,0));
                return;
            }
        }
        int insertPosition = Input.mousePosition.x < Screen.width/2 ? 0 : cards.Count;
        AddCardToContainer(card,insertPosition);
    }

    public void ReleaseCard()
    {
        bool inside = HelperFunctions.IsPointInRectTransform(Input.mousePosition, _checkHandBox, null);
        if(inside)
        {
            ReleaseCardInHandBox(heldCard);
            UpdateCardPositions();
            heldCard.GetInHandRect().GetComponent<CardMover>().HoverCard();
        }
        else if(lastHoveredPile == null)
        {
            AddCardToContainer(heldCard, null);
            UpdateCardPositions();
        }
        lastHoveredPile = null;
        heldCard = null;
        GameManager.Instance.SendUpdatedDecks();
    }
    
    public void AddCardToContainer(Card card, int? position)
    {
        card.currentLocation = this;
        int inputPosition = position == null ? cards.Count : (int)position;
        SetupRectForHand(card.GetInHandRect(), card);
        cards.Insert(inputPosition,card);
        UpdateCardPositions();
    }

    public void SetupRectForHand(RectTransform rt, Card card)
    {
        rt.SetParent(_handParent);
        rt.GetComponent<CardMover>().card = card;
        rt.sizeDelta = _cardDimensions;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        card.EnableRect();
    }
    public void RemoveCardFromContainer(Card card)
    {
        card.currentLocation = null;
        cards.Remove(card);
        UpdateCardPositions();
    }




    public void UpdateCardPositions()
    {

        float distanceBetweenCards = cards.Count <= _cardsPerHand ? _cardDimensions.x : (_boxExtents.x - _cardDimensions.x) / (cards.Count - 1);
        float startPos = -_cardDimensions.x/2 * (cards.Count-1);

        float currentXPosition = Mathf.Max(-_boxExtents.x/2 + _cardDimensions.x/2, startPos);
        foreach(Card card in cards)
        {
            RectTransform cardRect = card.GetInHandRect();
            cardRect.anchoredPosition = new Vector2(currentXPosition, -_cardDimensions.y/_cardYPositionFraction);
            currentXPosition += distanceBetweenCards;
        }
        UpdateCardSiblingPositions();
    }

    public void UpdateCardSiblingPositions()
    {
        for(int i = 0; i < cards.Count; ++i)
        {
            cards[i].GetInHandRect().SetSiblingIndex(i);
        }
    }

    public void UpdateHoverCardPosition( RectTransform hoverCardRect)
    {
        if(cards.Count <= _cardsPerHand || hoverCardRect == cards.Last().GetInHandRect())
        {
            return;
        }
        float distanceBetweenCards = cards.Count <= _cardsPerHand ? _cardDimensions.x : (_boxExtents.x - _cardDimensions.x * 2) / (cards.Count - 2);
        float startPos = -_cardDimensions.x/2 * (cards.Count-1);

        float currentXPosition = Mathf.Max(-_boxExtents.x/2 + _cardDimensions.x/2, startPos);
        foreach(Card card in cards)
        {
            RectTransform cardRect = card.GetInHandRect();
            cardRect.anchoredPosition = new Vector2(currentXPosition, cardRect.anchoredPosition.y);
            currentXPosition += cardRect == hoverCardRect ? _cardDimensions.x : distanceBetweenCards;
        }
    }

    void DragCard()
    {
        if(lastHoveredPile != null)
        {
            return;
        }
        heldCard.GetInHandRect().transform.position = (Vector2) Input.mousePosition - offset;
    }

    bool IsDraggingCard()
    {
        return heldCard != null && Input.GetMouseButton(0);
    }

    bool IsReleasingCard()
    {
        return heldCard != null && !Input.GetMouseButton(0);
    }


    void Update()
    {    
        if(IsReleasingCard())
        {
            ReleaseCard();
        }

        if(IsDraggingCard())
        {
            DragCard();
        }

        CheckForPile();

        
    }

    void CheckForPile()
    {
        Transform pile = RaycastForPile();
        if(lastHoveredPile == pile)
        {
            return;
        }
        if(lastHoveredPile != null &&  pile == null)
        {
            lastHoveredPile.GetComponent<CardOnFieldContainer>().ExitMouseOver();
            lastHoveredPile = null;
            return;
        }
        CardOnFieldContainer container = pile.GetComponent<CardOnFieldContainer>();
        lastHoveredPile = pile;
        if(lastHoveredPile != null)
        {
            lastHoveredPile.GetComponent<CardOnFieldContainer>().ExitMouseOver();
        }
        container.EnterMouseOver();
    }

    Transform RaycastForPile()
    {
        
        int pileLayer = LayerMask.NameToLayer("CardHolder");
        int layerMask = (1 << pileLayer);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Transform hitObject = hit.collider.transform;
            return hitObject;
        }

        
        return null;
    }

    void FixedUpdate()
    {
        
    }
}
