using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }
    public static Vector3 GetRandomSpawnPos()
    {
        if (spawnPoints.Count == 0)
        { 
            return Vector3.zero;
        }
        Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
}
