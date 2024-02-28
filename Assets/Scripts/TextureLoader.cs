using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TextureLoader : MonoBehaviour
{
    [SerializeField]
    Texture2D cardback;
    
    [SerializeField]
    Transform _HandHolder;
  

    public IEnumerator GetSprite(string setCode, string cardNo, string name)
    {
        string filepath = $"Assets/Resources/Textures/{setCode}{cardNo}.jpg";
        if (File.Exists(filepath))
        {
            yield break;
        }

        string url = $"https://api.scryfall.com/cards/{setCode.ToLower()}/{cardNo}?format=image&version=normal&face=front";

        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(url);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"{url} Error"); // LOG ERRORS
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
            byte[] pngBytes = myTexture.EncodeToPNG();
            File.WriteAllBytes(filepath, pngBytes);

            Sprite sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.one * 0.5f);

            GetComponent<SpriteRenderer>().sprite = sprite;
            CardInfo card = GameManager.Instance.nameToCardInfo[name];
            card.CardSprite = sprite;
        }
    }


   
}