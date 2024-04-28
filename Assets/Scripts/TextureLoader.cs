using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Web;

public class TextureLoader : MonoBehaviour
{
    Queue<CardTexture> cardsToBeLoaded = new Queue<CardTexture>();
    float timer = 0;
    float cooldownPeriod = 0.1f; // 10 miliseconds
  
    
    public bool TextureImage(Card cardToBeTextured)
    {
        Debug.Log($"Texturing - {cardToBeTextured.name} - {cardToBeTextured.info.faceName}");
        if(GameManager.Instance.nameToSprite.ContainsKey(cardToBeTextured.info.uuid))
        {
            cardToBeTextured.SetSprite(GameManager.Instance.nameToSprite[cardToBeTextured.info.uuid]);
            return true;
        }

        string filepath = $"Assets/Resources/Textures/{cardToBeTextured.info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            GameManager.Instance.nameToSprite[cardToBeTextured.info.uuid] = LoadSpriteFromFile(filepath);
            cardToBeTextured.SetSprite(GameManager.Instance.nameToSprite[cardToBeTextured.info.uuid]);
            return true;
        }

        bool addedCard = false;
        for(int i = 0; i < cardsToBeLoaded.Count; ++i)
        {
            CardTexture cardToTexture = cardsToBeLoaded.ElementAt(i);
            if(cardToTexture.card == cardToBeTextured)
            {
                cardToTexture.cardsToBeTextured.Enqueue(cardToBeTextured);
                addedCard = true;
                break;
            }
        }
        if(!addedCard)
        {
            cardsToBeLoaded.Enqueue(new CardTexture(cardToBeTextured));
        }
        return false;
    }
    private Sprite LoadSpriteFromFile(string filepath)
    {
        byte[] fileData = File.ReadAllBytes(filepath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        return sprite;
    }
    // TODO make multiple sources that images can be gathered from. 
    public IEnumerator GetSprite(Card card, Queue<Card> toBeTextured)
    {
        string name = card.info.faceName == "" ? card.info.name : card.info.faceName; 
        Debug.Log($"Loading {name}");
        string filepath = $"Assets/Resources/Textures/{card.info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            GameManager.Instance.nameToSprite[card.info.uuid] = LoadSpriteFromFile(filepath);
            TextureCards(toBeTextured);
            yield break;
        }

        string face = GameManager.Instance.twoSidedCards.Contains(name) && card.info.layout != "meld" ? "back" : "front";
        string url = $"https://api.scryfall.com/cards/{card.info.setCode.ToLower()}/{card.info.cardNumber}?format=image&version=normal&face={face}";
        string backupUrl = $"https://api.scryfall.com/cards/named?exact={HttpUtility.UrlEncode(name)}&format=image&face={face}";
        
        

        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(url);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            textureRequest = UnityWebRequestTexture.GetTexture(backupUrl);
            yield return textureRequest.SendWebRequest();
            if (textureRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Both URLS failed to fetch tectures #1={url} #2={backupUrl}"); 
                yield break;
            }
        }
        
        Texture2D myTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        byte[] pngBytes = myTexture.EncodeToPNG();
        File.WriteAllBytes(filepath, pngBytes);

        Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.one * 0.5f);
        GameManager.Instance.nameToSprite[card.info.uuid] = sprite;
        TextureCards(toBeTextured);
    }

    void TextureCards(Queue<Card> cardsToBeTextured)
    {
        while(cardsToBeTextured.Count != 0)
        {
            Card card = cardsToBeTextured.Dequeue();
            if(card == null)
            {
                continue;
            }
            card.SetSprite(GameManager.Instance.nameToSprite[card.info.uuid]);
        }
    }

    

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > cooldownPeriod)
        {
            timer = 0;
            if(cardsToBeLoaded.Count != 0)
            {
                CardTexture cardText = cardsToBeLoaded.Dequeue();
                Card card = cardText.card;
                StartCoroutine(GetSprite(card, cardText.cardsToBeTextured));
            }
        }

    }


   
}