using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevealPlayerManager : MonoBehaviour
{
    [SerializeField]
    Toggle toggle;

    public TMP_InputField input;


    [SerializeField]
    Transform topBar;

    [SerializeField]
    Transform bottomBar;

    [HideInInspector]
    public Deck selectedDeck;

    List<(Toggle, string)> toggleables = new List<(Toggle, string)>();

    [SerializeField] Button revealButton;

    public void EnableBox(List<Player> playersToSelect, Deck? deck, bool enableInput)
    {
        Cleanup();
        input.gameObject.SetActive(enableInput);
        selectedDeck = deck;
        gameObject.SetActive(true);
        if(deck == null)
        {
            revealButton.onClick.AddListener(() => GameManager.Instance.SendRevealHand(this));
            revealButton.onClick.AddListener(() => GameManager.Instance._uiManager.Disable(this.transform));

        }
        else
        {
            revealButton.onClick.AddListener(() => GameManager.Instance.SendRevealedDeck(this, enableInput));
            // revealButton.onClick.AddListener(() => GameManager.Instance._uiManager.Disable(this.transform));
        }
        foreach(Player currentPlayer in playersToSelect)
        {
            Toggle newToggle = Instantiate(toggle);
            newToggle.transform.SetParent(transform);
            TextMeshProUGUI name = newToggle.transform.Find("name").GetComponent<TextMeshProUGUI>();
            name.text = currentPlayer.name;
            toggleables.Add((newToggle, currentPlayer.uuid));
        }
        if(enableInput)
        {
            input.transform.SetAsFirstSibling();
        }
        topBar.SetAsFirstSibling();
        bottomBar.SetAsLastSibling();
        transform.SetAsLastSibling();
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
