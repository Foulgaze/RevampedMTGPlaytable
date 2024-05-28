using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class Playtable
{
	private List<Player> _players = new List<Player>();
	
	private NetworkManager _networkManager;

	public NetworkAttribute<int> readyUpNeeded;

	public bool GameStarted {get;set;} = false;

	public event PropertyChangedEventHandler gameStarted = delegate { };
	
	public Playtable(NetworkManager networkManager, int readyUpCount)
	{
		this._networkManager = networkManager;
		this.readyUpNeeded = new NetworkAttribute<int>("0",readyUpCount);
	}

	/// <summary>
	/// Adds a player to the table
	/// </summary>
	/// <param name="name"> Name of the player</param>
	/// <param name="uuid"> UUID of player</param>
	public void AddPlayer(string name, string uuid)
	{
		if(GameStarted)
		{
			return;
		}
		Player player = new Player(uuid, name,40);
		player.ReadiedUp.valueChange += CheckForStartGame;
		_players.Add(player);
	}

	/// <summary>
	/// Gets a player in the game
	/// </summary>
	/// <param name="uuid">UUID of desired player</param>
	/// <returns>Player or null depending of if uuid exists</returns>
	public Player? GetPlayer(string uuid)
	{
		return _players.FirstOrDefault(player => player.Uuid == uuid);;
	}

	void CheckForStartGame(object obj, PropertyChangedEventArgs args)
	{
		int readyCount = _players.Count(player => player.ReadiedUp.Value);
		if (readyCount >= readyUpNeeded.Value)
		{
			StartGame();
		}
	}

	void StartGame()
	{
		GameStarted = true;
		SetupDecks();
		gameStarted(this, new PropertyChangedEventArgs("Started"));
	}

	void SetupDecks()
	{
		foreach(Player player in _players)
		{
			(List<string> cards, _) = CardParser.ParseDeckList(player.DeckListRaw.Value);
			CardContainer library = player.GetCardContainer(CardZone.Library);
		}
	}


	public bool RemovePlayer(string uuid)
	{
		Player player = GetPlayer(uuid);
		if (player != null)
		{
			return _players.Remove(player);
		}
		return false;
	}
}