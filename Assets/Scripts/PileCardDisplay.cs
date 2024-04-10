using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileCardDisplay : MonoBehaviour
{
    public bool mouseInBox = false;

    public LibraryBoxController libraryController;

    Vector2 offset;
    public void OnMouseEnter()
    {
        Debug.Log("Entering");
        if(!GameManager.Instance.handManager.IsHoldingCard() || !libraryController.interactable)
        {
            return;
        }
        SetupRectForHand(GameManager.Instance.handManager.heldCard);
        // libraryController.currentDeck.AddCardToContainer(GameManager.Instance.handManager.heldCard, null);
    }

    public void SetupRectForHand(Card card)
    {
        RectTransform rt = card.GetInHandRect();
        offset = GameManager.Instance.handManager.offset;
        GameManager.Instance.handManager.offset = Vector2.zero;
        // rt.SetParent(_handParent);
        rt.sizeDelta = libraryController.cardDimensions;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        card.EnableRect();
    }

    public void OnMouseExit()
    {
        if(!GameManager.Instance.handManager.IsHoldingCard() || !libraryController.interactable)
        {
            return;
        }
        Debug.Log("Exiting");
        GameManager.Instance.handManager.offset = offset;
        Card heldCard = GameManager.Instance.handManager.heldCard;
        GameManager.Instance.handManager.SetupRectForHand(heldCard.GetInHandRect(), heldCard);
    }
}
