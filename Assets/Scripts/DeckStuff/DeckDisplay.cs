using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckDisplay : MonoBehaviour
{
    

    public DisplayContainerController deckDisplayController;

    Vector2 offset;
    public void OnMouseEnter()
    {
        if(!GameManager.Instance.handManager.IsHoldingCard() || !deckDisplayController.interactable)
        {
            return;
        }
        SetupRectForHand(GameManager.Instance.handManager.heldCard);
    }

    public void SetupRectForHand(Card card)
    {
        RectTransform rt = card.GetInHandRect();
        offset = GameManager.Instance.handManager.offset;
        GameManager.Instance.handManager.offset = Vector2.zero;
        rt.sizeDelta = deckDisplayController.cardDimensions;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        card.EnableRect();
    }

    public void OnMouseExit()
    {
        if(!GameManager.Instance.handManager.IsHoldingCard() || !deckDisplayController.interactable)
        {
            return;
        }
        GameManager.Instance.handManager.offset = offset;
        Card heldCard = GameManager.Instance.handManager.heldCard;
        GameManager.Instance.handManager.SetupRectForHand(heldCard.GetInHandRect(), heldCard);
    }
}
