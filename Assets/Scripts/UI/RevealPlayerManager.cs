using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevealPlayerManager : MonoBehaviour
{
    [SerializeField] Transform userToggle;
    [SerializeField] Transform toggleHolder;
    [HideInInspector] public Deck selectedDeck;

    List<(Toggle, string)> toggleables = new List<(Toggle, string)>();

    [SerializeField] Button revealButton;
    public TMP_InputField cardCountInput;

    public bool hasCardCount;

    public void InitMenu(List<Player> playersToSelect, Deck? deck, bool passToCardCountWindow)
    {
        Cleanup();
        selectedDeck = deck;
        gameObject.SetActive(true);
        hasCardCount = passToCardCountWindow;
        CreateToggles(playersToSelect);
        if(passToCardCountWindow)
        {
            revealButton.onClick.AddListener(() => {GameManager.Instance._uiManager.EnableRevealTopCardsToPlayers(this);});
        }
        else
        {
            revealButton.onClick.AddListener(() => {GameManager.Instance.SendRevealedDeck(this);});
        }
        revealButton.onClick.AddListener(() => {GameManager.Instance._uiManager.Disable(this.transform);});

        transform.SetAsLastSibling();
    }


    void CreateToggles(List<Player> playersToSelect)
    {
        foreach(Player currentPlayer in playersToSelect)
        {
            Transform user = Instantiate(userToggle);
            user.transform.SetParent(toggleHolder);
            user.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentPlayer.name;
            toggleables.Add((user.transform.GetChild(1).GetComponent<Toggle>(), currentPlayer.uuid));
        }
    }

    public List<string> GetSelectedPlayers()
    {
        List<string> returnList = new List<string>();
        foreach((Toggle t, string uuid) in toggleables)
        {
            if(t.isOn)
            {
                returnList.Add(uuid);
            }
        }
        return returnList;
    }

    void Cleanup()
    {
        selectedDeck = null;
        revealButton.onClick.RemoveAllListeners();
        foreach((Toggle t, string uuid) in toggleables)
        {
            Destroy(t.gameObject);
        }
        toggleables.Clear();
    }
}
