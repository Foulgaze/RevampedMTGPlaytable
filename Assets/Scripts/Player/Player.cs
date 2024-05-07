using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// TO DO
// Change to use propertychangedeventhandlers
public class Player
{
    public string uuid {get;set;}
    public string name {get;set;}
    public Deck library {get; set;}
    public Deck exile {get;set;}
    public Deck graveyard {get;set;}
    public int id {get; set;}
    public int health {get;set;} = 40;
    

	public BoardComponents boardScript {get;set;}

    public PlayerDescriptionController playerDescriptionController{get;set;}
    public HandManager hand;

    public Player(string uuid, string name, int id, BoardComponents components, bool isClientPlayer)
    {
        this.uuid = uuid;
        this.name = name;
        this.id = id;
		this.boardScript = components;
        components.SetupDecks();
        SetupPiles(isClientPlayer);
    }

    void SetupPiles(bool isClientPlayer)
    {
        this.library = boardScript.library;
        this.library.LateConstructor(Piletype.library, id, $"{name}'s Library", isClientPlayer,false);
        this.library.deckID = Piletype.library;
		this.exile = boardScript.exile;
        this.exile.LateConstructor(Piletype.exile, id, $"{name}'s exile",isClientPlayer);
		this.graveyard = boardScript.graveyard;
        this.graveyard.LateConstructor(Piletype.graveyard, id, $"{name}'s graveyard",isClientPlayer);

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


    public Deck GetDeck(Piletype pile)
    {
        switch(pile)
        {
            default:
            case Piletype.graveyard: return graveyard;
            case Piletype.library: return library;
            case Piletype.exile: return exile;
        }
    }

    public void UpdateDecks(Dictionary<int, DeckDescriptor> newDecks)
    {
        HashSet<Card> allCards = boardScript.GetAllCardsOnBoard();
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
                    boardScript.UpdateDeck(currDescriptor, Piletype.leftField, allCards);
                    break;
                case (int) Piletype.mainField:
                    boardScript.UpdateDeck(currDescriptor, Piletype.mainField, allCards);
                    break;
                case (int) Piletype.rightField:
                    boardScript.UpdateDeck(currDescriptor, Piletype.rightField, allCards);
                    break;
            }
        }
        foreach(Card card in allCards)
        {
            card.ClearStats();
        }
    }

    
}