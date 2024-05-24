using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevealPlayerManager : MonoBehaviour
{
    [SerializeField] Transform userTogglePrefab;
    [SerializeField] Transform toggleHolder;
    
    List<(Toggle, string)> toggleables = new List<(Toggle, string)>();

    [SerializeField] Button confirmButton;
    public TMP_InputField cardCountInput;

    public bool hasCardCount;

    public void InitMenu(List<Player> playersToSelect, Action onConfirm, bool passToCardCountWindow = false, bool exclusiveToggle = false)
    {
        Cleanup();
        gameObject.SetActive(true);
        hasCardCount = passToCardCountWindow;
        CreateToggles(playersToSelect, exclusiveToggle);
        confirmButton.onClick.AddListener(() => onConfirm());
        confirmButton.onClick.AddListener(() => {GameManager.Instance._uiManager.Disable(this.transform);});

        transform.SetAsLastSibling();
    }

    void CreateToggles(List<Player> playersToSelect, bool exclusiveToggle)
    {
        foreach(Player currentPlayer in playersToSelect)
        {
            Transform user = Instantiate(userTogglePrefab);
            user.SetParent(toggleHolder);
            user.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentPlayer.name;
            Toggle toggle = user.transform.GetChild(1).GetComponent<Toggle>();
            if(exclusiveToggle)
            {
                toggle.onValueChanged.AddListener(delegate {UnToggleOthers(toggle);});
            }
            toggleables.Add((toggle, currentPlayer.uuid));
        }
    }

    void UnToggleOthers(Toggle toggle)
    {
        foreach((Toggle t, string uuid) in toggleables)
        {
            if(t != toggle)
            {
                t.isOn = false;
            }
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
        confirmButton.onClick.RemoveAllListeners();
        foreach((Toggle t, string uuid) in toggleables)
        {
            Destroy(t.transform.parent.gameObject);
        }
        toggleables.Clear();
    }
}
