using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

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

    public (CardOnFieldBoard?, int) FindBoardContainingCard(int id)
    {
        int count = 0;
        foreach(Card iterCard in mainField.GetComponent<CardOnFieldBoard>().cards)
        {
            if(iterCard.id == id )
            {
                return (mainField.GetComponent<CardOnFieldBoard>(), count);
            }
            count += 1;
        }
        count = 0;
        foreach(Card iterCard in leftField.GetComponent<CardOnFieldBoard>().cards)
        {
            if(iterCard.id == id )
            {
                return (leftField.GetComponent<CardOnFieldBoard>(), count);
            }
            count += 1;
        }
        count = 0;
        foreach(Card iterCard in rightField.GetComponent<CardOnFieldBoard>().cards)
        {
            if(iterCard.id == id )
            {
                return (rightField.GetComponent<CardOnFieldBoard>(), count);
            }
            count += 1;
        }
        return (null, 0);
    }
    
    public HashSet<Card> GetAllCardsOnBoard()
    {
        HashSet<Card> cardsOnBoard = new HashSet<Card>();
        foreach(Transform transform in new Transform[]{leftField, rightField, mainField})
        {
            cardsOnBoard.AddRange(GetCardOnFieldBoard(transform).cards);
        }
        return cardsOnBoard;
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

    // TO DO 
    // REPLACE Trasnform of the boards with the cardonfieldboard component instead. Transform can be accessed from there
    CardOnFieldBoard GetCardOnFieldBoard(Transform passedBoard)
    {
        return passedBoard.GetComponent<CardOnFieldBoard>();
    }

    public DeckDescriptor GetDeckDescriptor(Piletype position)
    {
        return GetCardOnFieldBoard(piletypeToHolder[position]).GetDeckDescription(position);
    }

    public void UpdateDeck(DeckDescriptor deck, Piletype position, HashSet<Card> cardsAlreadyOnBoard)
    {
        GetCardOnFieldBoard(piletypeToHolder[position]).UpdateDeck(deck, cardsAlreadyOnBoard);
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
