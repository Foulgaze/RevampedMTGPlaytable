using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject boxHolder;
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

    [SerializeField]
    Transform rightClickMenu;

    [SerializeField]
    Transform drawCardBox;

    [SerializeField]
    public Transform libraryHolder;
    

    public Deck currentSelectedDeck;


    bool ready = false;


    
    void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && rightClickMenu.gameObject.activeSelf) // Technically should cache rt perhaps and cam main
        {
            if(!RectTransformUtility.RectangleContainsScreenPoint(rightClickMenu.GetComponent<RectTransform>(), Input.mousePosition))
            {
                rightClickMenu.gameObject.SetActive(false);
            }
        }
    }
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


    public void EnableDrawCardsBox()
    {
        GameManager.Instance.gameInteractable = false;
        drawCardBox.gameObject.SetActive(true);
    }

    public void DisableDrawCardsBox()
    {
        GameManager.Instance.gameInteractable = true;
        drawCardBox.gameObject.SetActive(false);
    }

    public void EnableRightClickMenu(Deck deck)
    {
        currentSelectedDeck = deck;
        rightClickMenu.gameObject.SetActive(true);
        RectTransform menu = rightClickMenu.GetComponent<RectTransform>();
        menu.position = Input.mousePosition + new Vector3(menu.sizeDelta.x/2,menu.sizeDelta.y/2 ,0);
    }

    public void DisableRightClickMenu()
    {
        rightClickMenu.gameObject.SetActive(false);
    }

    public Transform ShowLibrary()
    {
        libraryHolder.gameObject.SetActive(true);
        return libraryHolder;
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
