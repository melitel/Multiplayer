using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Settings")]
    [SerializeField] private int cameraPriority = 15;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        { 
            virtualCamera.Priority = cameraPriority;
        }
    }
}
