using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevealLibraryXCards : MonoBehaviour
{
    [SerializeField]
    Toggle toggle;


    [SerializeField]
    Transform topBar;

    [SerializeField]
    Transform bottomBar;

    [HideInInspector]
    public Deck selectedDeck;

    List<(Toggle, string)> toggleables = new List<(Toggle, string)>();

    public void EnableBox(List<Player> playersToSelect, Deck deck)
    {
        Cleanup();
        selectedDeck = deck;
        gameObject.SetActive(true);
        foreach(Player currentPlayer in playersToSelect)
        {
            Toggle newToggle = Instantiate(toggle);
            newToggle.transform.SetParent(transform);
            TextMeshProUGUI name = newToggle.transform.Find("name").GetComponent<TextMeshProUGUI>();
            name.text = currentPlayer.name;
            toggleables.Add((newToggle, currentPlayer.uuid));
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
        foreach((Toggle t, string uuid) in toggleables)
        {
            Destroy(t);
        }
        toggleables.Clear();
    }
}
