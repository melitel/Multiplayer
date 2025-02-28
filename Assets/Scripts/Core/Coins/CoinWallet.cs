using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    private void Start()
    {
        TotalCoins.OnValueChanged += OnCoinsUpdated;
    }

    private void OnCoinsUpdated(int previousValue, int newValue)
    {
        Debug.Log($"Coins updated! Previous: {previousValue}, New: {newValue}");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.TryGetComponent<RespawningCoin>(out RespawningCoin coin)) return;

        // Tell the server to collect the coin
        CollectCoinServerRpc(coin.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void CollectCoinServerRpc(NetworkObjectReference coinReference)
    {
        if (!coinReference.TryGet(out NetworkObject coinObject)) return;
        RespawningCoin coin = coinObject.GetComponent<RespawningCoin>();
        if (coin == null) return;

        int coinValue = coin.Collect();
        TotalCoins.Value += coinValue;
    }

    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }
}
