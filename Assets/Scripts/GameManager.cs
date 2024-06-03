using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Collections;

/*
TODO
Currently all the sending of networked are done from this file.
Any function that is called from network receiver is currently in this file.
Should separate functionality into sender/receiver function files.
Game Manager should soley be responsible for starting game stuff and sole point of info. 
*/

/*
TODO
Offload prefabs from gamemanager to file that actually uses it
*/

/*
TODO
Currently many of the networked functions perform the function locally, then send networked call. 
The paradigm should be that nothing is ever done locally. Network calls should be sent out instead
and then when the call is received it is done locally. 
Fix functions that don't follow this.
*/

/*
TODO
The mapping for cards is currently name to cardinfo and tokenname to cardinfo. This needs to be changed
to be uuid to cardinfo in both cases. 
*/
public enum NetworkInstruction
{
    playerConnection, readyUp, userDisconnect, setLobbySize, chatboxMessage, unReady, 
    updateDecks, showCardContainer, revealTopCard, millXCards, copyCard, deleteCard, tapUnap, 
    changePowerToughness, createRelatedCard, changeHealth, revealHand, GiveCardOnField
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Should be defined outside of file
    const string _server = "127.0.0.1";
    const int _port = 54000;
    public int _readyUpNeeded = 1;

    // Player Stuff
    int readyUpCount = 0;
    public string _uuid {get;  set;}
    int _PID;
    Dictionary<string, string> uuidToName = new Dictionary<string, string>();
    Dictionary<string, Dictionary<string,int>> uuidToDeckMap = new Dictionary<string, Dictionary<string,int>>();
    public Dictionary<string, Sprite> uuidToSprite = new Dictionary<string, Sprite>();

    public HashSet<string> twoSidedCards = new HashSet<string>();
    // CHILDREN
    public UIManager _uiManager;
    public WindowMoverController windowController;
    public TextureLoader textureLoader;
    public HandManager handManager;
    public PlayerDescriptionController playerDescriptionController;
    public PlayerController playerController;

    [SerializeReference]
    NetworkManager _networkManager;

    // Board Info
    public Transform onFieldCard;
    public Transform cardBack;
    public Transform pileTopper;
    public Material pilemat;
    public BoardMovement boardMovement;
    [SerializeField]
    Transform _boardPrefab;

    public Dictionary<string, CardInfo> nameToCardInfo = new Dictionary<string, CardInfo>();
    public Dictionary<string, TokenInfo> nameToToken = new Dictionary<string, TokenInfo>();

    public Dictionary<string, HashSet<string>> nameToRelatedCards = new Dictionary<string, HashSet<string>>();
    // TO DO
    // LOAD VALUES FROM FILES

    public int necessaryCardCount = 100;    

    public bool gameInteractable = true;
    bool requireCardCount = false;

    public int tapDegrees = 5;
    float _cameraHeight = 14.5f;
    float _cameraAngle = 80;
    float _cameraZOffset = -5.1f;

    // Prefabs
    public Transform cardInHandPrefab;
    public Transform customCardInHandPrefab;


    

    // Game Management
    [HideInInspector]
    public string _username;

    public Player clientPlayer;
    public Player enemyPlayer;
    public Dictionary<string, Player> uuidToPlayer = new Dictionary<string, Player>();
    public Dictionary<int, Card> idToCard = new Dictionary<int, Card>();

    public bool _gameStarted = false;

    public bool singlePlayer = true;


    private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }


    void FauxGame()
    {
        AddUser(_uuid, "Gabe");
        _readyUpNeeded = 1;
        //\"Plains\": 1, \"Serra Angel\": 1, \"Lightning Bolt\": 100, \"Counterspell\": 1, \"Giant Growth\": 1, \"Llanowar Elves\": 1, \"Doom Blade\": 1, \"Wrath of God\": 1, \"Black Lotus\": 1, \"Birds of Paradise\": 1, \"Lightning Helix\": 1, \"Darien, King of Kjeldor\": 1
        ReadyUp(_uuid, "{\"Lightning Bolt\": 100  }");
    }

    void Start()
    {
        Debug.developerConsoleVisible = true;
        FileLoader.LoadAllCardsCSV(nameToCardInfo, "Assets/cards.csv");
        FileLoader.LoadTokenCSV(nameToRelatedCards,nameToToken, "Assets/tokens.csv");
        _uuid = System.Guid.NewGuid().ToString();
        if(singlePlayer)
        {
            StartCoroutine(DelayedStart());
        }
        else
        {
            _uiManager.SwitchToConnect();
        }
    }

    public List<Player> GetPlayerList(bool includeClient)
    {
        List<Player> players = uuidToPlayer.Values.ToList();
        if(!includeClient)
        {
            players.Remove(clientPlayer);
        }
        return players;
    }

    IEnumerator DelayedStart()
    {
        yield return null; // Wait for one frame

        FauxGame();
    }

    public void AddUser( string uuid,string name)
    {
        if(_gameStarted)
        {
            return;
        }
        uuidToName[uuid] = name;
    }

    public void RemoveUser(string uuid)
    {
        if(_gameStarted)
        {
            return;
        }
        if(!uuidToName.ContainsKey(uuid))
            return;
        uuidToName.Remove(uuid); 
    }

    public void SendRevealedDeck(RevealPlayerManager controller, Deck deck)
    {
        List<string> uuids = controller.GetSelectedPlayers();
        if(uuids.Count == 0)
        {
            GameManager.Instance._uiManager.Disable(controller.transform);
            return;
        }
        LibraryDescriptor descriptor;
        List<int> allCards = HelperFunctions.GetCardInts(deck.GetCards());
        int? cardShowCount = null;
        if(controller.hasCardCount)
        {
            int output;
            if(Int32.TryParse(controller.cardCountInput.text, out output))
            {
                cardShowCount = output;
            }
            Debug.Log(cardShowCount);
        }
        descriptor = new LibraryDescriptor(allCards, deck.deckID,uuids, cardShowCount);
        GameManager.Instance._uiManager.Disable(controller.transform);
        _networkManager.SendMessage(NetworkInstruction.showCardContainer, JsonConvert.SerializeObject(descriptor));
    }

    public void RevealOpponentLibrary(string message, string uuid)
    {
        LibraryDescriptor descriptor = JsonConvert.DeserializeObject<LibraryDescriptor>(message);
        if(descriptor.uuids.Contains(_uuid))
        {
            _uiManager.RevealOpponentLibrary(descriptor, uuid);
        }
    }

    public List<Card> IntToCards(List<int> intToCard)
    {
        List<Card> returnCards = new List<Card>();
        foreach(int cardNumber in intToCard)
        {
            returnCards.Add(idToCard[cardNumber]);
        }
        return returnCards;
    }

    public void SendDestroyCard(int cardID)
    {
        _networkManager.SendMessage(NetworkInstruction.deleteCard,cardID.ToString());
    }

    public void ReceiveDestroyCard(string strID)
    {
        int cardID;
        if(!int.TryParse(strID, out cardID))
        {
            Debug.LogError($"Could not parse id - {strID}");
            return;
        }
        if(!idToCard.ContainsKey(cardID))
        {
            Debug.LogError($"Could not find card id - {cardID}");
            return;
        }
        idToCard[cardID].Destroy();
    }

    public void ReceieveCopyCard(string uuid, string strID)
    {
        int id;
        if(!int.TryParse(strID, out id))
        {
            Debug.LogError($"Cannot parse id - {strID}");
            return;
        }

        if(!idToCard.ContainsKey(id))
        {
            Debug.LogError($"Cannot find card - {id}");
            return;
        }
        if(!uuidToPlayer.ContainsKey(uuid))
        {
            Debug.LogError($"Cannot find player - {uuid}");
            return;
        }
        Card copiedCard = CardFactory.CopyCard(idToCard[id]);
        InsertCardNextToCard(uuid, copiedCard, id);
    }

    public void SendGiveCard(RevealPlayerManager controller, bool onField, Card card)
    {
        List<string> uuids = controller.GetSelectedPlayers();
        if(uuids.Count == 0)
        {
            return;
        }
        if(uuids.Count > 1)
        {
            Debug.LogError("Invalid Player Count");
            return;
        }
        card.currentLocation.RemoveCardFromContainer(card);
        SendUpdatedDecks();
        _networkManager.SendMessage(NetworkInstruction.GiveCardOnField, $"{uuids[0]}|{card.Id}");
    }

    public void ReceiveGiveCard(string instruction)
    {

        if (string.IsNullOrEmpty(instruction))
        {
            Debug.LogError("Received empty instruction");
            return;
        }

        string[] parts = instruction.Split('|');
        if (parts.Length != 2)
        {
            Debug.LogError("Invalid instruction format: " + instruction);
            return;
        }

        string targetUUID = parts[0];

        if (!uuidToPlayer.ContainsKey(targetUUID))
        {
            Debug.LogError($"Target player not found: {targetUUID}");
            return;
        }

        if(targetUUID != _uuid)
        {
            return;
        }
        if (!int.TryParse(parts[1], out int cardId))
        {
            Debug.LogError("Invalid card ID format: " + parts[1]);
            return;
        }

        if (!idToCard.ContainsKey(cardId))
        {
            Debug.LogError($"Card not found: {cardId}");
            return;
        }

        Card card = idToCard[cardId];

        Player targetPlayer = uuidToPlayer[targetUUID];
        
        targetPlayer.boardScript.GetCardOnFieldBoard(targetPlayer.boardScript.mainField).AddCardToContainer(card, null);
        SendUpdatedDecks();
    }

    

    void InsertCardNextToCard(string uuid, Card card, int originalCardID)
    {
        Player player = uuidToPlayer[uuid];
        (CardOnFieldBoard? boardContainingCard, int position) = player.boardScript.FindBoardContainingCard(originalCardID);
        if(boardContainingCard == null)
        {
            Debug.LogError($"Cannot find board containing card - {card.Id}");
            card.Destroy();
            return;
        }
        boardContainingCard.AddCardToContainer(card, position + 1);
    }





    void CreatePlayerBoards(int PID)
    {
        List<string> uuids = uuidToName.Keys.ToList<string>();
        uuids.Sort();
        int playerID = 0;
        foreach(string uuid in uuids)
        {
            Transform newBoard = GameObject.Instantiate(_boardPrefab);
            BoardComponents components = newBoard.GetComponent<BoardComponents>();
            Player player = new Player(uuid,uuidToName[uuid], playerID++, components, this._uuid == uuid);
            components.SetBoardValues(player.id);
            uuidToPlayer[uuid] = player;
            if(uuid == _uuid)
            {
                clientPlayer = player;
            }
        }
        Player[] players = BoardGenerator.ArrangeBoards(uuidToPlayer, _uuid, new Vector2 (0,20));
        boardMovement.SetValues(players,clientPlayer);
        BoardGenerator.PositionCamera(clientPlayer.boardScript.transform, Camera.main.transform, _cameraHeight, _cameraZOffset,_cameraAngle);
    }



    void SetupUserDecks()
    {
        clientPlayer.hand = transform.GetComponent<HandManager>();
        clientPlayer.hand.SetValues();
        List<string> playersIDs = uuidToPlayer.Keys.ToList<string>();
        playersIDs.Sort();
        foreach(string playerID in playersIDs)
        {
            Player player = uuidToPlayer[playerID];
            Dictionary<string, int> playersDeck = uuidToDeckMap[player.uuid];
            List<string> cardNames = new List<string>();
            foreach(string cardName in playersDeck.Keys)
            {
                for(int cardAdd = 0; cardAdd < playersDeck[cardName]; ++cardAdd)
                {
                    cardNames.Add(cardName);
                }
            }
            CardFactory.LoadDeck(cardNames, player.library);
        }
    }

    int GetSeed()
    {
        List<string> players = uuidToName.Keys.ToList<string>();
        players.Sort();
        int seed = 0;
        foreach(char c in players[0])
        {
            seed += c;
        }
        return seed;
    }
    void StartGame()
    {
        Random.InitState(GetSeed());
        _gameStarted = true;
        _uiManager.SwitchToStartGame();
        _uiManager.tokenCreatorController.LoadTokens(nameToToken);
        CreatePlayerBoards(_PID);
        SetupUserDecks();
        if(!singlePlayer)
        {
            playerDescriptionController.UpdateHealthBars();
        }

    }


    public void ConnectToServer(TMP_InputField textMeshProText)
    {
        if(textMeshProText.text == "")
            return;
        if(_networkManager.Connect(_server, textMeshProText.text, _port) != 0)
        {
            // _uiManager.spawnErrorMessage("Failed to connect to server\n", errorTimer);
            UnityEngine.Debug.LogError("Failed To Connect To Server");
            return;
        }
        _uiManager.SwitchToDeckLoad();
    }

    public void ReadyUp(string instructionUUID, string deck)
    {

        if(readyUpCount >= _readyUpNeeded)
            return;
    
        uuidToDeckMap[instructionUUID] = JsonConvert.DeserializeObject<Dictionary<string,int>>(deck);
        readyUpCount += 1;
        if(readyUpCount == _readyUpNeeded)
        {
            StartGame();
        }
    }

    public void SendCopyCard(OnFieldCardRightClickController controller)
    {
        if(controller.card == null)
        {
            return;
        }
        _networkManager.SendMessage(NetworkInstruction.copyCard,controller.card.Id.ToString());
    }

    public void UnreadyUp(string instructionUUID)
    {
        if(!uuidToPlayer.ContainsKey(instructionUUID))
        {
            return;
        }
        uuidToPlayer[instructionUUID].library = null;
        readyUpCount -= 1;
    }

    public void SendUpdatedDecks()
    {
        _networkManager.SendMessage(NetworkInstruction.updateDecks,JsonConvert.SerializeObject(clientPlayer.GetDeckDescriptions()) );
    }
    public void UpdateDecks(string instructionUUID, string decks)
    {
        if(!uuidToPlayer.ContainsKey(instructionUUID))
        {
            Debug.LogError($"Unable to find player {instructionUUID}");
            return;
        }
        Player p = uuidToPlayer[instructionUUID];
        if(p == clientPlayer)
        {
            return; // Skip client
        }
        Dictionary<int, DeckDescriptor> newDecks = JsonConvert.DeserializeObject<Dictionary<int,DeckDescriptor>>(decks);
        p.UpdateDecks(newDecks);
    }

    public void UpdateRevealDeck(string instructionUUID)
    {
        Player player = uuidToPlayer[instructionUUID];
        player.library._revealTopCard = !player.library._revealTopCard;
        player.library.UpdateContainer();
    }

    public void NetworkUpdateDeck()
    {
        _networkManager.SendMessage(NetworkInstruction.revealTopCard);
    }
    

    public bool VerifySubmittedDeck(TMP_InputField textMeshProText)
    {
        Dictionary<string, int> nameToCount = new Dictionary<string, int>();
        List<string> missedCards = new List<string>(); 
        int cardsLoaded = FileLoader.ParseCardList(textMeshProText.text,nameToCardInfo,nameToCount,missedCards );
        if(cardsLoaded == necessaryCardCount || !requireCardCount)
        {
            _networkManager.SendMessage(NetworkInstruction.readyUp,payload: JsonConvert.SerializeObject(nameToCount));
            return true;
        }
        else
        {
            string errorMessage;
            if(missedCards.Count != 0)
            {
                errorMessage = "Unable To Load Cards:\n";
                errorMessage += string.Join("\n", missedCards);
            }
            else
            {
                errorMessage = $"Only able to load {cardsLoaded}";
            }
            
            _uiManager.DisplayErrorMessage(errorMessage, 2);
        }
        return false;
    }

    public void UnReady()
    {
        _networkManager.SendMessage(NetworkInstruction.unReady);
    }

    public void SendTapUntap(Card card)
	{
        _networkManager.SendMessage(NetworkInstruction.tapUnap, card.Id.ToString());
	}
    public void ReceiveTapUnTap(string strID)
    {
        int cardID;
        if(!int.TryParse(strID, out cardID))
        {
            Debug.LogError($"Could not parse card ID - {strID}");
            return;
        }   
        if(!idToCard.ContainsKey(cardID))
        {
            Debug.LogError($"Could not find card id - {cardID}");
            return;
        }
        idToCard[cardID].TapUntap();
    }

    public void SendChangePowerToughness(bool power, Card card, int value)
    {
        string valueChanged = power ? "power" : "toughness";
        _networkManager.SendMessage(NetworkInstruction.changePowerToughness, $"{valueChanged}:{card.Id}:{value}");
    }

    public void ReceieveChangePowerToughness(string changeInstruction)
    {
        string[] changes = changeInstruction.Split(":");
        int cardID;
        int newValue;
        if(changes.Count() != 3)
        {
            Debug.LogError($"Improper formatting - {changeInstruction}");
            return;
        }
        if(!int.TryParse(changes[1], out cardID) || !idToCard.ContainsKey(cardID) )
        {
            Debug.LogError($"Invalid card ID - {changeInstruction} - [{changes[1]}]");
            return;
        }
        if(!int.TryParse(changes[2], out newValue))
        {
            Debug.LogError($"Invalid power/toughness - {changeInstruction} - [{changes[2]}]");
            return;
        }
        Card card = idToCard[cardID];
        if(changes[0] == "power")
        {
            card.power = newValue;
        }   
        else
        {
            card.toughness = newValue;
        }
        card.DisplayPowerToughness(true);
    }

    public void SendCreateRelatedCard(int id, string relatedCardName)
    {
        _networkManager.SendMessage(NetworkInstruction.createRelatedCard, $"{id}|{relatedCardName}");
    }

    public void ReceiveCreateReleatedCard(string instructionUUID, string instruction)
    {
        if (string.IsNullOrEmpty(instruction))
        {
            Debug.LogError("Received empty instruction");
            return;
        }

        string[] parts = instruction.Split('|');

        if (parts.Length != 2)
        {
            Debug.LogError("Invalid instruction format: " + instruction);
            return;
        }

        if (!int.TryParse(parts[0], out int cardId))
        {
            Debug.LogError("Invalid card ID format: " + parts[0]);
            return;
        }

        Card? relatedCard = CardFactory.CreateRelatedCard(parts[1]);
        if(relatedCard == null)
        {
            Debug.LogError($"Cannot find card {parts[1]}");
            return;
        }
        if(cardId < 0)
        {
            uuidToPlayer[instructionUUID].boardScript.mainField.GetComponent<CardOnFieldBoard>().AddCardToContainer(relatedCard, null);
            return;
        }
        InsertCardNextToCard(instructionUUID, relatedCard, cardId);
    }

    public void SendChangeHealth()
    {
        _networkManager.SendMessage(NetworkInstruction.changeHealth, clientPlayer.health.ToString());
    }

    public void ReceiveChangeHealth(string instructionUUID, string instruction)
    {
        if (!uuidToPlayer.ContainsKey(instructionUUID))
        {
            Debug.LogError($"Player not found in dict {instructionUUID}");
            return;
        }

        Player player = uuidToPlayer[instructionUUID];
        
        int newHealth;
        if (!int.TryParse(instruction, out newHealth))
        {
            Debug.LogError($"Invalid health value in instruction: {instruction}");
            return;
        }

        player.health = newHealth;
        playerDescriptionController.UpdateHealthBars();
    }
    
    public void SendRevealHand(RevealPlayerManager controller)
    {
        List<string> uuids = controller.GetSelectedPlayers();
        List<int> cardInts = HelperFunctions.GetCardInts(handManager.GetCards());

        _networkManager.SendMessage(NetworkInstruction.revealHand, JsonConvert.SerializeObject(new HandDescriptor(cardInts, uuids)));
    }

    public void ReceiveRevealHand(string instructionUUID, string instruction)
    {
        HandDescriptor descriptor = JsonConvert.DeserializeObject<HandDescriptor>(instruction);
        if(descriptor.uuids.Contains(_uuid))
        {
            _uiManager.RevealOpponentHand(descriptor, $"{uuidToPlayer[instructionUUID].name}'s hand");
        }
    }
}
