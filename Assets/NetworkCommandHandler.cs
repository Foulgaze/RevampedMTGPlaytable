using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkCommandHandler
{
    static public string ParseSocketData(string messageBuffer)
    {
        while(true)
        {
            string messageLength = "";
            int messageLengthRemaining = 0;
            int i = 0;
            while(messageLength.Length != 4)
            {
                if(i >= messageBuffer.Length)
                {
                    return messageBuffer;
                }

                messageLength = messageLength + messageBuffer[i++];
            }
            
            messageLengthRemaining = Int32.Parse(messageLength);
            
            
            string currentCommand = "";
            if(messageLengthRemaining > messageBuffer.Length - 4)
            {
                return messageBuffer;
            }
            
            currentCommand = messageBuffer.Substring(i,messageLengthRemaining);
            ParseCommand(currentCommand);
            messageBuffer = messageBuffer.Substring(i+messageLengthRemaining);
        }
    }
    static void ParseCommand(string completeMessage)
    {
        // Parsing recieved message into UUID, opCode, and Content

        int breakPos = completeMessage.IndexOf("|");
        string msgUUID = completeMessage.Substring(0,breakPos);
        int opCode = Int32.Parse(completeMessage.Substring(breakPos+1,2));
        string instruction = completeMessage.Substring(breakPos+4); 

        // Doing action based on opCode
        switch(opCode)
        {
            default:
            {
                Debug.LogError($"The program has recieved an unknown opCode of [{opCode}]");
                break;
            }
            case 0: // This is when a new player connects to the server
            {
                // gameController.addToPlayerList(uuid == msgUUID, instruction,msgUUID);
                if(GameManager.Instance._uuid == msgUUID)
                {
                    GameManager.Instance._username = instruction;
                }
                // chatbox.AddMessage($"{instruction} Has Connected!");
                GameManager.Instance.AddUser(instruction,msgUUID);
                break;                    
            } 
            // case 1: // This is when a user readies up
            // {
            //     gameManager.readyUp(uuid,instruction);
            //     chatbox.AddMessage($"{gameManager.getUsername(uuid)} is ready.");
            //     chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
            //     break;
            // }
            // case 2: // This is when a user disconnects to the server
            // {
            //     gameManager.RemoveUser(uuid);
            //     chatbox.AddMessage($"{instruction} Has Disconnected!");
            //     break;
            // }
            // case 3: // This is when a user sets lobbysize
            // {
            //     int previousLobbySize = gameManager.lobbySize;
            //     gameManager.setLobbySize(instruction);
            //     chatbox.AddMessage($"Lobby size changed from {previousLobbySize} to {gameManager.lobbySize}");
            //     break;
            // }
            // case 4: // This is where user sends message
            // {
            //     chatbox.AddMessage(instruction);
            //     break;
            // }
            // case 5: // When a user unreadies up
            // {
            //     gameManager.unReady(uuid);
            //     chatbox.AddMessage($"{gameManager.getUsername(uuid)} has unreadied.");
            //     chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
            //     break;
            // }
            
        }
    }
}
