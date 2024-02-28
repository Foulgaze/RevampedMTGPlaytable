using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
public class HandManager : MonoBehaviour
{
    List<RectTransform> _cardRects = new List<RectTransform>();

    [SerializeField]
    RectTransform _handBox;
    [SerializeField]
    Transform _handParent;
    [SerializeField]
    RectTransform _checkHandBox;
    [SerializeField]
    Transform _cardInHand;
    [SerializeField]
    Transform _customCard;
    [SerializeField]
    Vector2 _boxExtents = Vector2.zero;
    Vector2 _cardDimensions;
    float _cardRatio = 7f/5f;
    int _cardsPerHand = 5;
    int _cardYPositionFraction = 3;

    [SerializeField]
    Image debugImage;

    public RectTransform heldCard = null;   

    Transform lastHoveredPile = null;


    void Start()
    {


    }

    public void SetValues()
    {
        _boxExtents = new Vector2(_handBox.rect.width, _handBox.rect.height);
        _cardDimensions = new Vector2(_boxExtents.x/_cardsPerHand, _boxExtents.x/_cardsPerHand * _cardRatio );
        _checkHandBox.sizeDelta = new Vector2(_boxExtents.x,_cardDimensions.y);
        _checkHandBox.transform.position = new Vector3(_handBox.transform.position.x,_handBox.transform.position.y -_cardDimensions.y/_cardYPositionFraction,0);

        
    }

    public bool IsHoldingCard()
    {
        return heldCard != null;
    }

    public void DragCard(RectTransform newHeldCard)
    {
        Camera c = Camera.main;
        heldCard = newHeldCard;
        _cardRects.Remove(newHeldCard);
        UpdateCardPositions();

        newHeldCard.transform.GetComponent<Image>().raycastTarget = false;
    }
    

    void AddCardToHand(RectTransform card)
    {
        for(int i = _cardRects.Count - 1; i > -1; --i)
        {
            if(HelperFunctions.IsPointInRectTransform(Input.mousePosition, _cardRects[i], null))
            {
                i = Input.mousePosition.x > _cardRects[i].transform.position.x ? i + 1 : i;
                _cardRects.Insert(Mathf.Max(i,0),card);
                UpdateCardPositions();
                return;
            }
        }
        int insertPosition = Input.mousePosition.x < Screen.width/2 ? 0 : _cardRects.Count;
        _cardRects.Insert(insertPosition,card);
    }

    public void ReleaseCard(RectTransform rt)
    {
        heldCard = null;
        bool inside = HelperFunctions.IsPointInRectTransform(Input.mousePosition, _checkHandBox, null);
        if(inside)
        {
            AddCardToHand(rt);
        }
        else
        {
            if(lastHoveredPile != null)
            {
                Debug.Log("Destroyin");
                Destroy(rt.transform);
                lastHoveredPile = null;
            }
            else
            {
                AddCardToEndOfHand(rt);
            }
        }
        // UpdateHoverCardPosition(rt);
        UpdateCardPositions();
    }
    
    public void AddCardToEndOfHand(Transform newCard)
    {


        RectTransform cardRect = newCard.GetComponent<RectTransform>();
        cardRect.sizeDelta = _cardDimensions;
        // newCard.GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
        newCard.SetParent(_handParent);
        newCard.GetComponent<CardMover>()._handManager = this;
        _cardRects.Add(cardRect);
        
        UpdateCardPositions();
    }

    public void CreateAndAddCardToHand(string cardName)
    {
        Transform newCard = Instantiate(_cardInHand);
        Image image = newCard.GetComponent<Image>();
        if(!GameManager.Instance.nameToCardInfo.ContainsKey(cardName))
        {
            return;
        }
        Sprite cardSprite = GameManager.Instance.nameToCardInfo[cardName].GetCardSprite(image);
        CardInfo info = GameManager.Instance.nameToCardInfo[cardName];
        if(cardSprite == null)
        {
            newCard = Instantiate(_customCard);
            SetCardValues(info,newCard.GetComponent<CardComponents>());
        }
        else
        {
            image.sprite = cardSprite;
        }
        newCard.GetComponent<CardMover>().info = info;
        AddCardToEndOfHand(newCard);
    }

    void SetCardValues(CardInfo info, CardComponents components)
    {
        components.cardDescription.text = info.text;
        components.cardName.text = info.name;
        components.cardType.text = info.type;
        components.manaCost.text = info.manaCost;
    }

    public void UpdateCardPositions()
    {

        float distanceBetweenCards = _cardRects.Count <= _cardsPerHand ? _cardDimensions.x : (_boxExtents.x - _cardDimensions.x) / (_cardRects.Count - 1);
        float startPos = -_cardDimensions.x/2 * (_cardRects.Count-1);

        float currentXPosition = Mathf.Max(-_boxExtents.x/2 + _cardDimensions.x/2, startPos);
        foreach(RectTransform cardRect in _cardRects)
        {
            cardRect.anchoredPosition = new Vector2(currentXPosition, -_cardDimensions.y/_cardYPositionFraction);
            currentXPosition += distanceBetweenCards;
        }
        UpdateCardSiblingPositions();
    }

    public void UpdateCardSiblingPositions()
    {
        for(int i = 0; i < _cardRects.Count; ++i)
        {
            _cardRects[i].SetSiblingIndex(i);
        }
    }

    public void UpdateHoverCardPosition( RectTransform hoverCardRect)
    {
        if(_cardRects.Count <= _cardsPerHand || hoverCardRect == _cardRects.Last())
        {
            return;
        }
        float distanceBetweenCards = _cardRects.Count <= _cardsPerHand ? _cardDimensions.x : (_boxExtents.x - _cardDimensions.x * 2) / (_cardRects.Count - 2);
        float startPos = -_cardDimensions.x/2 * (_cardRects.Count-1);

        float currentXPosition = Mathf.Max(-_boxExtents.x/2 + _cardDimensions.x/2, startPos);
        foreach(RectTransform cardRect in _cardRects)
        {
            
            cardRect.anchoredPosition = new Vector2(currentXPosition, cardRect.anchoredPosition.y);
            currentXPosition += cardRect == hoverCardRect ? _cardDimensions.x : distanceBetweenCards;
        }
    }


    void Update()
    {    
    }

    void CheckForPile()
    {
        if(heldCard == null)
        {
            if(lastHoveredPile != null)
            {
                lastHoveredPile.GetComponent<CardOnFieldContainer>().ExitMouseOver();
                lastHoveredPile = null;
            }
            return;
        }
        Transform pile = RaycastForPile();
        if(pile == null)
        {
            if(lastHoveredPile != null)
            {
                lastHoveredPile.GetComponent<CardOnFieldContainer>().ExitMouseOver();
                lastHoveredPile = null;
            }
        }
        if(lastHoveredPile == pile)
        {
            return;
        }
        CardOnFieldContainer container = pile.GetComponent<CardOnFieldContainer>();
        if(lastHoveredPile == null)
        {
            Debug.Log("Setting Pile");
            container.EnterMouseOver();
            lastHoveredPile = pile;
            return;
        }
        
        lastHoveredPile.GetComponent<CardOnFieldContainer>().ExitMouseOver();
        container.EnterMouseOver();
        lastHoveredPile = pile;
        return;


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
        CheckForPile();
        
    }
}
