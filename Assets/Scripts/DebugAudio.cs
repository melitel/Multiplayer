using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAudio : MonoBehaviour
{
    void Start()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true); // Include inactive objects
        Debug.Log($"[DEBUG] Audio Listeners in Scene: {listeners.Length}");

        foreach (AudioListener listener in listeners)
        {
            Debug.Log($"Audio Listener found on: {listener.gameObject.name}", listener.gameObject);
        }
    }
}
