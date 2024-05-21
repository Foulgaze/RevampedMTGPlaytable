using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMovement : MonoBehaviour
{
    int _currentIndex;
    Player[] _players;

    Transform clientBoard;

    Vector3 _endPosition;
    float _sinTime;
    float _moveSpeed = 6;

    float _movementAllowance = 0.1f;
    bool _moving = false;

    public void SetValues(Player[] players, Player clientPlayer)
    {
        _players = players;
        clientBoard = clientPlayer.boardScript.transform;
    }

    void CheckForMovement()
    {
        if(!GameManager.Instance._gameStarted || (!Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow)) || _moving || _players.Count() < 1)
        {
            return;
        }
        _moving = true;
        _sinTime = 0;
        _currentIndex = Input.GetKeyDown(KeyCode.RightArrow) ? _currentIndex + 1 : _currentIndex - 1;
        _currentIndex = _currentIndex < 0 ? _players.Length - 1 : _currentIndex;
        _currentIndex %= _players.Length;
        Transform nextBoard = _players[_currentIndex].boardScript.transform;
        _endPosition = new Vector3(nextBoard.position.x, clientBoard.position.y, clientBoard.position.z);
        GameManager.Instance.playerDescriptionController.UpdateHealthBars();
    }

    public Player GetCurrentPlayer()
    {
        return _players[_currentIndex];
    }

    void UpdateMovement()
    {
        if(!_moving)
        {
            return;
        }
        if(Mathf.Abs(clientBoard.position.x - _endPosition.x) <= _movementAllowance)
        {
            _sinTime = 0;
            _moving = false;
            clientBoard.position = _endPosition;
            return;
        }
        _sinTime += Time.deltaTime * _moveSpeed;
        _sinTime = Mathf.Clamp(_sinTime, 0, Mathf.PI);
        float t = evaluate(_sinTime);
        clientBoard.position = Vector3.Lerp(clientBoard.position, _endPosition, t);
    }
    
    void FixedUpdate()
    {
        if(_moving)
        {
            UpdateMovement();
            return;
        }
        CheckForMovement();       
    }

    float evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - MathF.PI / 2f) * 0.5f;
    }
}
