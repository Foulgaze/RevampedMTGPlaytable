using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject boxHolder;
    [SerializeField]
    GameObject connectMenu;
    [SerializeField]
    GameObject deckLoadMenu;

    [SerializeField]
    GameObject errorMessage;
    [SerializeField]
    TextMeshProUGUI errorText;
    [SerializeField]
    TextMeshProUGUI readyUpText;
    [SerializeField]
    TMP_InputField deckLoad;
    public Transform libraryPileRightClickMenu;
    public Transform genericPileRightClickMenu;

    [SerializeField]
    Transform drawCardBox;

    [SerializeField]
    public Transform libraryHolder;

    [SerializeField]
    RevealPlayerManager selectPlayerMenu;

    [SerializeField]
    Transform playerButton;

    [SerializeField]
    DisplayDeckController revealHolder;

    public Transform unusedCardHolder;
    public DisplayDeckController libraryBoxController;

    public Deck currentSelectedDeck;


    bool ready = false;


    
    void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && libraryPileRightClickMenu.gameObject.activeSelf) // Technically should cache rt perhaps and cam main
        {
            if(!RectTransformUtility.RectangleContainsScreenPoint(libraryPileRightClickMenu.GetComponent<RectTransform>(), Input.mousePosition))
            {
                libraryPileRightClickMenu.gameObject.SetActive(false);
                genericPileRightClickMenu.gameObject.SetActive(false);
            }
        }
    }
    void Awake()
    {
        connectMenu.SetActive(false);
        deckLoadMenu.SetActive(false);
        errorMessage.SetActive(false);
    }

    public void SwitchToConnect()
    {
        connectMenu.SetActive(true);
    }

    public void SwitchToDeckLoad()
    {
        connectMenu.SetActive(false);
        deckLoadMenu.SetActive(true);
    }

    public void SwitchToStartGame()
    {
        deckLoadMenu.SetActive(false);
    }

    public void DisplayErrorMessage(string newMessage, int timer)
    {
        errorText.text = newMessage;
        StartCoroutine(DisplayErrorMessageForSeconds(timer));
    }
    
    public void ChangeReadyStatus()
    {
        ready = !ready;

        if(ready)
        {
            readyUpText.text = "Unready";
            deckLoad.interactable = false;
            return;
        }
        readyUpText.text = "Ready";
        deckLoad.interactable = true;

    }


    public void EnableDrawCardsBox()
    {
        GameManager.Instance.gameInteractable = false;
        drawCardBox.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        drawCardBox.gameObject.SetActive(true);
        drawCardBox.SetAsLastSibling();
        DisableRightClickMenu();
    }

    public void DisableDrawCardsBox()
    {
        GameManager.Instance.gameInteractable = true;
        drawCardBox.gameObject.SetActive(false);
    }

    public void EnableRightClickMenu(Deck deck,Transform menuTransform )
    {
        currentSelectedDeck = deck;
        menuTransform.gameObject.SetActive(true);
        RectTransform menu = menuTransform.GetComponent<RectTransform>();
        menuTransform.SetAsLastSibling();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    }

    // public void EnableRightClickMenu(Deck deck)
    // {
    //     currentSelectedDeck = deck;
    //     rightClickMenu.gameObject.SetActive(true);
    //     RectTransform menu = rightClickMenu.GetComponent<RectTransform>();
    //     rightClickMenu.SetAsLastSibling();
    //     menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    // }

    public void DisableRightClickMenu()
    {
        libraryPileRightClickMenu.gameObject.SetActive(false);
    }

    public Transform ShowLibrary()
    {
        libraryHolder.gameObject.SetActive(true);
        libraryHolder.SetAsLastSibling();
        return libraryHolder;
    }

    public void HideLibrary()
    {
        libraryHolder.gameObject.SetActive(false);
    }
    

    public void Disable(Transform thingToDisable)
    {
        thingToDisable.gameObject.SetActive(false);
    }

    public void Enable(Transform thingToEnable) // easy to call function w/ buttons
    {
        thingToEnable.gameObject.SetActive(true);
        thingToEnable.SetAsLastSibling();
    }

    public void RevealXCardsFromLibrary(Transform dialogBox)
    {
        LibraryInfoStorage storage = dialogBox.GetComponent<LibraryInfoStorage>();
        storage.ClearInput();
        storage.currentDeck = currentSelectedDeck;
        dialogBox.SetAsLastSibling();
        dialogBox.gameObject.SetActive(true);
    }




    private IEnumerator DisplayErrorMessageForSeconds(float enableTime)
    {
        
        errorMessage.SetActive(true);
        errorMessage.transform.SetAsLastSibling();
        yield return new WaitForSeconds(enableTime);

        errorMessage.SetActive(false);
        
    }

    public void HandleReadyButton(TMP_InputField textMeshProText)
    {
        if(!ready)
        {
            if(GameManager.Instance.VerifySubmittedDeck(textMeshProText))
            {
                ChangeReadyStatus();
            }
            return;
        }
        GameManager.Instance.UnReady();
        ChangeReadyStatus();
    }

    public void SelectPlayerMenu(bool showCardCount)
    {
        GameManager manager = GameManager.Instance;
        List<Player> players = new List<Player>();
        foreach(Player player in manager.uuidToPlayer.Values)
        {
            if(player != manager.clientPlayer)
            {
                players.Add(player);
            }
        }
        selectPlayerMenu.EnableBox(players, currentSelectedDeck, showCardCount);
    }

    public void RevealOpponentLibrary(LibraryDescriptor descriptor, string uuid)
    {
        Deck deck = GameManager.Instance.uuidToPlayer[uuid].GetDeck(descriptor.deckID);
        deck.cards = GameManager.Instance.IntToCards(descriptor.cards);
        if(descriptor.cardShowCount == null)
        {
            revealHolder.LoadPile(deck,null,false);
        }
        else
        {
            int cardMax = Math.Min(deck.cards.Count, (int)descriptor.cardShowCount);
            if(cardMax == 0)
            {
                return;
            }
            List<Card> cardsToRender = deck.cards.GetRange(deck.cards.Count - cardMax, cardMax);
            revealHolder.LoadPile(deck,cardsToRender,false);
        }
    }
}
