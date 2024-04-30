using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Image = UnityEngine.UI.Image;
// TO DO
// Make it so a card knows where it is at all points.
// This means when you add a card to a container it should note that so it can easily be removed later
public class Card : ITextureable
{
	public int id {get;}

	static List<string> typesThatRelateCards = new List<string>(){"meld", "transform", "modal_dfc"};
	GameManager gameManager;

	public Sprite sprite {get;}
	public CardInfo info {get;}
	RectTransform inHandCardRect = null;
	Transform cardOnField = null;

	public int power;
	public int toughness;
	public string name;

	public bool interactable = true;
	public bool tapped = false;
	public bool ethereal = false;

	public string[]? twoSidedNames {get;set;}  = null;

	public Card(int id,CardInfo info, GameManager gameManager)
	{
		this.id = id;
		this.info = info;
		this.gameManager = gameManager;
		UpdatePowerToughness();
		HandleTwoSidedCard();
		SetName();
	}
	void SetName()
	{
		this.name = info.faceName == "" ? info.name : info.faceName;
	}

	void HandleTwoSidedCard()
	{
		if(!Card.typesThatRelateCards.Contains(info.layout))
		{
			return;
		}
		string[] cardNames = this.info.name.Split("//");
		if(cardNames.Count() != 2)
		{
			return;
		}
		cardNames[0] = cardNames[0].Trim();
		cardNames[1] = cardNames[1].Trim();
		gameManager.twoSidedCards.Add(cardNames[1]);
		twoSidedNames = cardNames;
	}

	public bool IsTwoSided()
	{
		return twoSidedNames != null;
	}

	public Card(Card card, int id,CardInfo info, GameManager gameManager)
	{
		// Needs to be refactored, essentially is just calling original constructor
		this.id = id;
		this.info = info;
		this.gameManager = gameManager;
		HandleTwoSidedCard();
		SetName();

		this.power = card.power;
		this.toughness = card.toughness;
		this.tapped = card.tapped;
		
		SetupEtherealCard();
		bool ptEnabled = card.GetCardOnField().GetComponent<CardOnFieldComponents>().cardPowerToughness.gameObject.activeInHierarchy;
		DisplayPowerToughness(ptEnabled);
		UpdateTapUntap();
	}

	 public void SetupEtherealCard()
	{
		this.ethereal = true;
		GetCardOnField();
		RectTransform inHand = GetInHandRect();
		DisableRect();
		inHand.SetParent(gameManager.handManager._handParent);
	}
	void UpdatePowerToughness()
	{
		int result = 0;
		int.TryParse(info.power, out result);
		power = result;
		int.TryParse(info.toughness, out result);
		toughness = result;
	}

	public void Destroy()
	{
		if(inHandCardRect != null)
		{
			GameObject.Destroy(inHandCardRect.gameObject);
		}
		if(cardOnField != null)
		{
			GameObject.Destroy(cardOnField.gameObject);
		}
	}

	public void DisplayPowerToughness(bool enablePTVisual)
	{
		if(cardOnField == null || !cardOnField.gameObject.activeInHierarchy)
		{
			return;
		}
		Transform physicalCard = GetCardOnField();
		CardOnFieldComponents components = physicalCard.GetComponent<CardOnFieldComponents>();
		if(enablePTVisual)
		{
			components.SetPowerToughnessState(true);
		}
		Debug.Log("Setting PT - {power}/{toughness}");
		components.cardPowerToughness.text = $"{power}/{toughness}";
	}

	public RectTransform GetInHandRect()
	{
		this.interactable = true;
		return GenerateInHandRect();
	}

	public RectTransform GetInHandRect(bool interactable)
	{
		this.interactable = interactable;
		return GenerateInHandRect();
	}

	RectTransform GenerateInHandRect()
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
		SetCardValues(newCard.GetComponent<CardComponents>());
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

	public void ResetPivot()
	{
		if(inHandCardRect == null)
		{
			return;
		}
		inHandCardRect.anchorMin = new Vector2(0.5f, 0.5f); // Center
        inHandCardRect.anchorMax = new Vector2(0.5f, 0.5f); // Center
        inHandCardRect.pivot = new Vector2(0.5f, 0.5f); // Center
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

	public void ClearStats()
	{
		tapped = false;
		if(cardOnField != null)
		{
			GameObject.Destroy(cardOnField.gameObject);
			UpdatePowerToughness();
			cardOnField = null;
		}
	}




	public void TapUntap()
	{
		tapped = !tapped;
		UpdateTapUntap();
	}
	
    public void UpdateTapUntap()
    {
        Transform cardOnField = GetCardOnField();
        cardOnField.GetComponent<CardOnFieldComponents>().tappedSymbol.gameObject.SetActive(tapped);
        if(tapped)
        {
            cardOnField.transform.rotation = Quaternion.Euler(new Vector3(0,gameManager.tapDegrees,0));
        }
        else
        {
            cardOnField.transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
        }
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
		components.cardName.text = name;
		if(info.power.Length == 0)
		{
			components.SetPowerToughnessState(false);
		}
		else
		{
			components.cardPowerToughness.text = $"{info.power}/{info.toughness}";
		}
	}

	void SetCardValues(CardComponents components)
    {
        components.cardDescription.text = info.text;
        components.cardName.text = name;
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

	public void TextureSelf(CardInfo info)
	{
		Sprite newSprite = gameManager.uuidToSprite[info.uuid];
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

	public CardInfo GetInfo()
	{
		return info;
	}
}