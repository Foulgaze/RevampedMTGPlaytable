using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardInHandController : MonoBehaviour
{
    public Card card;
    public void InitMenu(Card card)
    {
        this.card = card;
        transform.gameObject.SetActive(true);
    }

}