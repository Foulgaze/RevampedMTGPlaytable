using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DisplayDeckController : MonoBehaviour, CardContainer
{
    [SerializeField]
    Transform cardIconPrefab;    

    [SerializeField]
    GridLayoutGroup layout;

    [SerializeField]
    RectTransform scrollView;

    [SerializeField]
    TextMeshProUGUI boxName;

    [SerializeField]
    Transform hiddenCardPrefab;


    public RectTransform cardHolder;
    public bool interactable;

    int cardsPerRow = 5;
    float cardRatio = 5f/7;

    [HideInInspector]
    public Vector2 cardDimensions;

    [HideInInspector]
    public Deck currentDeck;
    bool renderEntireLibrary = false;
    List<Card> cardsToRender;
    List<Transform> hiddenCards = new List<Transform>();
    Transform unusedCardHolder;

    void Start()
    {
        unusedCardHolder = GameManager.Instance._uiManager.unusedCardHolder;
        cardHolder = layout.transform.GetComponent<RectTransform>();
    }


    public void LoadPile(Deck deck, List<Card> cardsToRender, bool interactable)
    {
        if(deck == null)
        {
            Debug.LogError("Unable to load deck");
        }
        gameObject.SetActive(true);
        this.interactable = interactable;
        renderEntireLibrary = cardsToRender == null;
        Cleanup();
        this.cardsToRender = cardsToRender;
        this.boxName.text = deck.name;
        currentDeck = deck;
        boxName.text = GetName(currentDeck.deckID);
        if(renderEntireLibrary)
        {
            RenderLibrary(deck.cards);
        }
        else
        {
            RenderTopXCards(deck.cards);
        }
    }

    void Cleanup()
    {
        DestroyHiddenCards();
        CleanupChildren();
        cardsToRender = null;
    }

    void DestroyHiddenCards()
    {
        if(hiddenCards == null)
        {
            return;
        }
        foreach(Transform t in hiddenCards)
        {
            Destroy(t.gameObject);
        }
        hiddenCards.Clear();
    }

    void CleanupChildren()
    {
        List<Transform> children = new List<Transform>();
        foreach(Transform card in cardHolder.transform)
        {
            children.Add(card);
        }
        foreach(Transform card in children)
        {
            card.SetParent(unusedCardHolder);
        }
    }

    string GetName(Piletype deckid)
    {
        switch(deckid)
        {
            default: return "Unknown";
            case Piletype.library: return "Library";
            case Piletype.graveyard: return "Graveyard";
            case Piletype.exile: return "Exile";
        }
    }

    public void UpdateHolder()
    {
        DestroyHiddenCards();
        if(renderEntireLibrary)
        {
            RenderLibrary(currentDeck.cards);
        }
        else
        {
            RenderTopXCards(currentDeck.cards);
        }
    }
    public void RenderLibrary(List<Card> cards)
    {
        List<Card> cardsCopy = new List<Card>(cards);
        cardsCopy = cardsCopy.OrderBy(x=>x.name).ToList();
        SetGridValues(cardsCopy.Count);
        for(int i = 0; i < cardsCopy.Count; ++i)
        {
            Card card = cardsCopy[i];
            RectTransform newCardIcon = card.GetInHandRect(interactable);
            card.EnableRect();
            SetupCardForHolder(newCardIcon);
            newCardIcon.transform.SetParent(layout.transform);
            newCardIcon.SetSiblingIndex(i);
        }
    }

    public void RenderTopXCards(List<Card> allCards)
    {
        List<Card> cardCopy = new List<Card>(allCards);
        cardCopy.Reverse();
        SetGridValues(allCards.Count);
        for(int i = 0; i < allCards.Count; ++i)
        {
            Card card = cardCopy[i];
            RectTransform newCardIcon;
            if(cardsToRender.Contains(card))
            {
                newCardIcon = card.GetInHandRect(interactable);
                card.EnableRect();
            }
            else
            {
                Transform newCard = Instantiate(hiddenCardPrefab);
                hiddenCards.Add(newCard);
                newCardIcon = newCard.GetComponent<RectTransform>();
            }
            SetupCardForHolder(newCardIcon);
            newCardIcon.SetSiblingIndex(i);
        }
    }

    void SetupCardForHolder(RectTransform rt)
    {
        rt.sizeDelta = layout.cellSize;
        rt.transform.SetParent(layout.transform);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
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
        if(cardHolder.childCount == 0)
        {
            AddCardToContainer(card, null);
            return;
        }
        AddCardToContainer(card, Math.Min(currentDeck.cards.Count(), cardHolder.childCount - FindClosestTransform()));
    }

    int FindClosestTransform()
    {
        Vector2 mousePosition = Input.mousePosition;
        float currentMinDistance = float.MaxValue;
        int currentIndex = 0;
        for(int i = 0; i < cardHolder.childCount; ++i)
        {
            Transform currentChild = cardHolder.GetChild(i);
            float currentDistance = Vector3.Distance(currentChild.position, mousePosition);
            if(currentDistance < currentMinDistance)
            {
                currentMinDistance = currentDistance;
                currentIndex = i;
                if(currentChild.transform.position.x < mousePosition.x)
                {
                    currentIndex += 1;
                }
            }
        }
        if(currentMinDistance > layout.cellSize.y/2)
        {
            return cardHolder.childCount;
        }
        return currentIndex;
    }
}
