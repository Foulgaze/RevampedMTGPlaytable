using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using System.Diagnostics;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class CardInfo
{
    [Name("name")]
    public string name {get;set;}
    [Name("faceName")]
    public string faceName {get;set;}

    [Name("setCode")]
    public string setCode {get;set;}
    [Name("number")]
    public string cardNumber {get; set;}
    [Name("power")]
    public string power {get;set;}
    [Name("toughness")]

    public string toughness {get;set;}
    [Name("manaCost")]
    public string manaCost {get;set;}
    
    [Name("text")]
    public string text {get;set;}

    [Name("types")]
    public string types {get;set;}
    [Name("type")]
    public string type {get;set;}
    


    [Ignore]
    private Sprite cardSprite;
    [Ignore]
    public Queue<Image> missingSprites = new Queue<Image>();

    

    [Ignore]

    public Sprite CardSprite
    {
        get 
        { 
            return cardSprite; 
        }
        set
        {
            cardSprite = value;
            TextureItems();
        }
    }


    public Sprite GetCardSprite(Image i)
    {
        if(cardSprite != null)
        {
            return cardSprite;
        }
        AddToSpriteQueue(i);
        return null;
    }

    void AddToSpriteQueue(Image image)
    {
        missingSprites.Enqueue(image);
        TextureItems();
    }

    public void TextureItems() 
    {
        if(cardSprite == null)
        {
            return;
        }
        while(missingSprites.Count != 0)
        {
            Image missingSprite = missingSprites.Dequeue();
            if(missingSprite == null)
            {
                continue;
            }
            missingSprite.sprite = cardSprite;
        }
    }

}
public class FileLoader
{
	public static void LoadCSV(Dictionary<string,CardInfo> nameToCardInfo, string filePath)
	{

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var c = csv.GetRecord<CardInfo>();
                nameToCardInfo[c.name] = c;
                nameToCardInfo[c.faceName] = c;
            }
        }
	}

    public static int ParseCardList(string input,Dictionary<string,CardInfo> cards, Dictionary<string, int> nameToCount, List<string> missedCards)
    {
        int totalCount = 0;
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();

        string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if(line.StartsWith("//"))
            {
                continue;
            }
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2 && int.TryParse(parts[0], out int count))
            {
                string cardName = string.Join(' ', parts, 1, parts.Length - 1).TrimEnd( '\r', '\n' );
                if(cards.ContainsKey(cardName))
                {
                    nameToCount[cardName] = count;
                    totalCount += count;
                    continue;
                }
                UnityEngine.Debug.Log(cardName);
            }
            missedCards.Add(line);

        }

        return totalCount;
    }
}
