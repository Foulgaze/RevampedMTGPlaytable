using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardMovement : MonoBehaviour
{
    int _currentIndex;
    Transform[] _boards;

    Transform _board;

    Vector3 _endPosition;
    float _sinTime;
    float _moveSpeed = 6;

    float _movementAllowance = 0.1f;
    bool _moving = false;

    public void SetValues(Transform[] boards, Transform board)
    {
        _boards = boards;
        _board = board;
    }

    void CheckForMovement()
    {
        if((!Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow)) || _moving)
        {
            return;
        }
        _moving = true;
        _sinTime = 0;
        _currentIndex = Input.GetKeyDown(KeyCode.RightArrow) ? _currentIndex + 1 : _currentIndex - 1;
        _currentIndex = _currentIndex < 0 ? _boards.Length - 1 : _currentIndex;
        _currentIndex %= _boards.Length;
        _endPosition = new Vector3(_boards[_currentIndex].position.x, _board.position.y, _board.position.z);
    }

    void UpdateMovement()
    {
        if(!_moving)
        {
            return;
        }
        if(Mathf.Abs(_board.position.x - _endPosition.x) <= _movementAllowance)
        {
            _sinTime = 0;
            _moving = false;
            _board.position = _endPosition;
            return;
        }
        _sinTime += Time.deltaTime * _moveSpeed;
        _sinTime = Mathf.Clamp(_sinTime, 0, Mathf.PI);
        float t = evaluate(_sinTime);
        _board.position = Vector3.Lerp(_board.position, _endPosition, t);
    }
    
    void Update()
    {
        if(_moving)
        {
            UpdateMovement();
        }
        CheckForMovement();       
    }

    float evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - MathF.PI / 2f) * 0.5f;
    }
}
