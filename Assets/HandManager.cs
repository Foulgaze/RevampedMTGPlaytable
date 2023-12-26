using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeReference]
    Transform _cardInHandPrefab;
    [SerializeReference]
    RectTransform _handBox; 
    List<RectTransform> _cardRects;

    float _widthRatio = 0.7159f;

    void Start()
    {
        _cardRects = new List<RectTransform>();
    }
    void AddCardToHand(Transform card, int position)
    {
        card.SetParent(_handBox.transform);
        card.GetComponent<CardInHandManager>()._handManager = this;
        _cardRects.Insert(position, card.GetComponent<RectTransform>());
        UpdateHand();
    }

    void UpdateHand()
    {
        if(_cardRects.Count <= 0)
        {
            return;
        }

        Vector2 cardSize = new Vector2(_handBox.rect.height * _widthRatio, _handBox.rect.height);
        float totalCardWidth = cardSize.x * _cardRects.Count;
        float space = totalCardWidth <= _handBox.rect.width ? cardSize.x : (_handBox.rect.width - cardSize.x) / (_cardRects.Count - 1);
        float xCurrentPosition = Mathf.Max(-1 * (space / 2 * (_cardRects.Count - 1)), _handBox.rect.width / -2 + cardSize.x / 2);
        for(int cardPosition = 0; cardPosition < _cardRects.Count; ++cardPosition)   
        {
            RectTransform currentCard = _cardRects[cardPosition];
            currentCard.sizeDelta = cardSize;
            currentCard.anchoredPosition = new Vector2(xCurrentPosition,0);
            xCurrentPosition += space;
        }   
    }

    public void ReorderHand()
    {
        for(int cardPosition = 0; cardPosition < _cardRects.Count; ++cardPosition)   
        {
            RectTransform currentCard = _cardRects[cardPosition];
            currentCard.SetSiblingIndex(cardPosition);
        }   
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            AddCardToHand(GameObject.Instantiate(_cardInHandPrefab), _cardRects.Count);
        }
    }
}
