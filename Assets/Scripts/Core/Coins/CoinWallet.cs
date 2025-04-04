using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        health.OnDie -= HandleDie;
    }
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

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue) { return; }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(coinPrefab,
            GetSpawnPoint(),
            Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    private Vector2 GetSpawnPoint()
    {       
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
