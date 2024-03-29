using System.Linq;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class Card
{
	public int id {get;}
	GameManager gameManager;

	public Sprite sprite {get;}
	public CardInfo info {get;}
	RectTransform inHandCardRect = null;
	Transform cardOnField = null;
	public CardOnFieldComponents onFieldComponents {get;set;}

	public CardContainer currentLocation {get;set;}
	public CardContainer originLocation {get;set;}
	public Card(int id,CardInfo info, GameManager gameManager)
	{
		this.id = id;
		this.info = info;
		this.gameManager = gameManager;
	}

	public RectTransform GetInHandRect()
	{
		if(inHandCardRect != null)
		{
			return inHandCardRect;
		}

		Transform newCard = GameObject.Instantiate(gameManager.cardInHandPrefab);
        newCard.GetComponent<CardMover>().card = this;
		inHandCardRect = newCard.GetComponent<RectTransform>();
        if(gameManager.textureLoader.TextureImage(this))
		{
			return inHandCardRect;
		}
		newCard = GameObject.Instantiate(gameManager.customCardInHandPrefab);
		inHandCardRect = newCard.GetComponent<RectTransform>();
		SetCardValues(info,newCard.GetComponent<CardComponents>());
        newCard.GetComponent<CardMover>().card = this;
        return inHandCardRect;
	}

	public void DisableRect()
	{
		if(inHandCardRect == null)
		{
			return;
		}
		inHandCardRect.gameObject.SetActive(false);
	}

	public void EnableRect()
	{
		if(inHandCardRect == null)
		{
			return;
		}
		inHandCardRect.gameObject.SetActive(true);
	}

	public void EnableOnFieldCard()
	{
		if(cardOnField == null)
		{
			return;
		}
		cardOnField.gameObject.SetActive(true);
	}

	public void DisableOnFieldCard()
	{
		if(cardOnField == null)
		{
			return;
		}
		cardOnField.gameObject.SetActive(false);
	}



	public Transform GetCardOnField()
	{
		if(cardOnField != null)
		{
			return cardOnField;
		}
		cardOnField = GameObject.Instantiate(gameManager.onFieldCard);		
        gameManager.textureLoader.TextureImage(this);
		cardOnField.GetChild(0).GetComponent<CardMover>().card = this;
		SetupOnFieldCard(cardOnField.GetComponent<CardOnFieldComponents>());
		return cardOnField;
	}

	void SetupOnFieldCard(CardOnFieldComponents components )
	{
		components.cardName.text = info.name;
		if(info.power.Length == 0)
		{
			components.SetPowerToughnessState(false);
		}
		else
		{
			components.cardPowerToughness.text = $"{info.power}/{info.toughness}";
		}
	}

	void SetCardValues(CardInfo info, CardComponents components)
    {
        components.cardDescription.text = info.text;
        components.cardName.text = info.name;
        components.cardType.text = info.type;
        components.manaCost.text = info.manaCost;
    }

	public void TextureImage (Image cardImage)
	{
		if(cardImage == null)
		{
			Debug.LogError("Null image passed to texturer");
			return;
		}
	}

	public void SetSprite(Sprite newSprite)
	{	
		if(inHandCardRect != null)
		{
			Image cardImage = inHandCardRect.transform.GetComponent<Image>();
			cardImage.sprite = newSprite;
			cardImage.color = Color.white;
			HelperFunctions.KillChildren(inHandCardRect);
		}	
		if(cardOnField != null)
		{
			cardOnField.GetComponent<CardOnFieldComponents>().cardArt.sprite = newSprite;
		}
	}
}