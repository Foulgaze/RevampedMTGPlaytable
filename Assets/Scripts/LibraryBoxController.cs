using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
public class LibraryBoxController : MonoBehaviour, CardContainer
{
    [SerializeField]
    Transform cardIconPrefab;    

    [SerializeField]
    GridLayoutGroup layout;

    [SerializeField]
    RectTransform scrollView;

    public RectTransform cardHolder;


    int cardsPerRow = 5;
    float cardRatio = 5f/7;
    public Vector2 cardDimensions;

    public Deck currentDeck;
    List<Transform> heldCards = new List<Transform>();

    void Start()
    {
        cardHolder = layout.transform.GetComponent<RectTransform>();
    }


    public void LoadPile(Deck deck)
    {
        if(deck == null)
        {
            Debug.LogError("Unable to load deck");
        }
        currentDeck = deck;
        LoadCards(deck.cards);
    }
    public void LoadCards(List<Card> cards)
    {
        List<Card> cardsCopy = new List<Card>(cards);
        cardsCopy = cardsCopy.OrderBy(x=>x.info.name).ToList();
        SetGridValues(cardsCopy.Count);
        foreach(Card card in cardsCopy)
        {
            RectTransform newCardIcon = card.GetInHandRect();
            newCardIcon.sizeDelta = layout.cellSize;
            newCardIcon.transform.SetParent(layout.transform);
            card.EnableRect();
            // newCardIcon.GetChild(0).GetComponent<TextMeshProUGUI>().text = card.info.name;
            // heldCards.Add(newCardIcon);
        }
    }

    void SetGridValues(int cardCount)
    {
        Vector2 sizeDelta = cardHolder.sizeDelta;
        int spacingValue = (int) (sizeDelta.x * 0.025f);
        int paddingValue = (int) (sizeDelta.x * 0.035f);
        RectOffset padding = new RectOffset(paddingValue,paddingValue,paddingValue,paddingValue);
        Vector2 spacing = new Vector2(spacingValue, spacingValue);
        float totalHorizontalFiller = (spacingValue * (cardsPerRow - 1)) + (paddingValue * 2);
        float availableWidth = sizeDelta.x - totalHorizontalFiller;
        float cardWidth = availableWidth / cardsPerRow;
        float cardHeight = cardWidth * (1/cardRatio);
        layout.cellSize = new Vector2(cardWidth, cardHeight);
        layout.spacing = spacing;
        layout.padding = padding;
        cardDimensions = new Vector2(cardWidth, cardHeight);
        float totalHeight = paddingValue * 2 + (cardCount/cardsPerRow * cardHeight)  + (spacingValue * (cardCount - 1));
        totalHeight = Mathf.Max(totalHeight, scrollView.sizeDelta.y);
        cardHolder.sizeDelta = new Vector2(sizeDelta.x, totalHeight);
    }

    public void AddCardToContainer(Card card, int? position)
    {
        currentDeck.AddCardToContainer(card,position);
    }

    public void RemoveCardFromContainer(Card card)
    {
        currentDeck.RemoveCardFromContainer(card);
    }

    public int GetOwner()
    {
        return currentDeck.GetOwner();
    }

    public void ReleaseCardInBox(Card card)
    {
        AddCardToContainer(card, null);
        currentDeck.ReleaseCardInBox(card);
    }
}
