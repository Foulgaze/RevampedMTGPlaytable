using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Data.Analysis;
using System.IO;
using System;
using Deedle;
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }

    // Should be defined outside of file
    const string _server = "127.0.0.1";
    const int _port = 54000;
    public string _uuid {get;  set;}

    // CHILDREN
    [SerializeReference]
    UIManager _uiManager;

    // Board Info
    [SerializeField]
    Transform cardHolder;
    [SerializeField]
    BoardMovement boardMovement;

    Dictionary<int, Transform> _idToHolder;
    Dictionary<int, Transform> _idToBoard;
    


    // TO DO
    // LOAD VALUES FROM FILES
    Vector3 _intercardPadding = new Vector3(2,0,3);
    Vector4 _boardPadding = new Vector4(4,1,2,1);

    int _rowCount = 8;
    int _colCount = 3;
    int _playerCount = 4;

    float _cameraHeight = 15;
    float _cameraAngle = 25;
    float _cameraZOffset = 10;



    // Networking
    [SerializeReference]
    NetworkManager _networkManager;

    // Game Management
    [HideInInspector]
    public string _username;
    Dictionary<string, string> _uuidToName;

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

    void Start()
    {
        _uuid = System.Guid.NewGuid().ToString();
        _uuidToName = new Dictionary<string, string>();
        _idToHolder = new Dictionary<int, Transform>();
        _idToBoard = new Dictionary<int, Transform>();
        int PID  =0;
        for(int playerID = 0; playerID < _playerCount; ++playerID)
        {
            _idToBoard[playerID] = BoardGenerator.GenerateBoard(playerID,_idToHolder,_rowCount,_colCount,cardHolder, _intercardPadding,_boardPadding, playerID == PID );        
        }
        Transform[] boards = BoardGenerator.ArrangeBoards(_idToBoard, PID, Vector2.zero);
        boardMovement.SetValues(boards, _idToBoard[PID]);

        BoardGenerator.PositionCamera(_idToBoard[PID], Camera.main.transform, _cameraHeight, _cameraZOffset,_cameraAngle);


    }
    public void AddUser(string name, string uuid)
    {
        _uuidToName[uuid] = name;
    }

    public void RemoveUser(string uuid)
    {
        _uuidToName.Remove(uuid); 
    }

    public void ConnectToServer(TMP_InputField textMeshProText)
    {
        if(textMeshProText.text == "")
        {
            return;
        }
        if(_networkManager.Connect(_server, textMeshProText.text, _port) != 0)
        {
            // _uiManager.spawnErrorMessage("Failed to connect to server\n", errorTimer);
            UnityEngine.Debug.LogError("Failed To Connect To Server");
            return;
        }
        _uiManager.SwitchToDeckLoad();
    }


}
