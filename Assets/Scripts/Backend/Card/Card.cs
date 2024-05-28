using System;
using System.Collections.Generic;

public class Card
{
	public int Id {get;}

	static List<string> typesThatRelateCards = new List<string>(){"meld", "transform", "modal_dfc"};

	public CardInfo FrontInfo {get;}
	public CardInfo BackInfo {get;}
	public CardInfo CurrentInfo {get;}
	public CardContainer CurrentLocation {get;set;}
	public NetworkAttribute<int> power;
	public NetworkAttribute<int> toughness;
	public NetworkAttribute<bool> tapped;
	public NetworkAttribute<string> name;
	public bool ethereal = false;

	public Card(int id,CardInfo FrontInfo, CardInfo BackInfo)
	{
		this.Id = id;
		this.FrontInfo = FrontInfo;
		this.BackInfo = BackInfo;

		power = NetworkAttributeManager.AddNetworkAttribute<int>(id.ToString(), ParsePT(Info.power));
		toughness = NetworkAttributeManager.AddNetworkAttribute<int>(id.ToString(), ParsePT(Info.toughness));
		tapped = NetworkAttributeManager.AddNetworkAttribute<bool>(id.ToString(),false);
		
	}

	/// <summary>
	/// Check if current card info is backside
	/// </summary>
	/// <returns>If card is flipped</returns>
	public bool IsFlipped()
	{		
		return CurrentInfo != FrontInfo;
	}

	public bool FlipCard()
	{	
	
	}

    private void UpdateAttributes()
    {
        power.Value = ParsePT(CurrentInfo.power);
        toughness.Value = ParsePT(CurrentInfo.toughness);
        // Update any other attributes as necessary
    }

	int ParsePT(string value)
	{
		int parsedValue;
		if(!Int32.TryParse(value, out parsedValue))
		{
			return 0;
		}
		return parsedValue;
	}

}