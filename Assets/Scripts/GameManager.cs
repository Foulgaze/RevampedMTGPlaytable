using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Linq;
using System;
using Random = UnityEngine.Random;
public enum NetworkInstruction
{
    playerConnection, readyUp, userDisconnect, setLobbySize, chatboxMessage, unReady
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Should be defined outside of file
    const string _server = "127.0.0.1";
    const int _port = 54000;
    int _readyUpNeeded = 1;

    // Player Stuff
    int _readyUpCount = 0;
    public string _uuid {get;  set;}
    int _PID;
    Dictionary<string, string> uuidToName = new Dictionary<string, string>();

    // CHILDREN
    [SerializeField]
    UIManager _uiManager;
    [SerializeField]
    TextureLoader _textureLoader;

    // Board Info
    public Sprite cardBack;
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




    // Networking
    [SerializeReference]
    NetworkManager _networkManager;

    // Game Management
    [HideInInspector]
    public string _username;

    public Player clientPlayer;
    Dictionary<string, Player> _uuidToPlayer = new Dictionary<string, Player>();

    HashSet<string> _spritesToBeLoaded = new HashSet<string>();

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
        Transform prefab = Instantiate(_boardPrefab);
        BoardComponents components = prefab.GetComponent<BoardComponents>();
        clientPlayer = new Player("1","Player 1",1,components);
        _gameStarted = true;
    }

    void Start()
    {
        FileLoader.LoadCSV(nameToCardInfo, "Assets/cards.csv");

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
        List<string> uuids = uuidToName.Values.ToList<string>();
        uuids.Sort();
        int playerID = 0;
        foreach(string uuid in uuids)
        {
            Transform newBoard = GameObject.Instantiate(_boardPrefab);
            BoardComponents components = newBoard.GetComponent<BoardComponents>();
            Player player = new Player(uuid,uuidToName[uuid], playerID++, components);
            _uuidToPlayer[uuid] = player;
        }
        Transform[] boards = BoardGenerator.ArrangeBoards(_uuidToPlayer, _uuid, new Vector2 (0,20));
        _boardMovement.SetValues(boards,clientPlayer.boardScript.transform);
        BoardGenerator.PositionCamera(clientPlayer.boardScript.transform, Camera.main.transform, _cameraHeight, _cameraZOffset,_cameraAngle);
    }


    void SetupUserDecks()
    {
        foreach(Player player in _uuidToPlayer.Values)
        {
            
            
            if(player == clientPlayer)
            {
                player.hand = transform.GetComponent<HandManager>();
                player.hand.SetValues();
            }
        }
    }

    int GetSeed()
    {
        List<string> players = _uuidToPlayer.Keys.ToList<string>();
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
        // LoadClientCards();
        SetupUserDecks();

    }
    
        
    void LoadClientCards()
    {
        foreach(Player player in _uuidToPlayer.Values)
        {
            _spritesToBeLoaded.UnionWith(player.library._cards);
        }
        foreach(string spriteName in _spritesToBeLoaded)
        {
            if(!nameToCardInfo.ContainsKey(spriteName))
            {
                Debug.LogError($"Could not load card ${spriteName}");
                continue;
            }
            UnityEngine.Debug.Log($"{nameToCardInfo[spriteName].name} - {nameToCardInfo[spriteName].setCode} - {nameToCardInfo[spriteName].cardNumber}");
            StartCoroutine(_textureLoader.GetSprite(nameToCardInfo[spriteName].setCode,nameToCardInfo[spriteName].cardNumber, spriteName));
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

    public void ReadyUp(string uuid, string deck)
    {
        Player p = _uuidToPlayer[uuid];

        if(_readyUpCount >= _readyUpNeeded || p == null)
            return;
    
        // p.library = new Deck(JsonConvert.DeserializeObject<Dictionary<string,int>>(deck));
        _readyUpCount += 1;
        if(_readyUpCount == _readyUpNeeded)
        {
            StartGame();
        }
    }

    public void UnreadyUp(string uuid)
    {
        if(!_uuidToPlayer.ContainsKey(uuid))
            return;
        _uuidToPlayer[uuid].library = null;
        _readyUpCount -= 1;

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
