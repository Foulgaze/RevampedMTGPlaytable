using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string uuid {get;set;}
    public string name {get;set;}
    public Deck library {get; set;}
    public Deck exile {get;set;}
    public Deck graveyard {get;set;}
    public int id {get; set;}

	public BoardComponents boardScript {get;set;}

    public HandManager hand;

    public Player(string uuid, string name, int id, BoardComponents components)
    {
        this.uuid = uuid;
        this.name = name;
        this.id = id;
		this.boardScript = components;
        components.SetupDecks();
        SetupPiles();
    }

    void SetupPiles()
    {
        this.library = boardScript.library;
        this.library._revealTopCard = false;
        this.library.deckID = Piletype.library;
        this.library.owner = id;
		this.exile = boardScript.exile;
        this.exile.deckID = Piletype.exile;
        this.exile.owner = id;
		this.graveyard = boardScript.graveyard;
		this.graveyard.deckID = Piletype.graveyard;
        this.graveyard.owner = id;
    }

    public bool DrawCard()
    {
        Card? drawnCard = library.DrawCard();
        if(drawnCard == null)
        {
            return false;
        }
        hand.AddCardToContainer(drawnCard, null);
        return true;
    }

    public Dictionary<int, DeckDescriptor> GetDeckDescriptions()
    {
        Dictionary<int, DeckDescriptor> returnDict = new Dictionary<int,DeckDescriptor>();
        returnDict[(int) library.deckID] = library.GetDeckDescription();
        returnDict[(int) graveyard.deckID] = graveyard.GetDeckDescription();
        returnDict[(int) exile.deckID] = exile.GetDeckDescription();
        returnDict[(int) Piletype.leftField] = boardScript.GetDeckDescriptor(Piletype.leftField);
        returnDict[(int) Piletype.rightField] = boardScript.GetDeckDescriptor(Piletype.rightField);
        returnDict[(int) Piletype.mainField] = boardScript.GetDeckDescriptor(Piletype.mainField);
        return returnDict;
    }

    public void UpdateDecks(Dictionary<int, DeckDescriptor> newDecks)
    {
        foreach(int deck in newDecks.Keys)
        {
            DeckDescriptor currDescriptor = newDecks[deck];
            switch(currDescriptor.deckID)
            {
                case (int) Piletype.library: 
                    library.UpdateDeck(newDecks[deck]);
                    break;
                case (int) Piletype.graveyard : 
                    graveyard.UpdateDeck(newDecks[deck]);
                    break;
                case (int) Piletype.exile : 
                    exile.UpdateDeck(newDecks[deck]);
                    break;
                case (int) Piletype.leftField:
                    boardScript.UpdateDeck(currDescriptor, Piletype.leftField);
                    break;
                case (int) Piletype.mainField:
                    boardScript.UpdateDeck(currDescriptor, Piletype.mainField);
                    break;
                case (int) Piletype.rightField:
                    boardScript.UpdateDeck(currDescriptor, Piletype.rightField);
                    break;
            }
        }
    }

    
}