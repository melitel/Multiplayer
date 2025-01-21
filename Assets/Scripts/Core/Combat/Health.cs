using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private bool isDead = false;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        currentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    { 
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healthValue)
    {
        ModifyHealth(healthValue);
    }

    private void ModifyHealth(int value)
    { 
        if(isDead) { return; }

        int newHealthValue = currentHealth.Value + value;
        currentHealth.Value = Mathf.Clamp(newHealthValue, 0, MaxHealth);

        if(currentHealth.Value == 0) 
        {
            OnDie?.Invoke(this);
            isDead = true;
        }
    }
}
