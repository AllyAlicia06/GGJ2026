using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDatabase characterDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        foreach (var client in HostManager.Instance.ClientData)
        {
            ulong clientId = client.Key;
            int characterId = client.Value.characterId;
            
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                var characterInstance = Instantiate(character.Prefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(clientId);
                
                //added for the ui and everything
                var pc = characterInstance.GetComponent<PlayerCharacter>();
                if (pc != null)
                    pc.CharacterId.Value = client.Value.characterId;
            }
        }
    }
}
