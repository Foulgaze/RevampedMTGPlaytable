using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LibraryInfoStorage : MonoBehaviour
{
    public TMP_InputField input;

    public void ClearInput()
    {
        input.text = "";
    }
    [HideInInspector]

    public Deck currentDeck;
}
