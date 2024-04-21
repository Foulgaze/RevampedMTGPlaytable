using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Piletype {graveyard, library, exile, leftField, rightField, mainField}
public class BoardComponents : MonoBehaviour
{
    [SerializeField]
    Transform physicalLibrary;
    [SerializeField]
    Transform physicalGraveyard;
    [SerializeField]
    Transform physicalExile;
    [HideInInspector]
    public Deck library;
    [HideInInspector]

    public Deck graveyard;
    [HideInInspector]

    public Deck exile;
    [SerializeField]
    Transform leftField;
    [SerializeField]
    Transform rightField;
    [SerializeField]
    Transform mainField;
    [SerializeField]
    Sprite cardBack;

    public bool isEnemyBoard = false;

    Dictionary<Piletype, Transform> piletypeToHolder;

    void Start()
    {
        piletypeToHolder = new Dictionary<Piletype, Transform>(){{Piletype.leftField , leftField},{Piletype.rightField , rightField}, {Piletype.mainField , mainField} };
        SetValues();
    }

    public void SetupDecks()
    {
        library = SetupDeck(physicalLibrary);
        graveyard = SetupDeck(physicalGraveyard);
        exile = SetupDeck(physicalExile);
    }

    Deck SetupDeck(Transform physicalDeck)
    {
        Deck returnDeck = gameObject.AddComponent<Deck>();
        physicalDeck.GetComponent<CardOnFieldContainer>().SetValues(returnDeck);
        returnDeck.physicalDeck = physicalDeck.GetComponent<PileController>();
        return returnDeck;
    }

    public void SetValues()
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

    public void SetBoardValues(int id)
    {
        SetBoardValue(leftField, id, 4);
        SetBoardValue(rightField, id, 4);
        SetBoardValue(mainField, id, 8);
    }

    public DeckDescriptor GetDeckDescriptor(Piletype position)
    {
        return piletypeToHolder[position].GetComponent<CardOnFieldBoard>().GetDeckDescription(position);
    }

    public void UpdateDeck(DeckDescriptor deck, Piletype position)
    {
        piletypeToHolder[position].GetComponent<CardOnFieldBoard>().UpdateDeck(deck);
    }
    void SetBoardValue(Transform field, int id, int cardCount)
    {
        CardOnFieldBoard cardHolder = field.GetComponent<CardOnFieldBoard>();
        cardHolder.owner = id;
        cardHolder.cardCount = cardCount;
        cardHolder.SetValues();
    }

    void Flip90(Transform t)
    {
        Vector3 currentRotation = t.rotation.eulerAngles;
        t.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 180f, currentRotation.z);
    }
}
