using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
public class FileLoader
{
	
	public static void LazyLoadCSV(Dictionary<String,String> nameToLine, string fileName)
	{
		  
        DateTime before = DateTime.Now;     
        int count = 0;
        if (csvFile != null)
        {
            StringReader reader = new StringReader(csvFile.text);

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                
                string[] fields = line.Split("\t");
                string originalName = fields[50].Trim('"');
                (string? frontname, string? backname) = ExtractName(originalName);
  
                CardData cardData = new CardData
                {
                    FrontName = frontname, // Name
                    Subtype = fields[69], // Subtype
                    Supertype = fields[70], // Supertype
                    Types = fields[74], // Types
                    Power = fields[57], // Power
                    Toughness = fields[72], // Toughness
                    OriginalText = fields[54], // Original Text
                    ManaCost = fields[49], // Mana Cost
                    BackName = backname
                };
                cardDataDict[cardData.FrontName] = cardData;
                cardDataDict[originalName] = cardData;

                count += 1;
            }

                reader.Close();
        }
        else
        {
            Debug.LogError("CSV file not assigned.");
        }
	}

	public static void LoadSpecificCard()
	{

	}
}
