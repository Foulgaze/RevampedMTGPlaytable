using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using UnityEngine;
using System.Linq;
public class CardInfo
{
    [Name("name")]
    public string name {get;set;}
    [Name("faceName")]
    public string faceName {get;set;} = "";

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

    // [Name("types")]
    // public string types {get;set;}
    [Name("type")]
    public string type {get;set;}
    [Name("layout")]
    public string layout {get;set;}
    [Name("uuid")]
public string uuid {get;set;}
}

public class TokenInfo : CardInfo
{
    [Name("relatedCards")]
    public string relatedCards {get;set;}
}
public class FileLoader
{
	public static void LoadAllCardsCSV(Dictionary<string,CardInfo> nameToCardInfo, string filePath)
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

    public static void LoadTokenCSV(Dictionary<string, HashSet<string>> nameToRelatedCards,Dictionary<string, TokenInfo> nameToToken, string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var token = csv.GetRecord<TokenInfo>();
                if(token.relatedCards.Count() == 0)
                {
                    continue;
                }
                Dictionary<string, List<string>> relatedCards;
                // BLEH Way of doing it, im aware!
                try
                {
                    relatedCards = JsonConvert.DeserializeObject<Dictionary<string,List<string>>>(token.relatedCards);
                }
                catch
                {
                    try
                    {
                        relatedCards = JsonConvert.DeserializeObject<Dictionary<string,List<string>>>(token.relatedCards.Replace("\\\\", "\\"));
                    }
                    catch
                    {
                        Debug.LogError($"Skipped - {token.relatedCards}");
                        continue;
                    }
                }

                foreach(string relatedCard in relatedCards["reverseRelated"])
                {
                    if(!nameToRelatedCards.ContainsKey(relatedCard))
                    {
                        nameToRelatedCards[relatedCard] = new HashSet<string>();
                    }
                    nameToRelatedCards[relatedCard].Add(token.name);
                }
                nameToToken[token.name] = token;
                if(token.name == "Goat")
                {
                    Debug.Log("HERE");
                }
                
                
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
