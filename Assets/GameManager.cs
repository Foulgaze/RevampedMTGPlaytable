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
    UIManager uiManager;


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


        string filePath = "Assets/cards1.csv";

        var msftRaw = Frame.ReadCsv(filePath);
        Debug.Log("DONE");
        // string data = File.ReadAllText(filePath);
        // DataFrame df = DataFrame.LoadCsvFromString(data);


        // Get the elapsed time in milliseconds

        // Display the elapsed time


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
            // uiManager.spawnErrorMessage("Failed to connect to server\n", errorTimer);
            UnityEngine.Debug.LogError("Failed To Connect To Server");
            return;
        }
        uiManager.SwitchToDeckLoad();

    }


}
