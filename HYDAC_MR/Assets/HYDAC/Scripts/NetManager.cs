using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Normal.Realtime;
using UnityEngine.Serialization;

public class NetManager : MonoBehaviour
{
    [SerializeField] private Realtime normRealtime;

    private RealtimeAvatarManager _normAvatarManager = null; 
    
    private void Awake()
    {
        normRealtime.didConnectToRoom += OnTryToConnect;
    }

    private void OnTryToConnect(Realtime realtime)
    {
        if (!realtime.connected)
        {
            Debug.Log("#NetManager#-------------------------OnTryConnect: Failed to connect");
            return;
        }
        
        Debug.Log("#NetManager#-------------------------OnTryConnect: Connected");

        _normAvatarManager = realtime.GetComponent<RealtimeAvatarManager>();
        _normAvatarManager.avatarCreated += OnAvatarCreated;

    }

    private void OnAvatarCreated(RealtimeAvatarManager avatarmanager, RealtimeAvatar avatar, bool islocalavatar)
    {
        if (islocalavatar)
        {
            Debug.Log("#NetManager#-------------------------OnAvatarCreated: Local avatar created");
        }
        else
        {
            Debug.Log("#NetManager#-------------------------OnAvatarCreated: Remote created");
            
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
