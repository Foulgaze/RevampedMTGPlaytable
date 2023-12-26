using System.Collections;
using System.Collections.Generic;
using Apache.Arrow.Types;
using UnityEngine;

public class BoardGenerator
{
   

    // Padding = Top, Right, Bottom, Left
    public static Transform GenerateBoard(int opponentID,Dictionary<int, Transform> idToHolder,int row, int col, Transform cardHolder, Vector3 intercardPadding, Vector4 padding, bool main)
    {
        
        if(main)
        {
            padding = new Vector4(Mathf.Min(padding.x,padding.z), Mathf.Min(padding.y,padding.w), Mathf.Min(padding.x, padding.z), Mathf.Min(padding.y,padding.w));
        }
        Vector3 cardHolderDimensions = cardHolder.GetComponent<Renderer>().bounds.extents;

        float boardZDepth = col * (intercardPadding.z + cardHolderDimensions.z * 2) + (padding.x + padding.z) - intercardPadding.z;
        float boardXWidth = row * (intercardPadding.x + cardHolderDimensions.x * 2) + (padding.y + padding.w) - intercardPadding.x; 

        Transform board = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        board.localScale = new Vector3(boardXWidth, 1, boardZDepth);

        Vector3 topLeftOfBoard = new Vector3(board.position.x - boardXWidth/2,0.5f + board.position.y, board.position.z + boardZDepth/2);
        Vector3 nextCardHolderPosition = topLeftOfBoard +  new Vector3(padding.w,0,padding.x * -1) + new Vector3(cardHolderDimensions.x, cardHolderDimensions.y , cardHolderDimensions.z * - 1);

        float beginningOfRowX = nextCardHolderPosition.x;
        int opponentIDStartIndex = opponentID * row * col;

        for(int i = 0; i < col; ++i)
        {
            for(int a = 0; a < row; ++a)
            {
                Transform holder = GameObject.Instantiate(cardHolder).transform;
                holder.position = nextCardHolderPosition;
                nextCardHolderPosition += new Vector3(cardHolderDimensions.x*2 + intercardPadding.x,0,0);
                holder.SetParent(board);
                idToHolder[opponentIDStartIndex + a + i * row] = holder;
            }
            nextCardHolderPosition += new Vector3(0,0,(cardHolderDimensions.z*2  + intercardPadding.z) * -1);
            nextCardHolderPosition.x = beginningOfRowX;
        }

        return board;
    } 


    public static Transform[] ArrangeBoards(Dictionary<int, Transform> idToBoards, int targetID, Vector2 padding) // X is  distance from player to main board. Y is distance inbetween boards
    {
        int opponentCount = idToBoards.Count - 1;
        Transform mainBoard = idToBoards[targetID];
        mainBoard.transform.position = Vector3.zero;

        Vector3 extents = mainBoard.GetComponent<Renderer>().bounds.extents;

        Transform[] boards = new Transform[opponentCount];


        for(int opponentID = 0; opponentID < opponentCount ; ++opponentID )
        {
            boards[opponentCount - 1 - opponentID] = idToBoards[(targetID + 1 +opponentID) % idToBoards.Count ];
        }
        
        for(int opponentID = 0; opponentID < opponentCount; ++opponentID)
        {
            boards[opponentID].position = new Vector3(mainBoard.position.x + opponentID * (extents.x*2 + padding.y), mainBoard.position.y, mainBoard.position.z + padding.x + extents.z*2);
        }
        return boards;
    }

    public static void PositionCamera(Transform board, Transform camera, float height,float zOffset,  float angle)
    {
        Debug.Log($"{board.position.z} - {board.GetComponent<Renderer>().bounds.extents.z} - {zOffset} - {board.GetComponent<Renderer>().bounds.extents.z + zOffset} - {board.position.z - (board.GetComponent<Renderer>().bounds.extents.z + zOffset)}");
        camera.position =  new Vector3(board.position.x, board.position.y + height, board.position.z - (board.GetComponent<Renderer>().bounds.extents.z + zOffset));
        camera.rotation = Quaternion.Euler(new Vector3(angle,0,0));
        camera.SetParent(board);

    }


}
