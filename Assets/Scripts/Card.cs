using UnityEngine;
using Image = UnityEngine.UI.Image;
public class Card
{
	public int id {get;}
	GameManager gameManager;

	public Sprite sprite {get;}
	public CardInfo info {get;}
	RectTransform inHandCardRect;
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
        newCard.GetComponent<CardMover>().info = info;
		inHandCardRect = newCard.GetComponent<RectTransform>();
        if(gameManager.textureLoader.TextureImage(this))
		{
			return inHandCardRect;
		}
		newCard = GameObject.Instantiate(gameManager.customCardInHandPrefab);
		inHandCardRect = newCard.GetComponent<RectTransform>();
		SetCardValues(info,newCard.GetComponent<CardComponents>());
        newCard.GetComponent<CardMover>().info = info;
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



	public Transform GetOnFieldCard(Transform cardOnFieldPrefab)
	{
		if(onFieldComponents != null)
		{
			return onFieldComponents.transform;
		}
		Transform newCard = GameObject.Instantiate(cardOnFieldPrefab);
        gameManager.textureLoader.TextureImage(this);
		return inHandCardRect;
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
		if(onFieldComponents != null)
		{
			onFieldComponents.cardArt.sprite = newSprite;
		}
	}
}