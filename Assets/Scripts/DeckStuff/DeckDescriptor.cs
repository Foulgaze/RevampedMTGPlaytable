using System.Collections.Generic;

public class DeckDescriptor
{
	public int cardCount {get;}
	public int topCard {get;}
	public int deckID {get;}
	public bool revealTop {get;}

	public List<int> cards {get;}


	public DeckDescriptor(int cardCount, int topCard, int deckID, bool revealTop, List<int> cards)
	{
		this.cardCount = cardCount;
		this.topCard = topCard;
		this.deckID = deckID;
		this.revealTop = revealTop;
		this.cards = cards;
	}
}


public class LibraryDescriptor
{
	public List<int> cards {get;}
	public List<int> allCards {get;}
	public Piletype deckID {get;}
	public List<string> uuids {get;}
	public int? cardShowCount {get;}


	public LibraryDescriptor(List<int> cards, Piletype deckID, List<string> uuids, int? cardShowCount)
	{
		this.cards = cards;
		this.deckID = deckID;
		this.uuids = uuids;
		this.cardShowCount = cardShowCount;
	}
}