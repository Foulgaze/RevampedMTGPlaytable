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
public enum NetworkInstruction
{
    playerConnection, readyUp, userDisconnect, setLobbySize, chatboxMessage, unReady, updateDecks, showLibrary, revealTopCard, millXCards, copyCard, deleteCard
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Should be defined outside of file
    const string _server = "127.0.0.1";
    const int _port = 54000;
    int _readyUpNeeded = 2;

    // Player Stuff
    int readyUpCount = 0;
    public string _uuid {get;  set;}
    int _PID;
    Dictionary<string, string> uuidToName = new Dictionary<string, string>();
    Dictionary<string, Dictionary<string,int>> uuidToDeckMap = new Dictionary<string, Dictionary<string,int>>();
    public Dictionary<string, Sprite> nameToSprite = new Dictionary<string, Sprite>();

    // CHILDREN
    public UIManager _uiManager;
    public WindowMoverController windowController;
    public TextureLoader textureLoader;
    public HandManager handManager;

    [SerializeReference]
    NetworkManager _networkManager;

    // Board Info
    public Transform onFieldCard;
    public Transform cardBack;
    public Transform pileTopper;
    public Material pilemat;
    [SerializeField]
    BoardMovement _boardMovement;
    [SerializeField]
    Transform _boardPrefab;

    public Dictionary<string, CardInfo> nameToCardInfo = new Dictionary<string, CardInfo>();

    // TO DO
    // LOAD VALUES FROM FILES

    public int necessaryCardCount = 100;    

    public bool gameInteractable = true;
    bool requireCardCount = false;

    public int tapDegrees = 5;
    float _cameraHeight = 15.5f;
    float _cameraAngle = 70;
    float _cameraZOffset = -2.5f;

    // Prefabs
    public Transform cardInHandPrefab;
    public Transform customCardInHandPrefab;


    

    // Game Management
    [HideInInspector]
    public string _username;

    public Player clientPlayer;
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
        ReadyUp(_uuid, "{ \"Plains\": 1, \"Serra Angel\": 1, \"Lightning Bolt\": 1, \"Counterspell\": 1, \"Giant Growth\": 1, \"Llanowar Elves\": 1, \"Doom Blade\": 1, \"Wrath of God\": 1, \"Black Lotus\": 1, \"Birds of Paradise\": 1, \"Lightning Helix\": 1 }");
    }

    void Start()
    {
        Debug.developerConsoleVisible = true;
        FileLoader.LoadCSV(nameToCardInfo, "Assets/cards.csv");
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

    public void SendRevealedDeck(RevealPlayerManager controller)
    {
        List<string> uuids = controller.GetSelectedPlayers();
        if(uuids.Count == 0)
        {
            return;
        }
        LibraryDescriptor descriptor;
        List<int> allCards = controller.selectedDeck.GetCards();
        int? cardShowCount = null;
        if(controller.input.gameObject.activeInHierarchy)
        {
            int output;
            if(Int32.TryParse(controller.input.text, out output))
            {
                cardShowCount = output;
            }
        }
        descriptor = new LibraryDescriptor(allCards, controller.selectedDeck.deckID,uuids, cardShowCount);
        _networkManager.SendMessage(NetworkInstruction.showLibrary, JsonConvert.SerializeObject(descriptor));
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

    public void CopyCard(string uuid, string strID)
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
        Card copiedCard = CardManager.CopyCard(idToCard[id]);
        Player player = uuidToPlayer[uuid];
        (CardOnFieldBoard? boardContainingCard, int position) = player.boardScript.FindBoardContainingCard(id);
        if(boardContainingCard == null)
        {
            Debug.LogError($"Cannot find board containing card - {copiedCard.id}");
            return;
        }
        boardContainingCard.AddCardToContainer(copiedCard, position + 1);
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
            Player player = new Player(uuid,uuidToName[uuid], playerID++, components);
            components.SetBoardValues(player.id);
            uuidToPlayer[uuid] = player;
            if(uuid == _uuid)
            {
                clientPlayer = player;
            }
        }
        Transform[] boards = BoardGenerator.ArrangeBoards(uuidToPlayer, _uuid, new Vector2 (0,20));
        _boardMovement.SetValues(boards,clientPlayer.boardScript.transform);
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
            List<string> totalDeck = new List<string>();
            foreach(string cardName in playersDeck.Keys)
            {
                for(int cardAdd = 0; cardAdd < playersDeck[cardName]; ++cardAdd)
                {
                    totalDeck.Add(cardName);
                }
            }
            CardManager.LoadDeck(totalDeck, player.library);
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
        CreatePlayerBoards(_PID);
        SetupUserDecks();

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

    public void ReadyUp(string uuid, string deck)
    {

        if(readyUpCount >= _readyUpNeeded)
            return;
    
        uuidToDeckMap[uuid] = JsonConvert.DeserializeObject<Dictionary<string,int>>(deck);
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
        _networkManager.SendMessage(NetworkInstruction.copyCard,controller.card.id.ToString() );
    }

    public void UnreadyUp(string uuid)
    {
        if(!uuidToPlayer.ContainsKey(uuid))
        {
            return;
        }
        uuidToPlayer[uuid].library = null;
        readyUpCount -= 1;
    }

    public void SendUpdatedDecks()
    {
        _networkManager.SendMessage(NetworkInstruction.updateDecks,JsonConvert.SerializeObject(clientPlayer.GetDeckDescriptions()) );
    }
    public void UpdateDecks(string uuid, string decks)
    {
        if(!uuidToPlayer.ContainsKey(uuid))
        {
            Debug.LogError($"Unable to find player {uuid}");
            return;
        }
        Player p = uuidToPlayer[uuid];
        if(p == clientPlayer)
        {
            return; // Skip client
        }
        Dictionary<int, DeckDescriptor> newDecks = JsonConvert.DeserializeObject<Dictionary<int,DeckDescriptor>>(decks);
        p.UpdateDecks(newDecks);
    }

    public void UpdateRevealDeck(string uuid)
    {
        Player player = uuidToPlayer[uuid];
        player.library._revealTopCard = !player.library._revealTopCard;
        player.library.UpdatePhysicalDeck();
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

}
