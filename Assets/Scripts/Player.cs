public class Player
{
    public string uuid {get;set;}
    public string name {get;set;}
    public Deck library {get; set;} = new Deck();
    public Deck exile {get;set;} = new Deck();
    public Deck graveyard {get;set;} = new Deck();
    public int id {get; set;}

	public BoardComponents boardScript {get;set;}

    public HandManager hand;

    public Player(string uuid, string name, int id, BoardComponents components)
    {
        this.uuid = uuid;
        this.name = name;
        this.id = id;
		this.boardScript = components;
		this.library = components.GetComponent<Deck>();
        this.library._revealTopCard = false;
		this.exile = components.GetComponent<Deck>();
		this.library = components.GetComponent<Deck>();

    }

    public void DrawCard()
    {
        Card? drawnCard = library.DrawCard();
        if(drawnCard == null)
        {
            return;
        }

        // hand.CreateAndAddCardToHand(drawnCard);

    }
    
}