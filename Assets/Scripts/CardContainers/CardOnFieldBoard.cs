using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardOnFieldBoard : MonoBehaviour, CardContainer, RaycastableHolder
{
    List<Card> cards = new List<Card>();
    bool mouseInTile = false;

    float percentageOfCardAsSpacer = 0.25f;
    public int owner {get;set;} = -1;
    float widthToHeightRatio = 4/3f;

    public int cardCount {get;set;}

    Card hoveredCard;

    Vector3 cardDimensions;
    Vector3 containerDimension;
    Vector3 tilePosition;

    public void SetValues()
    {
        cardDimensions = HelperFunctions.GetExtents(GameManager.Instance.onFieldCard);
        containerDimension = HelperFunctions.GetExtents(transform);
        tilePosition = transform.position;
    }
    public void AddCardToContainer(Card card, int? position)
    {
        if(position == null)
        {
            cards.Add(card);
            UpdateCardPositions();
            return;
        }
        int insertPosition = Mathf.Min(cards.Count, (int)position);
        cards.Insert(insertPosition, card);
        UpdateCardPositions();
    } 

    void UpdateCardPositions()
    {
        int currentCardCount = Math.Max(cards.Count, cardCount);
        float cardWidth = transform.lossyScale.x / (currentCardCount + (currentCardCount - 1) * percentageOfCardAsSpacer);
        float totalWidth = (cardWidth * cards.Count) + (cardWidth * percentageOfCardAsSpacer * (cards.Count - 1));
        Vector3 iterPosition = transform.position + new Vector3(-totalWidth / 2 + cardWidth / 2, 0, 0);

        foreach(Card card in cards)
        {
            if(card == null)
            {
                Debug.LogError("Invalid card");
                continue;
            }
            Transform onFieldCard = card.GetCardOnField();
            if(onFieldCard == null)
            {
                Debug.LogError("OnFieldCard Can't be loaded");
                continue;
            }
            onFieldCard.localScale = new Vector3(cardWidth, onFieldCard.localScale.y, cardWidth * 1/widthToHeightRatio);
            onFieldCard.transform.position = iterPosition;
            iterPosition += new Vector3(cardWidth + cardWidth * percentageOfCardAsSpacer, 0, 0);
        }
    }

    public DeckDescriptor GetDeckDescription(Piletype id)
    {
        List<int> cardsInHand = new List<int>();
        foreach(Card card in cards)
        {
            cardsInHand.Add(card.id);
        }
        return new DeckDescriptor(0,0,(int) id,false,cardsInHand);
    }
    public int GetOwner()
    {
        return owner;
    }

    public void RemoveCardFromContainer(Card card)
    {
        cards.Remove(card);
        UpdateCardPositions();
    }

    public void EnterMouseOver()
    {
        mouseInTile = true;
        if(!GameManager.Instance.handManager.IsHoldingCard() || GameManager.Instance.clientPlayer.id != GetOwner())
        {
            return;
        }
        hoveredCard = GameManager.Instance.clientPlayer.hand.heldCard;
        PrepareForHover();
    }

    public void UpdateDeck(DeckDescriptor deck)
    {
        cards.Clear();
        foreach(int cardNumber in deck.cards)
        {
            Card card = GameManager.Instance.idToCard[cardNumber];
            if(card is null)
            {
                Debug.LogError($"Could not load card id {cardNumber}");
            }
            card.EnableOnFieldCard();
            card.DisableRect();
            cards.Add(card);
        }
        UpdateCardPositions();
    }

    void PrepareForHover()
    {
        hoveredCard.DisableRect();
        Transform cardOnField = hoveredCard.GetCardOnField();
        hoveredCard.EnableOnFieldCard();
    }
    public void ExitMouseOver()
    {
        mouseInTile = false;
        if(!GameManager.Instance.handManager.IsHoldingCard() || GameManager.Instance.clientPlayer.id != GetOwner())
        {
            return;
        }
        hoveredCard = GameManager.Instance.clientPlayer.hand.heldCard;
        hoveredCard.EnableRect();
        hoveredCard.DisableOnFieldCard();
        hoveredCard = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mouseInTile && GameManager.Instance.handManager.IsHoldingCard())
        {
            GameManager.Instance.handManager.heldCard.GetCardOnField().transform.position = GameManager.Instance.handManager.lastHitPosition;
        }
    }

    public void ReleaseCardInBox(Card insertCard)
    {
        if(cards.Count == 0)
        {
            AddCardToContainer(insertCard, null);
        }
        else
        {
            Vector3 mousePosition = GameManager.Instance.handManager.lastHitPosition;
            int insertPosition = 0;
            float smallestDistance = float.MaxValue;
            for(int i = 0; i < cards.Count; ++i)
            {
                Card card = cards[i];
                CardOnFieldComponents components = card.GetCardOnField().GetComponent<CardOnFieldComponents>();
                float leftDistance = Vector3.Distance(mousePosition, components.leftSide.transform.position);
                float rightDistance = Vector3.Distance(mousePosition, components.rightSide.transform.position);
                if(leftDistance < smallestDistance)
                {
                    smallestDistance = leftDistance;
                    insertPosition = i;
                }
                if(rightDistance < smallestDistance)
                {
                    smallestDistance = rightDistance;
                    insertPosition = i + 1;
                }
            }
            AddCardToContainer(insertCard, insertPosition);
        }
        hoveredCard = null;
    }
}
