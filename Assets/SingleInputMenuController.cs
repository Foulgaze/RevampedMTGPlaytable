using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleInputMenuController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI windowName;
    public Button submitButton;
    public TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI submitButtonText;

    public void InitMenu(string windowName, string buttonText, Action action)
    {
        submitButton.onClick.RemoveAllListeners();
        submitButtonText.text = buttonText;
        this.windowName.text = windowName;
        inputField.text = "";

        submitButton.onClick.AddListener(() => {GameManager.Instance._uiManager.Disable(transform);});
        submitButton.onClick.AddListener(new UnityEngine.Events.UnityAction(action));

        gameObject.SetActive(true);
        transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
    }
}
