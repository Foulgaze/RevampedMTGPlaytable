using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
public enum CardZone {Library, Graveyard, Exile, CommandZone, MainField, LeftField, RightField, Hand}
public class Player
{
    public string Name {get;}
    public string Uuid {get;}
    public NetworkAttribute<string> DeckListRaw {get;}
    public NetworkAttribute<int> Health {get;}
    public NetworkAttribute<bool> ReadiedUp {get;set;}

    public bool GameStarted {get;} = false;

    private Dictionary<CardZone, CardContainer> zoneToContainer = new Dictionary<CardZone, CardContainer>();
    public Player(string uuid, string name, int startingHealth)
    {
        this.Uuid = uuid;
        this.Name = name;
        Health = NetworkAttributeManager.AddNetworkAttribute<int>(Uuid,startingHealth);
        DeckListRaw = NetworkAttributeManager.AddNetworkAttribute<string>(Uuid,"");
        ReadiedUp = NetworkAttributeManager.AddNetworkAttribute(Uuid,false);
        InitializeBoards();
    }

    void InitializeBoards()
    {
        foreach(CardZone zone in Enum.GetValues(typeof(CardZone)))
        {
            zoneToContainer[zone] = new CardContainer(zone, Uuid);
        }
    }

    /// <summary>
    /// Gets cardzone from player
    /// </summary>
    /// <param name="zone">Zone to get</param>
    /// <returns>Requested zone</returns>
    public CardContainer GetCardContainer(CardZone zone)
    {
        return zoneToContainer[zone];
    }

    /// <summary>
    /// Determines if the current user decklist is valid
    /// </summary>
    /// <returns>A list of the lines/names that were not able to be parsed</returns>
    public List<string> ValidateDeckList(Func<string, bool> validateCardName)
    {
        (List<string> cardNames,List<string> errorLines) = CardParser.ParseDeckList(DeckListRaw.Value);
        if(errorLines.Count > 0)
        {
            return errorLines;
        }
        cardNames = cardNames.Where(x => !validateCardName(x)).ToList();
        return cardNames;
    }
}