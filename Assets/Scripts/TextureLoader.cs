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
    public bool TextureImage(ITextureable toBeTextured)
    {
        Debug.Log($"Starting Texture - {toBeTextured.GetInfo().name}");
        CardInfo info = toBeTextured.GetInfo();
        if(GameManager.Instance.uuidToSprite.ContainsKey(info.uuid))
        {
            toBeTextured.TextureSelf(info);
            return true;
        }

        string filepath = $"Assets/Resources/Textures/{info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            GameManager.Instance.uuidToSprite[info.uuid] = LoadSpriteFromFile(filepath);
            toBeTextured.TextureSelf(info);
            return true;
        }

        bool addedCard = false;
        for(int i = 0; i < cardsToBeLoaded.Count; ++i)
        {
            CardTexture cardToTexture = cardsToBeLoaded.ElementAt(i);
            if(cardToTexture.info == info)
            {
                cardToTexture.Enqueue(toBeTextured);
                addedCard = true;
                break;
            }
        }
        if(!addedCard)
        {
            CardTexture newTexture = new CardTexture(info);
            newTexture.Enqueue(toBeTextured);
            cardsToBeLoaded.Enqueue(newTexture);
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
    public IEnumerator GetSprite(CardInfo info, Queue<ITextureable> toBeTextured)
    {
        string name = info.faceName == "" ? info.name : info.faceName; 
        string filepath = $"Assets/Resources/Textures/{info.uuid}.jpg";
        if (File.Exists(filepath))
        {
            GameManager.Instance.uuidToSprite[info.uuid] = LoadSpriteFromFile(filepath);
            TextureCards(info,toBeTextured);
            yield break;
        }

        string face = GameManager.Instance.twoSidedCards.Contains(name) && info.layout != "meld" ? "back" : "front";
        string url = $"https://api.scryfall.com/cards/{info.setCode.ToLower()}/{info.cardNumber}?format=image&version=normal&face={face}";
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
        GameManager.Instance.uuidToSprite[info.uuid] = sprite;
        TextureCards(info, toBeTextured);
    }

    void TextureCards(CardInfo info,Queue<ITextureable> cardsToBeTextured)
    {
        while(cardsToBeTextured.Count != 0)
        {
            ITextureable textureableObject = cardsToBeTextured.Dequeue();
            textureableObject.TextureSelf(info);
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
                CardInfo info = cardText.info;
                Debug.Log($"Dequeing - {info.name}");
                StartCoroutine(GetSprite(info, cardText.cardsToBeTextured));
            }
        }

    }


   
}