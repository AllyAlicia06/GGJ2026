using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    public NetworkVariable<int> CharacterId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void SetCharacterId(int characterId)
    {
        if (IsServer) return;
        CharacterId.Value = characterId;
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
