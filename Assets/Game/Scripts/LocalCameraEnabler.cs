using System;
using Unity.Netcode;
using UnityEngine;

public class LocalCameraEnabler : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener listener;

    private void Awake()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>(true);
        if(listener == null) listener = GetComponentInChildren<AudioListener>(true);
    }

    public override void OnNetworkSpawn()
    {
        bool local = IsOwner;
        
        if(cam != null) cam.enabled = local;
        if(listener != null) listener.enabled = local;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
