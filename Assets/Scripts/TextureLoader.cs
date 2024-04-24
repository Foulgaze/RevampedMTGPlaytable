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
        if(GameManager.Instance.nameToSprite.ContainsKey(cardToBeTextured.info.name))
        {
            cardToBeTextured.SetSprite(GameManager.Instance.nameToSprite[cardToBeTextured.info.name]);
            return true;
        }

        string filepath = $"Assets/Resources/Textures/{cardToBeTextured.info.setCode}{cardToBeTextured.info.cardNumber}.jpg";
        if (File.Exists(filepath))
        {
            GameManager.Instance.nameToSprite[cardToBeTextured.info.name] = LoadSpriteFromFile(filepath);
            cardToBeTextured.SetSprite(GameManager.Instance.nameToSprite[cardToBeTextured.info.name]);
            return true;
        }

        bool addedCard = false;
        for(int i = 0; i < cardsToBeLoaded.Count; ++i)
        {
            CardTexture cardToTexture = cardsToBeLoaded.ElementAt(i);
            if(cardToTexture.cardInfo == cardToBeTextured.info)
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
    public IEnumerator GetSprite(string setCode, string cardNo, string name, Queue<Card> toBeTextured)
    {
        string filepath = $"Assets/Resources/Textures/{setCode}{cardNo}.jpg";
        // if (File.Exists(filepath))
        // {
        //     GameManager.Instance.nameToSprite[name] = LoadSpriteFromFile(filepath);
        //     TextureCards(toBeTextured);
        //     yield break;
        // }

        string url = $"https://api.scryfall.com/cards/{setCode.ToLower()}/{cardNo}?format=image&version=normal&face=front";
        string backupUrl = $"https://api.scryfall.com/cards/named?exact={HttpUtility.UrlEncode(name)}&format=image";
        

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
        GameManager.Instance.nameToSprite[name] = sprite;
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
            card.SetSprite(GameManager.Instance.nameToSprite[card.info.name]);
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
                CardInfo info = cardText.cardInfo;
                StartCoroutine(GetSprite(info.setCode,info.cardNumber,info.name, cardText.cardsToBeTextured));
            }
        }

    }


   
}