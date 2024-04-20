using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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
        if(GameManager.Instance._uiManager.libraryHolder.gameObject.activeInHierarchy)
        {
            HideLibrary(GameManager.Instance._uiManager.libraryHolder.GetComponent<DisplayDeckController>());
        }
        Transform library = GameManager.Instance._uiManager.ShowLibrary();
        library.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        DisplayDeckController controller = library.GetComponent<DisplayDeckController>();
        controller.LoadPile(GameManager.Instance._uiManager.currentSelectedDeck, null, true);
    }

    public void ViewTopXLibrary(LibraryInfoStorage storage)
    {
        if(GameManager.Instance._uiManager.libraryHolder.gameObject.activeInHierarchy)
        {
            HideLibrary(GameManager.Instance._uiManager.libraryHolder.GetComponent<DisplayDeckController>());
        }
        int cardCount;
        if(!Int32.TryParse(storage.input.text, out cardCount))
        {
            return;
        }
        List<Card> deckCards = storage.currentDeck.cards;
        cardCount = Math.Min(cardCount,deckCards.Count());
        Transform library = GameManager.Instance._uiManager.ShowLibrary();
        library.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        DisplayDeckController controller = library.GetComponent<DisplayDeckController>();
        List<Card> viewableCards = deckCards.GetRange(deckCards.Count - cardCount, cardCount);
        controller.LoadPile(storage.currentDeck, viewableCards, true);
    }

    public void HideLibrary(DisplayDeckController controller)
    {
        controller.gameObject.SetActive(false);
        controller.currentDeck.UpdatePhysicalDeck();
        controller.currentDeck = null;
        Deck currentlySelected = GameManager.Instance._uiManager.currentSelectedDeck;
        if(currentlySelected != null)
        {
            currentlySelected.UpdatePhysicalDeck();
        }
    }

    public void MoveTopCardToBottom()
    {
        Player currentPlayer = GameManager.Instance.clientPlayer;
        if(currentPlayer.library.MoveTopCardToBottom())
        {
            GameManager.Instance.SendUpdatedDecks();
        }
    }

    public void MillXCards(TMP_InputField userInput)
    {
        TransferXCards(userInput, GameManager.Instance.clientPlayer.graveyard);
    }

     public void ExileXCards(TMP_InputField userInput)
    {
        TransferXCards(userInput, GameManager.Instance.clientPlayer.exile);
    }

    public void TransferXCards(TMP_InputField userInput, Deck resultDeck)
    {
        int result;
        if(!int.TryParse(userInput.text, out result))
        {
            return;
        }
        if(result == 0)
        {
            return;
        }
        for(int i = 0; i < result ;++i)
        {
            Card? drawnCard = GameManager.Instance.clientPlayer.library.DrawCard();
            if(drawnCard == null)
            {
                break;
            }
            resultDeck.AddCard(drawnCard);
        }
        GameManager.Instance.SendUpdatedDecks();
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
