using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Linq;
using System;
using Random = UnityEngine.Random;
public enum NetworkInstruction
{
    playerConnection, readyUp, userDisconnect, setLobbySize, chatboxMessage, unReady, updateDecks
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Should be defined outside of file
    const string _server = "127.0.0.1";
    const int _port = 54000;
    int _readyUpNeeded = 1;

    // Player Stuff
    int readyUpCount = 0;
    public string _uuid {get;  set;}
    int _PID;
    Dictionary<string, string> uuidToName = new Dictionary<string, string>();
    Dictionary<string, Dictionary<string,int>> uuidToDeckMap = new Dictionary<string, Dictionary<string,int>>();
    public Dictionary<string, Sprite> nameToSprite = new Dictionary<string, Sprite>();

    // CHILDREN
    public UIManager _uiManager;
    public TextureLoader textureLoader;
    public HandManager handManager;

    [SerializeReference]
    NetworkManager _networkManager;

    // Board Info
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
    bool requireCardCount = false;


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
    Dictionary<string, Player> uuidToPlayer = new Dictionary<string, Player>();

    public bool _gameStarted = false;


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
        // Transform prefab = Instantiate(_boardPrefab);
        // BoardComponents components = prefab.GetComponent<BoardComponents>();
        // clientPlayer = new Player("1","Player 1",1,components);
        // _gameStarted = true;
        AddUser(_uuid, "Gabe");
        _readyUpNeeded = 1;
        ReadyUp(_uuid, "{ \"Plains\": 1, \"Serra Angel\": 1, \"Lightning Bolt\": 1, \"Counterspell\": 1, \"Giant Growth\": 1, \"Llanowar Elves\": 1, \"Doom Blade\": 1, \"Wrath of God\": 1, \"Black Lotus\": 1, \"Birds of Paradise\": 1, \"Lightning Helix\": 1 }");
    }

    void Start()
    {
        FileLoader.LoadCSV(nameToCardInfo, "Assets/cards.csv");
        _uuid = System.Guid.NewGuid().ToString();
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
        foreach(Player player in uuidToPlayer.Values)
        {
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
        Dictionary<int, DeckDescriptor> newDecks = JsonConvert.DeserializeObject<Dictionary<int,DeckDescriptor>>(decks);
        p.UpdateDecks(newDecks);
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
