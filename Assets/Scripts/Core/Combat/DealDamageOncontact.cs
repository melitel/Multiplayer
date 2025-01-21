using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOncontact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;
    public void SetOwner(ulong ownerClientId)
    { 
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) { return; }

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        { 
            if (netObj.OwnerClientId == ownerClientId) {return; }
        }

        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
