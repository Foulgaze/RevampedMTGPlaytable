using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
// TODO
// The hand manager, and card moving functionality should not be in the same file.
// Should separate into hand container, and card mover controller. 
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

    [SerializeField]
    DeckDisplay deckDisplay;


    [SerializeField]
    RectTransform pileViewBox;
    Vector2 _cardDimensions;
    float _cardRatio = 7f/5f;
    int _cardsPerHand = 5;
    int _cardYPositionFraction = 3;

    public Card heldCard = null;   
    public Card hoveredCard = null;
    public Vector2 offset;

    public Vector3 lastHitPosition;

    Transform lastHoveredPile = null;

    bool inDeckDisplayView = false;

    public void SetValues()
    {
        _boxExtents = new Vector2(_handBox.rect.width, _handBox.rect.height);
        _cardDimensions = new Vector2(_boxExtents.x/_cardsPerHand, _boxExtents.x/_cardsPerHand * _cardRatio );
        _checkHandBox.sizeDelta = new Vector2(_boxExtents.x,_cardDimensions.y);
        _checkHandBox.transform.position = new Vector3(_handBox.transform.position.x,_handBox.transform.position.y -_cardDimensions.y/_cardYPositionFraction,0);
    }

    public bool CardInHand(Card checkCard)
    {
        return cards.Contains(checkCard);
    }

    public bool IsHoldingCard()
    {
        return heldCard != null;
    }

    bool CardInDeckDisplayView()
    {
        return pileViewBox.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(pileViewBox, Input.mousePosition);
    }

    public void BeginCardDrag(Card card, Vector2 offset)
    {
        if(CardInDeckDisplayView() && !GameManager.Instance._uiManager.cardContainerController.interactable)
        {
            return;
        }
        bool cardInHand = CardInHand(card);
        heldCard = card;
        heldCard.ResetPivot();
        this.offset = cardInHand ? offset : Vector2.zero;
        card.currentLocation.RemoveCardFromContainer(card);
        if(CardInDeckDisplayView())
        {
            DisplayContainerController controller = GameManager.Instance._uiManager.cardContainerDisplayHolder.GetComponent<DisplayContainerController>();
            card.GetInHandRect().SetParent(_handParent);
            inDeckDisplayView = true;
        }
    }
    

    public void ReleaseCardInBox(Card card)
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

    public void ReleaseCard() // Honestly this whole function should be in a separate file.
    {
        bool insideHandBox = HelperFunctions.IsPointInRectTransform(Input.mousePosition, _checkHandBox, null);
        if(heldCard.ethereal) // TODO Refactor, ugly
        {
            bool released = false;
            if(lastHoveredPile != null)
            {
                CardContainer container = lastHoveredPile.GetComponent<CardContainer>();
                if(container != null)
                {
                    container.ReleaseCardInBox(heldCard);
                    released = true;
                }
            }
            if(!released)
            {
                GameManager.Instance.SendDestroyCard(heldCard.id);
                hoveredCard = null;
            }
        }
        else if(insideHandBox)
        {
            ReleaseCardInBox(heldCard);
            UpdateContainer();
            heldCard.GetInHandRect().GetComponent<CardMover>().HoverCard();
        }
        else if(inDeckDisplayView)
        {
            deckDisplay.deckDisplayController.ReleaseCardInBox(heldCard);
        }
        else if(lastHoveredPile == null)
        {
            AddCardToContainer(heldCard, null);
            UpdateContainer();
        }
        else
        {
            CardContainer container = lastHoveredPile.GetComponent<CardContainer>();
            if(container != null)
            {
                container.ReleaseCardInBox(heldCard);
            }
            else
            {
                CardOnFieldContainer onFieldContainer = lastHoveredPile.GetComponent<CardOnFieldContainer>();
                if(onFieldContainer != null)
                {
                    onFieldContainer.EnterMouseOver();
                }
                else
                {
                    AddCardToContainer(heldCard, null);
                    UpdateContainer();
                }
            }
        }
        lastHoveredPile = null;
        inDeckDisplayView = false;
        heldCard = null;
        offset = Vector2.zero;
        GameManager.Instance.SendUpdatedDecks();
    }
    
    public void AddCardToContainer(Card card, int? position)
    {
        card.ClearStats();
        card.currentLocation = this;
        int inputPosition = position == null ? cards.Count : (int)position;
        SetupRectForHand(card.GetInHandRect(), card);
        cards.Insert(inputPosition,card);
        UpdateContainer();
    }

    public void SetupRectForHand(RectTransform rt, Card card)
    {
        card.ResetPivot();
        rt.SetParent(_handParent);
        rt.sizeDelta = _cardDimensions;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.transform.position = (Vector2) Input.mousePosition - offset;
        card.EnableRect();
    }
    public void RemoveCardFromContainer(Card card)
    {
        cards.Remove(card);
        UpdateContainer();
    }

    public int GetOwner()
    {
        return GameManager.Instance.clientPlayer.id;
    }




    public void UpdateContainer()
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
        if(lastHoveredPile != null && !heldCard.ethereal)
        {
            return;
        }
        heldCard.GetInHandRect().transform.position = (Vector2) Input.mousePosition - offset;
    }

    bool IsDraggingCard()
    {
        return IsHoldingCard() && Input.GetMouseButton(0);
    }

    bool IsReleasingCard()
    {
        return IsHoldingCard() && !Input.GetMouseButton(0);
    }

    bool IsRightClickingPile()
    {
        return Input.GetMouseButtonDown(1) && !IsHoldingCard() && lastHoveredPile != null && lastHoveredPile.GetComponent<CardOnFieldBoard>() == null;
    }
    
    bool IsRightClickingNothing()
    {
        return Input.GetMouseButtonDown(1) && !IsHoldingCard() && (lastHoveredPile == null || (lastHoveredPile != null && lastHoveredPile.GetComponent<CardOnFieldBoard>() != null)) && hoveredCard == null;
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

        if(IsRightClickingPile())
        {
            CardOnFieldContainer cardOnFieldContainer = lastHoveredPile.GetComponent<CardOnFieldContainer>();
            if(cardOnFieldContainer.deck.deckID == Piletype.graveyard || cardOnFieldContainer.deck.deckID == Piletype.exile)
            {
                GameManager.Instance._uiManager.EnableLibraryRightClickMenu(lastHoveredPile.GetComponent<CardOnFieldContainer>().deck, GameManager.Instance._uiManager.genericPileRightClickMenu);
            }
            else
            {
                GameManager.Instance._uiManager.EnableLibraryRightClickMenu(lastHoveredPile.GetComponent<CardOnFieldContainer>().deck, GameManager.Instance._uiManager.libraryPileRightClickMenu);
            }
        }
        if (IsRightClickingNothing())
        {
            GameManager.Instance._uiManager.EnableDefaultMenu();
        }
        CheckForPile();
        
    }

    // TODO Should probably be its own file. Also very ugly must be a better way to write
    void CheckForPile()
    {
        if(IsHoldingCard() && CardInDeckDisplayView() && GameManager.Instance._uiManager.cardContainerController.interactable)
        {
            if(lastHoveredPile != null)
            {
                lastHoveredPile.GetComponent<RaycastableHolder>().ExitMouseOver();
                lastHoveredPile = null;
            }
            if(!inDeckDisplayView)
            {
                deckDisplay.OnMouseEnter();
                inDeckDisplayView = true;
            }
            return;
        }
        if(IsHoldingCard() && inDeckDisplayView)
        {
            deckDisplay.OnMouseExit();
            inDeckDisplayView = false;
        }
        Transform pile = RaycastForPile();
        if(lastHoveredPile == pile)
        {
            return;
        }
        if(lastHoveredPile != null)
        {
            lastHoveredPile.GetComponent<RaycastableHolder>().ExitMouseOver();
        }
        if(pile == null || pile.GetComponent<RaycastableHolder>().GetOwner() != GetOwner())
        {
            lastHoveredPile = null;
            return;
        }
        lastHoveredPile = pile;
        pile.GetComponent<RaycastableHolder>().EnterMouseOver();
    }

    Transform RaycastForPile()
    {
        
        int pileLayer = LayerMask.NameToLayer("CardHolder");
        int layerMask = 1 << pileLayer;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
    {
            lastHitPosition = hit.point;
            Transform hitObject = hit.collider.transform;
            return hitObject;
        }

        
        return null;
    }

    public string GetName()
    {
        return "Hand";
    }

    public List<Card> GetCards()
    {
        return cards;
    }

}
