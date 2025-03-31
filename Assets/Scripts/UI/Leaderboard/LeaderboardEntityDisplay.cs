using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColor;

    private FixedString32Bytes clientName;
    public ulong ClientId { get; private set; }
    public int ClientCoins { get; private set; }

    public void Initialise(ulong clientId, FixedString32Bytes name, int coins)
    {
        this.clientName = name;
        ClientId = clientId;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        { 
            displayText.color = myColor;
        }

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        ClientCoins = coins;
        UpdateText();
    }
    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {clientName} ({ClientCoins})";
    }
}
