using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardGenerator
{
    public static Player[] ArrangeBoards(Dictionary<string, Player> idToPlayer, string targetID, Vector2 padding) // X is  distance from player to main board. Y is distance inbetween boards
    {
        int opponentCount = idToPlayer.Count - 1;
        Transform mainBoard = idToPlayer[targetID].boardScript.transform;
        mainBoard.transform.position = Vector3.zero;

        Vector3 extents = mainBoard.GetComponent<Renderer>().bounds.extents;
        string[] uuids = idToPlayer.Keys.ToArray<string>();
        Array.Sort(uuids);
        Player[] players = new Player[opponentCount];
        int counter = 0;
        foreach(string uuid in uuids)
        {
            if(uuid == targetID)
            {
                continue;
            }
            players[counter++] = idToPlayer[uuid];
        }
        for(int opponentID = 0; opponentID < opponentCount; ++opponentID)
        {
            Transform boardScript = players[opponentID].boardScript.transform;
            boardScript.position = new Vector3(mainBoard.position.x + opponentID * (extents.x*2 + padding.y), mainBoard.position.y, mainBoard.position.z + padding.x + extents.z*2);
            boardScript.Rotate(Vector3.up, 180f);
            boardScript.GetComponent<BoardComponents>().isEnemyBoard = true; 
        }
        return players;
    }

    public static void PositionCamera(Transform board, Transform camera, float height,float zOffset,  float angle)
    {
        camera.localPosition =  new Vector3(board.position.x, board.position.y + height, board.position.z - (board.GetComponent<Renderer>().bounds.extents.z + zOffset));
        camera.rotation = Quaternion.Euler(new Vector3(angle,0,0));
        camera.SetParent(board);

    }
}
