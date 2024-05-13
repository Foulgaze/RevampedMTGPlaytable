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
            case (int) NetworkInstruction.playerConnection: // This is when a new player connects to the server
            {
                // gameController.addToPlayerList(uuid == msgUUID, instruction,msgUUID);
                if(GameManager.Instance._uuid == msgUUID)
                {
                    GameManager.Instance._username = instruction;
                }
                // chatbox.AddMessage($"{instruction} Has Connected!");
                GameManager.Instance.AddUser(msgUUID,instruction);
                break;                    
            } 
            case (int) NetworkInstruction.readyUp: // This is when a user readies up
            {
                GameManager.Instance.ReadyUp(msgUUID,instruction);
                // chatbox.AddMessage($"{gameManager.getUsername(uuid)} is ready.");
                // chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
                break;
            }
            case (int) NetworkInstruction.updateDecks: // This is when a user readies up
            {
                GameManager.Instance.UpdateDecks(msgUUID,instruction);
                // chatbox.AddMessage($"{gameManager.getUsername(uuid)} is ready.");
                // chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
                break;
            }
            case 2: // This is when a user disconnects to the server
            {
                // gameManager.RemoveUser(uuid);
                // chatbox.AddMessage($"{instruction} Has Disconnected!");
                break;
            }
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
            case (int) NetworkInstruction.unReady: // When a user unreadies up
            {
                GameManager.Instance.UnreadyUp(msgUUID);
                // chatbox.AddMessage($"{gameManager.getUsername(uuid)} has unreadied.");
                // chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
                break;
            }
            case (int) NetworkInstruction.showCardContainer: // When a user unreadies up
            {
                if(msgUUID != GameManager.Instance._uuid)
                {
                    GameManager.Instance.RevealOpponentLibrary(instruction, msgUUID);
                }
                // chatbox.AddMessage($"{gameManager.getUsername(uuid)} has unreadied.");
                // chatbox.AddMessage($"{gameManager.readyCount}/{gameManager.lobbySize} users ready.");
                break;
            }
            case (int) NetworkInstruction.revealTopCard:
            {
                GameManager.Instance.UpdateRevealDeck(msgUUID);
                break;
            }
            case (int) NetworkInstruction.copyCard:
            {
                GameManager.Instance.ReceieveCopyCard(msgUUID, instruction);
                break;
            }
            case (int) NetworkInstruction.deleteCard:
            {
                GameManager.Instance.ReceiveDestroyCard(instruction);
                break;
            }         
            case (int) NetworkInstruction.tapUnap:
            {
                GameManager.Instance.ReceiveTapUnTap(instruction);
                break;
            }   
            case (int) NetworkInstruction.changePowerToughness:
            {
                GameManager.Instance.ReceieveChangePowerToughness(instruction);
                break;
            }   
            case (int) NetworkInstruction.createRelatedCard:
            {
                GameManager.Instance.ReceiveCreateReleatedCard(msgUUID, instruction);
                break;
            }
            case (int) NetworkInstruction.changeHealth:
            {
                GameManager.Instance.ReceiveChangeHealth(msgUUID, instruction);
                break;
            }
        }
    }
}
