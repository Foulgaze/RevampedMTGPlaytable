using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PileCardDisplay : MonoBehaviour
{
    public bool mouseInBox = false;

    public LibraryBoxController libraryController;
    public void OnMouseEnter()
    {
        Debug.Log("Entering");
        if(!GameManager.Instance.handManager.IsHoldingCard())
        {
            return;
        }
        SetupRectForHand(GameManager.Instance.handManager.heldCard);
        // libraryController.currentDeck.AddCardToContainer(GameManager.Instance.handManager.heldCard, null);
    }

    public void SetupRectForHand(Card card)
    {
        RectTransform rt = card.GetInHandRect();
        // rt.SetParent(_handParent);
        rt.sizeDelta = libraryController.cardDimensions;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        card.EnableRect();
    }

    public void OnMouseExit()
    {
        Debug.Log("Exiting");
        Card heldCard = GameManager.Instance.handManager.heldCard;
        GameManager.Instance.handManager.SetupRectForHand(heldCard.GetInHandRect(), heldCard);
    }
}
