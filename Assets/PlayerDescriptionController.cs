using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PlayerDescriptionController : MonoBehaviour
{
    [SerializeField]
    PlayerComponents clientPlayer;
    [SerializeField]
    PlayerComponents enemyPlayer;

    public void SetName(string name, bool client)
    {
        if(client)
        {
            clientPlayer.name.text = name;
            return;
        }
        enemyPlayer.name.text = name;

    }

    public void SetHealth(int health, bool client)
    {
        if(client)
        {
            clientPlayer.healthInput.text = health.ToString();
            return;
        }
        enemyPlayer.healthInput.text = health.ToString();
    }

    public void UpdateHealthBars()
    {
        clientPlayer.healthInput.text = GameManager.Instance.clientPlayer.health.ToString();
        clientPlayer.name.text = GameManager.Instance.clientPlayer.name;
        Player currentPlayer = GameManager.Instance.boardMovement.GetCurrentPlayer();
        enemyPlayer.healthInput.text = currentPlayer.health.ToString();
        enemyPlayer.name.text = currentPlayer.name;
    }

    public void IncreaseClientHealth()
    {
        // int newHealth = HelperFunctions.ChangeIntputField(clientPlayer.healthInput.text, true);
        GameManager.Instance.clientPlayer.health += 1;
        GameManager.Instance.SendChangeHealth();
    }

    public void ChangeClientHealth(TMP_InputField tmpInput)
    {
        int newHealth;
        if (!int.TryParse(tmpInput.text, out newHealth))
        {
            newHealth = 0;
        }

        GameManager.Instance.clientPlayer.health = newHealth;
        GameManager.Instance.SendChangeHealth();
    }
    public void DecreaseClientHealth()
    {
        // int newHealth = HelperFunctions.ChangeIntputField(clientPlayer.healthInput.text, false);
        GameManager.Instance.clientPlayer.health -= 1;
        GameManager.Instance.SendChangeHealth();
    }
}