using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangePowerToughnessController : MonoBehaviour
{
    [SerializeField]
    TMP_InputField powerField;

    [SerializeField]
    TMP_InputField toughnessField;
    Card card;

    public void InitMenu(Card card)
    {
        this.card = card;
        powerField.text = $"{card.power}";
        toughnessField.text = $"{card.toughness}";
    }
    public void IncreasePowerToughness(TMP_InputField input)
    {
        int newValue = HelperFunctions.ChangeIntputField(input.text, true);
        SetNewValue(input, newValue);
    }
    public void DecreasePowerToughness(TMP_InputField input)
    {
        int newValue = HelperFunctions.ChangeIntputField(input.text, false);
        SetNewValue(input, newValue);
    }

    public void UpdatePowerToughness(TMP_InputField input)
    {
        int newValue = HelperFunctions.ChangeIntputField(input.text, null);
        SetNewValue(input, newValue);
    }

    void SetNewValue(TMP_InputField input, int newValue)
    {
        input.text = $"{newValue}";
        bool changingPower = input == powerField;
        if(changingPower)
        {
            card.power = newValue;
        }
        else
        {
            card.toughness = newValue;
        }
        GameManager.Instance.SendChangePowerToughness(changingPower, card, newValue);
        // card.DisplayPowerToughness(true);
    }

}
