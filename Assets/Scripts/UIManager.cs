using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject connectMenu;
    [SerializeField]
    GameObject deckLoadMenu;

    [SerializeField]
    GameObject errorMessage;
    [SerializeField]
    TextMeshProUGUI errorText;
    [SerializeField]
    TextMeshProUGUI readyUpText;
    [SerializeField]
    TMP_InputField deckLoad;

    bool ready = false;


    

    void Awake()
    {
        connectMenu.SetActive(false);
        deckLoadMenu.SetActive(false);
        errorMessage.SetActive(false);
    }

    public void SwitchToConnect()
    {
        connectMenu.SetActive(true);
    }

    public void SwitchToDeckLoad()
    {
        connectMenu.SetActive(false);
        deckLoadMenu.SetActive(true);
    }

    public void SwitchToStartGame()
    {
        deckLoadMenu.SetActive(false);
    }

    public void DisplayErrorMessage(string newMessage, int timer)
    {
        errorText.text = newMessage;
        StartCoroutine(DisplayErrorMessageForSeconds(timer));
    }
    
    public void ChangeReadyStatus()
    {
        ready = !ready;

        if(ready)
        {
            readyUpText.text = "Unready";
            deckLoad.interactable = false;
            return;
        }
        readyUpText.text = "Ready";
        deckLoad.interactable = true;

    }

    private IEnumerator DisplayErrorMessageForSeconds(float enableTime)
    {
        
        errorMessage.SetActive(true);
        errorMessage.transform.SetAsLastSibling();
        yield return new WaitForSeconds(enableTime);

        errorMessage.SetActive(false);
        
    }

    public void HandleReadyButton(TMP_InputField textMeshProText)
    {
        if(!ready)
        {
            if(GameManager.Instance.VerifySubmittedDeck(textMeshProText))
            {
                ChangeReadyStatus();
            }
            return;
        }
        GameManager.Instance.UnReady();
        ChangeReadyStatus();
    }

    
}
