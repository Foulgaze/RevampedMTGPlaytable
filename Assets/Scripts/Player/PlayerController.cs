using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
/*
TO DO
Currently much of the UI is always present and is not instantiated at run time.
This is poor design as it causes much code reuse. What should really
happen is that there should be some class that can create basic input box menus for things
like draw x cards, mill x, exile x kind of deal. These menus should be instantiated then destroyed and not 
always present. 


*/
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
    

    public void ShuffleDeck()
    {
        GameManager.Instance.clientPlayer.library.Shuffle();
    }


    public void ViewCardContainer()
    {
        if(GameManager.Instance._uiManager.cardContainerDisplayHolder.gameObject.activeInHierarchy)
        {
            HideCardContainer(GameManager.Instance._uiManager.cardContainerDisplayHolder.GetComponent<DisplayContainerController>());
        }
        Transform library = GameManager.Instance._uiManager.ShowCardContainer();
        GameManager.Instance._uiManager.cardContainerController.LoadPile(GameManager.Instance._uiManager.currentSelectedDeck, null, true);
    }

    public void ViewTopXLibrary(LibraryInfoStorage storage)
    {
        if(GameManager.Instance._uiManager.cardContainerDisplayHolder.gameObject.activeInHierarchy)
        {
            HideCardContainer(GameManager.Instance._uiManager.cardContainerDisplayHolder.GetComponent<DisplayContainerController>());
        }
        int cardCount;
        if(!Int32.TryParse(storage.input.text, out cardCount))
        {
            return;
        }
        List<Card> deckCards = storage.currentDeck.cards;
        cardCount = Math.Min(cardCount,deckCards.Count());
        Transform library = GameManager.Instance._uiManager.ShowCardContainer();
        library.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        DisplayContainerController controller = library.GetComponent<DisplayContainerController>();
        List<Card> viewableCards = deckCards.GetRange(deckCards.Count - cardCount, cardCount);
        controller.LoadPile(storage.currentDeck, viewableCards, true);
    }

    public void HideCardContainer(DisplayContainerController controller)
    {
        controller.gameObject.SetActive(false);
        controller.UpdatePhysical();
        controller.currentContainer = null;
        CardContainer currentlySelected = GameManager.Instance._uiManager.currentSelectedDeck;
        if(currentlySelected is Deck)
        {
            ((Deck)currentlySelected).UpdateContainer();
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

    public void TapUntapCard(OnFieldCardRightClickController controller)
    {
        Card card = controller.card;
        if(card == null)
        {
            Debug.LogError("Null card passed to tap/untap");
            return;
        }
        card.TapUntap();
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

    public void MoveCardXFromTopOfDeck(MoveCardToController controller)
    {
        Player clientPlayer = GameManager.Instance.clientPlayer;
        int insertPosition;
        if(!int.TryParse(controller.input.text, out insertPosition))
        {
            Debug.LogError($"Unable to parse value - {controller.input.text}");
            return;
        }
        insertPosition = clientPlayer.library.cards.Count - insertPosition;
        MoveCardToDeck(GameManager.Instance.clientPlayer.library,controller.card, insertPosition);
    }
    public void MoveCardToTopOfDeck(MoveCardToController controller)
    {
        MoveCardToDeck(GameManager.Instance.clientPlayer.library,controller.card, null);
    }

    public void MoveCardToBottomOfDeck(MoveCardToController controller)
    {
        MoveCardToDeck(GameManager.Instance.clientPlayer.library,controller.card, 0);
    }
    
    public void MoveCardToExile(MoveCardToController controller)
    {
        MoveCardToDeck(GameManager.Instance.clientPlayer.exile, controller.card, null);
    }
    public void MoveCardToGraveyard(MoveCardToController controller)
    {
        MoveCardToDeck(GameManager.Instance.clientPlayer.graveyard, controller.card, null);
    }
    public void MoveCardToDeck(Deck deck,Card card, int? insertPosition)
    {
        if(card == null)
        {
            Debug.LogError("Card is null");
            return;
        }
        card.currentLocation.RemoveCardFromContainer(card);
        // if(!RemoveCardFromCurrentPosition(card))
        // {
        //     Debug.LogError("Unable to find card to remove");
        //     return;
        // }
        deck.AddCardToContainer(card, insertPosition);
        GameManager.Instance.SendUpdatedDecks();

    }

    

    public bool RemoveCardFromCurrentPosition(Card card)
    {
        (CardOnFieldBoard? board, _) = GameManager.Instance.clientPlayer.boardScript.FindBoardContainingCard(card.id);
        if(board == null)
        {
            return false;
        }
        board.RemoveCardFromContainer(card);
        return true;

    }

    public void CloneCard(OnFieldCardRightClickController controller)
    {
        Card card = controller.card;
    }

 

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance._gameStarted || !GameManager.Instance.gameInteractable)
        {
            return;
        }
        if(HelperFunctions.IsHoldingCTRL() && Input.GetKeyDown(KeyCode.D) && GameManager.Instance._gameStarted)
        {
           DrawCard();
        }
        if(HelperFunctions.IsHoldingCTRL() && Input.GetKeyDown(KeyCode.E) && GameManager.Instance._gameStarted)
        {
           GameManager.Instance._uiManager.EnableDrawCardsBox();
        }
        if(HelperFunctions.IsHoldingCTRL() && Input.GetKeyDown(KeyCode.S))
        {
            ShuffleDeck();
        }
    }
}
