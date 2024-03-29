using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DrawCard()
    {
        if(GameManager.Instance.clientPlayer.DrawCard())
        {
            GameManager.Instance.SendUpdatedDecks();
        }
    }

    public void DrawCards(TMP_InputField userInput)
    {
        int cardsToDrawCount;
        string text = userInput.text;
        userInput.text = "";
        if(text == "")
        {
            return;
        }
        if(!int.TryParse(text, out cardsToDrawCount))
        {
            Debug.LogError($"Unable To Parse Draw Card Value [{text}] - {text.Count()}");
            return;
        }

        if(cardsToDrawCount == 0)
        {
            return;
        }

        for(int i = 0; i < cardsToDrawCount; ++i)
        {
            GameManager.Instance.clientPlayer.DrawCard();
        }
        GameManager.Instance.SendUpdatedDecks();
        
    }

    bool IsHoldingCTRL()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    public void ShuffleDeck()
    {
        GameManager.Instance.clientPlayer.library.Shuffle();
    }

    public void ViewLibrary()
    {
        Transform library = GameManager.Instance._uiManager.ShowLibrary();
        LibraryBoxController controller = library.GetComponent<LibraryBoxController>();
        controller.LoadPile(GameManager.Instance._uiManager.currentSelectedDeck);
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance._gameStarted || !GameManager.Instance.gameInteractable)
        {
            return;
        }
        if(IsHoldingCTRL() && Input.GetKeyDown(KeyCode.D) && GameManager.Instance._gameStarted)
        {
           DrawCard();
        }
        if(IsHoldingCTRL() && Input.GetKeyDown(KeyCode.E) && GameManager.Instance._gameStarted)
        {
           GameManager.Instance._uiManager.EnableDrawCardsBox();
        }
        if(IsHoldingCTRL() && Input.GetKeyDown(KeyCode.S))
        {
            ShuffleDeck();
        }
    }
}
