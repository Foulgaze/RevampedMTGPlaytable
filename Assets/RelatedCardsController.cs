using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RelatedCardsController : MonoBehaviour
{
    [SerializeField]
    Transform relatedCardButtonPrefab;
    [SerializeField]
    Transform relatedCardsHolder;

    public void InitMenu(OnFieldCardRightClickController controller)
    {
        ClearMenu();

        Card card = controller.card;
        gameObject.SetActive(true);
        HashSet<string> relatedCards = GetRelatedCards(card);
        CreateMenuButtons(card, relatedCards);
    }

    private void ClearMenu()
    {
        foreach(Transform child in relatedCardsHolder)
        {
            Destroy(child.gameObject);
        }
    }

    private HashSet<string> GetRelatedCards(Card card)
    {
        HashSet<string> relatedCards = new HashSet<string>();
        if (GameManager.Instance.nameToRelatedCards.ContainsKey(card.name))
        {
            relatedCards.AddRange(GameManager.Instance.nameToRelatedCards[card.name]);
        }
        if (card.IsTwoSided())
        {
            relatedCards.Add(card.twoSidedNames[1]);
        }
        Debug.Log($"{card.IsTwoSided()} - {relatedCards.Count}");
        return relatedCards;
    }

    private void CreateMenuButtons(Card card, HashSet<string> relatedCards)
    {
        foreach (string cardName in relatedCards)
        {
            if (cardName.Contains("Checklist") || cardName == card.name)
            {
                Debug.Log($"{cardName} - {card.name}");
                continue;
            }
            Transform newButton = Instantiate(relatedCardButtonPrefab, relatedCardsHolder);
            newButton.GetChild(0).GetComponent<TextMeshProUGUI>().text = cardName;
            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => GameManager.Instance.SendCreateRelatedCard(card, cardName));
            button.onClick.AddListener(() => GameManager.Instance._uiManager.Disable(transform));
            button.onClick.AddListener(() => Debug.Log("SENDING"));

        }
    }

}
