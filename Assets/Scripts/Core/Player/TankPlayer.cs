using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SpriteRenderer minimapIcon;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    [SerializeField] private Color playerColor = Color.white;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes("Player"));

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;
    public override void OnNetworkSpawn()
    {
        Debug.Log($"TankPlayer spawned. IsOwner: {IsOwner}");

        if (IsServer)
        {
            //Always get userData for all players on the server, not just the Host
            UserData userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            if (userData != null) //Prevent potential NullReferenceException
            {
                playerName.Value = userData.userName;
            }
            else
            {
                Debug.LogWarning($"[SERVER] UserData is NULL for Client {OwnerClientId}");
            }

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            Debug.Log("Setting camera priority...");
            virtualCamera.Priority = cameraPriority;
            minimapIcon.color = playerColor;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        { 
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
