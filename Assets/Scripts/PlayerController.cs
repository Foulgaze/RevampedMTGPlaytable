using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance._gameStarted)
        {
            return;
        }
        if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.D) && GameManager.Instance._gameStarted)
        {
            if(GameManager.Instance.clientPlayer.DrawCard())
            {
                GameManager.Instance.SendUpdatedDecks();
            }
        }
    }
}
