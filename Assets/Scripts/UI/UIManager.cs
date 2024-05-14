using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    ChangePowerToughnessController changePowerToughnessMenu;
    public Transform libraryPileRightClickMenu;
    public Transform genericPileRightClickMenu;

    [SerializeField]
    Transform drawCardBox;

    [SerializeField]
    public Transform cardContainerDisplayHolder;

    [SerializeField]
    Transform cardInHandMenu;
    [SerializeField]
    RevealPlayerManager selectPlayerMenu;

    [SerializeField]
    Transform playerButton;

    [SerializeField]
    DisplayContainerController revealHolder;
    [SerializeField]
    OnFieldCardRightClickController cardOnFieldRightClickMenu;
    [SerializeField]
    CardInHandController cardInHandRightClickMenu;
    public Transform defaultRightClickMenu;

    [SerializeField]
    RelatedCardsController relatedCardsController;

    [SerializeField]
    Image currentlyHoveredCard;
    bool skipDisablingMenus = false;
    public Transform unusedCardHolder;
    public DisplayContainerController cardContainerController;

    public Deck currentSelectedDeck;
    public TokenCreatorController tokenCreatorController;

    [SerializeField]
    Image hoveredCardImage;
    HoveredCardController hoveredCardController;

    [SerializeField]
    Transform playerBar;
    [SerializeField]
    Transform enemyBar;


    bool ready = false;

    void Start()
    {
        hoveredCardController = new HoveredCardController(GameManager.Instance,hoveredCardImage);
    }

    /* TODO
    MAKE THIS ALL ARRAY STUFF
    Put all menus in one array, clean up code!
    */
    void DisableRightClickMenus()
    {
        libraryPileRightClickMenu.gameObject.SetActive(false);
        genericPileRightClickMenu.gameObject.SetActive(false);
        cardOnFieldRightClickMenu.gameObject.SetActive(false);
        defaultRightClickMenu.gameObject.SetActive(false);
        cardInHandRightClickMenu.gameObject.SetActive(false);
    }
    bool InSpecificMenu(Transform menu)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(menu.GetComponent<RectTransform>(), Input.mousePosition) && menu.gameObject.activeInHierarchy;
    }
    bool MouseInMenu()
    {
        bool inLibraryPileMenu = InSpecificMenu(libraryPileRightClickMenu);
        bool inGenericPileMenu = InSpecificMenu(genericPileRightClickMenu);
        bool inCardOnFieldRightClickMenu = InSpecificMenu(cardOnFieldRightClickMenu.transform);
        bool inDefaultRightClickMenu = InSpecificMenu(defaultRightClickMenu.transform);
        bool inInHandRightClickMenu = InSpecificMenu(cardInHandRightClickMenu.transform);

        return inLibraryPileMenu || inGenericPileMenu || inCardOnFieldRightClickMenu || inDefaultRightClickMenu || inInHandRightClickMenu;
    }
    void Update()
    {
        if(skipDisablingMenus)
        {
            skipDisablingMenus = false;
            return;
        }
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) // Technically should cache rt perhaps and cam main
        {
            if(!MouseInMenu())
            {
                DisableRightClickMenus();
            }
        }
    }
    void Awake()
    {
        connectMenu.SetActive(false);
        deckLoadMenu.SetActive(false);
        errorMessage.SetActive(false);
    }

    public void RemoveSelectedDeck()
    {
        currentSelectedDeck = null;
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
        playerBar.gameObject.SetActive(true);
        enemyBar.gameObject.SetActive(true);
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

    public void EnableLibraryRightClickMenu(Deck deck,Transform menuTransform )
    {
        skipDisablingMenus= true;
        DisableRightClickMenus();
        currentSelectedDeck = deck;
        menuTransform.gameObject.SetActive(true);
        RectTransform menu = menuTransform.GetComponent<RectTransform>();
        menuTransform.SetAsLastSibling();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    }

    public void EnableDefaultMenu()
    {
        skipDisablingMenus= true;
        DisableRightClickMenus();
        defaultRightClickMenu.gameObject.SetActive(true);
        RectTransform menu = defaultRightClickMenu.GetComponent<RectTransform>();
        defaultRightClickMenu.SetAsLastSibling();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    }

    public void EnableChangePowerToughnessMenu()
    {
        changePowerToughnessMenu.gameObject.SetActive(true);
        OnFieldCardRightClickController controller = cardOnFieldRightClickMenu.GetComponent<OnFieldCardRightClickController>();
        changePowerToughnessMenu.InitMenu(controller.card);
    }


    public void DisableRightClickMenu()
    {
        libraryPileRightClickMenu.gameObject.SetActive(false);
    }

    public Transform ShowCardContainer()
    {
        cardContainerDisplayHolder.gameObject.SetActive(true);
        cardContainerDisplayHolder.SetAsLastSibling();
        cardContainerDisplayHolder.position = new Vector3(Screen.width/2, Screen.height/2, 0);
        return cardContainerDisplayHolder;
    }

    public void HideLibrary()
    {
        cardContainerDisplayHolder.gameObject.SetActive(false);
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
            revealHolder.LoadPile(deck,null,false,deck.GetName());
        }
        else
        {
            int cardMax = Math.Min(deck.cards.Count, (int)descriptor.cardShowCount);
            if(cardMax == 0)
            {
                return;
            }
            List<Card> cardsToRender = deck.cards.GetRange(deck.cards.Count - cardMax, cardMax);
            revealHolder.LoadPile(deck,cardsToRender,false, deck.GetName());
        }
    }

    public void RevealOpponentHand(HandDescriptor descriptor, string name)
    {
        List<Card> handCards = GameManager.Instance.IntToCards(descriptor.cards);
        revealHolder.LoadPile(null, handCards, false, name );
    }

    public void EnableRelatedCardsMenu(OnFieldCardRightClickController menu)
    {
        relatedCardsController.InitMenu(menu);
    }

    public void SpawnCardOnFieldMenu(Card card)
    {
        DisableRightClickMenus();
        skipDisablingMenus = true;
        cardOnFieldRightClickMenu.gameObject.SetActive(true);
        cardOnFieldRightClickMenu.GetComponent<OnFieldCardRightClickController>().card = card;
        RectTransform menu = cardOnFieldRightClickMenu.GetComponent<RectTransform>();
        cardOnFieldRightClickMenu.relatedCardButton.gameObject.SetActive(GameManager.Instance.nameToRelatedCards.ContainsKey(card.name) || card.twoSidedNames != null);
        cardOnFieldRightClickMenu.transform.SetAsLastSibling();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    }

    public void SetHoveredCard(Card card)
    {
        hoveredCardController.ChangeHoveredCard(card);
    }
    public void SpawnCardInHandMenu(Card card)
    {
        cardInHandRightClickMenu.InitMenu(card);
        skipDisablingMenus = true;
        RectTransform menu = cardInHandRightClickMenu.GetComponent<RectTransform>();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);


    }
}
