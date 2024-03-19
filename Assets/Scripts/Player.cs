using System.Collections.Generic;

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
		this.exile = boardScript.exile;
        this.exile.deckID = Piletype.exile;
		this.graveyard = boardScript.graveyard;
		this.graveyard.deckID = Piletype.graveyard;
    }

    public void DrawCard()
    {
        Card? drawnCard = library.DrawCard();
        if(drawnCard == null)
        {
            return;
        }
        hand.AddCardToContainer(drawnCard, null);
        // hand.CreateAndAddCardToHand(drawnCard);
    }

    public Dictionary<int, DeckDescriptor> GetDeckDescriptions()
    {
        Dictionary<int, DeckDescriptor> returnDict = new Dictionary<int,DeckDescriptor>();
        returnDict[(int) library.deckID] = library.GetDeckDescription();
        returnDict[(int) graveyard.deckID] = graveyard.GetDeckDescription();
        returnDict[(int) exile.deckID] = exile.GetDeckDescription();
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
            }
        }
    }

    
}