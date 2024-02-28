using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Piletype {graveyard, library, exile}
public class BoardComponents : MonoBehaviour
{
    [SerializeField]
    Transform physicalLibrary;
    [SerializeField]
    Transform physicalGraveyard;
    [SerializeField]
    Transform physicalExile;
    [SerializeField]
    Deck library;
    [SerializeField]
    Deck graveyard;
    [SerializeField]
    Deck exile;
    [SerializeField]
    Transform leftField;
    [SerializeField]
    Transform rightField;
    [SerializeField]
    Transform mainField;
    [SerializeField]
    Sprite cardBack;

    public bool isEnemyBoard = false;

    void Start()
    {
        SetValues();
    }
    void SetValues()
    {
        if(isEnemyBoard)
        {
            Flip90(physicalLibrary);
            Flip90(physicalGraveyard);
            Flip90(physicalExile);
            Flip90(leftField);
            Flip90(rightField);
            Flip90(mainField);
        }

    }

    void Flip90(Transform t)
    {
        Vector3 currentRotation = t.rotation.eulerAngles;
        t.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 180f, currentRotation.z);
    }
}
