using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCardToController : MonoBehaviour
{
    public TMP_InputField input;

    public Card card;
    public void InitMenu(OnFieldCardRightClickController controller)
    {
        gameObject.SetActive(true);
        input.text = "0";
        this.card = controller.card;
    }

    public void InitMenu(CardInHandController controller)
    {
        gameObject.SetActive(true);
        input.text = "0";
        this.card = controller.card;
    }
    
}
