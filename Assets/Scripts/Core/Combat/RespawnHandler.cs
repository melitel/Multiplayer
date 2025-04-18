using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage = 50;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    { 
        int coinsKept = (int)(player.Wallet.TotalCoins.Value * (keptCoinPercentage / 100));
        Debug.Log($"[RespawnHandler] Player {player.OwnerClientId} died. Keeping {coinsKept} coins.");
        
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, coinsKept));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int coinsKept)
    {
        yield return new WaitForSeconds(1f);


        TankPlayer playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        playerInstance.Wallet.TotalCoins.Value += coinsKept;
    }
}
