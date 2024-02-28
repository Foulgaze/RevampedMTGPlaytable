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
    Sprite _topSprite;
    int _cardCount = 0;
    void Awake()
    {
        _cardPileExtents = HelperFunctions.GetExtents(transform);
        _cardTopperExtents = HelperFunctions.GetExtents(GameManager.Instance.pileTopper);

    }
    public void SetCardCount(int count)
    {
        _cardCount = count;
        UpdateCardPositions();
    }

    public void SetTopper(Sprite topSprite)
    {
        this._topSprite = topSprite;
        SetTopCardSprite();
    }

    void SetTopCardSprite()
    {
        if(_cards.Count == 0)
        {
            return;
        }
        Transform topCard = _cards.Peek();
        Transform canvasObj = topCard.GetChild(0);
        
        // Transform imageObj = canvasObj.GetChild(0);
        // topCard.GetComponent<Renderer>().material = GameManager.Instance._defaultMat;
        // canvasObj.gameObject.SetActive(true);
        // imageObj.GetComponent<Image>().sprite = _topSprite;
    }
    Transform GenerateNewCard()
    {
        Transform newCard = GameObject.Instantiate(GameManager.Instance.pileTopper);
        newCard.GetComponent<Renderer>().material = GameManager.Instance.pilemat;
        newCard.localScale = transform.lossyScale;
        return newCard;
    }

  

    void UpdateCardPositions()
    {
        while(_cards.Count > _cardCount)
        {
            GameObject.Destroy(_cards.Pop().gameObject);
        }
        if(_cards.Count < _cardCount && _cards.Count != 0)
        {
            _cards.Peek().GetChild(0).gameObject.SetActive(false); // Disable Previous CardTopper
        }
        while(_cards.Count < _cardCount)
        {
            Transform newCard = GenerateNewCard();
            newCard.position = GetIthCardPosition(_cards.Count);
            _cards.Push(newCard);
        }
        SetTopCardSprite();
        
        
    }
    Vector3 GetIthCardPosition(int i)
    {
        return transform.position + new Vector3(0,_cardTopperExtents.y * i * 2f + _cardPileExtents.y*2,0);
    }
}
