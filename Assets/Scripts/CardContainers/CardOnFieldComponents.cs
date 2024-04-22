using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;


public class CardOnFieldComponents : MonoBehaviour
{
    public Transform leftSide;
    public Transform rightSide;
    public Transform tappedSymbol;
    public Image cardArt;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardPowerToughness;

    public void SetPowerToughnessState(bool enabled)
    {
        cardPowerToughness.transform.parent.gameObject.SetActive(enabled);
    }

    
}
