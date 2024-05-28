using System;
using System.Collections.Generic;

public class Card
{
	public int Id {get;}
	public CardInfo FrontInfo {get;}
	public CardInfo BackInfo {get;}
	public CardInfo CurrentInfo => isFlipped.Value ? BackInfo : FrontInfo;
	public CardContainer CurrentLocation {get;set;}
	public NetworkAttribute<int> power;
	public NetworkAttribute<int> toughness;
	public NetworkAttribute<bool> tapped;
	public NetworkAttribute<string> name;
	public NetworkAttribute<bool> isFlipped;
	public bool ethereal = false;


	public Card(int id,CardInfo FrontInfo, CardInfo BackInfo)
	{
		this.Id = id;
		this.FrontInfo = FrontInfo;
		this.BackInfo = BackInfo;

		InitializeAttributes();		
	}

	/// <summary>
	/// Checks if card has backside
	/// </summary>
	/// <returns><Returns true if the card has a backside/returns>
	public bool HasBackside()
	{
		return this.BackInfo != null;
	}

	private void InitializeAttributes()
    {
		this.isFlipped = NetworkAttributeManager.AddNetworkAttribute<bool>(Id.ToString(), false);
		this.isFlipped.valueChange += UpdateAttributes;	
        this.power = NetworkAttributeManager.AddNetworkAttribute<int>(Id.ToString(), ParsePT(CurrentInfo.power));
        this.toughness = NetworkAttributeManager.AddNetworkAttribute<int>(Id.ToString(), ParsePT(CurrentInfo.toughness));
        this.tapped = NetworkAttributeManager.AddNetworkAttribute<bool>(Id.ToString(), false);
    }


    private void UpdateAttributes(object sender, EventArgs e) // No need to network because flipped is netwokred
    {
        this.power.NonNetworkedSet(ParsePT(CurrentInfo.power));
        this.toughness.NonNetworkedSet(ParsePT(CurrentInfo.toughness));
		this.name.NonNetworkedSet(CurrentInfo.name);
    }

	private int ParsePT(string value)
	{
		int parsedValue;
		if(!Int32.TryParse(value, out parsedValue))
		{
			return 0;
		}
		return parsedValue;
	}

}