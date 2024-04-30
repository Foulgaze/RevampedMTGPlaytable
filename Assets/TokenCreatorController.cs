using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class TokenCreatorController : MonoBehaviour
{
    // TO DO 
    // ONE DAY ;)
    // [SerializeField]
    // TMP_InputField nameInput;
    // [SerializeField]
    // TMP_InputField powerToughnessInput;
    // [SerializeField]
    // Image cardImage;
    [SerializeField]
    Transform contentHolder;
    [SerializeField]
    Transform buttonPrefab;
    [SerializeField]
    GridLayoutGroup grid;

    float cellHeight = 50;

    CardInfo selectedCard;
    string GetButtonText(CardInfo info)
    {
        if(info.power != "")
        {
            return $"{info.name} - {info.power}/{info.toughness}";            
        }
        return info.name;
    }
    public void LoadTokens(Dictionary<string, TokenInfo> map)
    {
        grid.cellSize = new Vector2(contentHolder.GetComponent<RectTransform>().sizeDelta.x, cellHeight);
        RectTransform rt = contentHolder.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, map.Count * cellHeight);
        
        List<string> sortedKeys = new List<string>(map.Keys);
        sortedKeys.Sort(); // Sort the keys alphabetically
        
        foreach (string key in sortedKeys)
        {
            TokenInfo info = map[key];
            Transform newButton = Instantiate(buttonPrefab, contentHolder);
            newButton.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetButtonText(info);
            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => this.SetSelectedCard(info));
        }

    }


    void SetSelectedCard(CardInfo info)
    {
        selectedCard = info;
    }
    
    public void CreateToken()
    {
        if(selectedCard == null)
        {
            return;
        }
        GameManager.Instance.SendCreateRelatedCard(-1,selectedCard.name);
    }

    public void InitMenu()
    {
        gameObject.SetActive(true);
        selectedCard = null;

    }
}
