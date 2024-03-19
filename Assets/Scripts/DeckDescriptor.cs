public class DeckDescriptor
{
	public int cardCount {get;}
	public int topCard {get;}
	public int deckID {get;}
	public bool revealTop {get;}


	public DeckDescriptor(int cardCount, int topCard, int deckID, bool revealTop)
	{
		this.cardCount = cardCount;
		this.topCard = topCard;
		this.deckID = deckID;
		this.revealTop = revealTop;
	}

}