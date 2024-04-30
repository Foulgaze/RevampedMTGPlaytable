using System;
using UnityEngine;
using UnityEngine.UI;
public class HoveredCardController : ITextureable
{

	[SerializeField]
	Image cardImage;
	
	CardInfo info;
	GameManager gameManager;
	public HoveredCardController(GameManager manager, Image image)
	{
		gameManager = manager;
		cardImage = image;
	}
	public CardInfo GetInfo()
	{
		return info;
	}

	public void ChangeHoveredCard(Card card)
	{
		this.info = card.info;
		gameManager.textureLoader.TextureImage(this);
	}

	public void TextureSelf(CardInfo info)
	{
		if(info != this.info)
		{
			return;
		}
		cardImage.sprite = gameManager.uuidToSprite[info.uuid];
	}
}