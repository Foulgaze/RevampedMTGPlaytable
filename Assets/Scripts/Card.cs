public class Card
{
	public int id {get;}
	public CardInfo info {get;}

	public CardContainer currentLocation {get;set;}
	public Card(int id,CardInfo info)
	{
		this.id = id;
		this.info = info;
	}
}