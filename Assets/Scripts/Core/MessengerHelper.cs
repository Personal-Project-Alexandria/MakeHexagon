using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MessengerHelper : MonoBehaviour
{
    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    public void OnLevelWasLoaded(int unused)
    {
        Messenger.Cleanup();
    }
}
