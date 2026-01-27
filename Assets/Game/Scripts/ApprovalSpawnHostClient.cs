using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

public class ApprovalSpawnHostClient : MonoBehaviour
{
    [SerializeField] private NetworkObject hostPlayerPrefab;
    [SerializeField] private NetworkObject clientPlayerPrefab;

    private void Awake()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.ConnectionApprovalCallback -= Approval;
    }

    private void Approval(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = false;
        
        //bool isHostClient = (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId);
        bool isHostClient = request.ClientNetworkId == NetworkManager.ServerClientId;

        NetworkObject prefab = isHostClient ? hostPlayerPrefab : clientPlayerPrefab;
        
        var player = Instantiate(prefab);
        player.SpawnAsPlayerObject(request.ClientNetworkId, destroyWithScene: true);
        
        Debug.Log($"[Approval] request from clientId={request.ClientNetworkId}, approved={response.Approved}");
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
