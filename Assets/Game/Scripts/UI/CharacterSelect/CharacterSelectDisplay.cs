using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private PlayerCard[] playerCards;

    [Header("UI")]
    [SerializeField] private Button selectInfectedButton;
    [SerializeField] private Button selectGuardButton;
    [SerializeField] private Button lockInButton;

    //[SerializeField] private GameObject characterInfoPanel;
    //[SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text joinCodeText;

    [Header("Database IDs")]
    [SerializeField] private int infectedCharacterId = 0;
    [SerializeField] private int guardCharacterId = 1;

    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            players.OnListChanged += HandlePlayersStateChanged;
            
            selectInfectedButton.onClick.AddListener(SelectInfected);
            selectGuardButton.onClick.AddListener(SelectGuard);
            lockInButton.onClick.AddListener(LockIn);

            //characterInfoPanel.SetActive(false);
            lockInButton.interactable = false;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost && joinCodeText != null)
        {
            joinCodeText.text = HostManager.Instance.JoinCode;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= HandlePlayersStateChanged;

            selectInfectedButton.onClick.RemoveListener(SelectInfected);
            selectGuardButton.onClick.RemoveListener(SelectGuard);
            lockInButton.onClick.RemoveListener(LockIn);
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != clientId) continue;
            players.RemoveAt(i);
            break;
        }
    }
    
    private void SelectInfected() => SelectById(infectedCharacterId);
    private void SelectGuard() => SelectById(guardCharacterId);
    
    public void Select(Character character)
    {
        if (character == null) return;
        SelectById(character.Id);
    }

    private void SelectById(int characterId)
    {
        var localId = NetworkManager.Singleton.LocalClientId;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != localId) continue;
            if (players[i].IsLockedIn) return;
            break;
        }

        if (!characterDatabase.IsValidCharacterId(characterId)) return;
        
        var character = characterDatabase.GetCharacterById(characterId);
        //characterNameText.text = character.DisplayName;
        //characterInfoPanel.SetActive(true);

        SelectServerRpc(characterId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        if (!characterDatabase.IsValidCharacterId(characterId)) return;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) continue;
            if (players[i].IsLockedIn) return;
            
            players[i] = new CharacterSelectState(players[i].ClientId, characterId, players[i].IsLockedIn);
            break;
        }
    }
    
    private void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) continue;

            if (!characterDatabase.IsValidCharacterId(players[i].CharacterId)) return;

            players[i] = new CharacterSelectState(players[i].ClientId, players[i].CharacterId, true);
            break;
        }
        
        foreach (var p in players)
        {
            if (!p.IsLockedIn) return;
        }

        foreach (var p in players)
        {
            HostManager.Instance.SetCharacter(p.ClientId, p.CharacterId);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i) playerCards[i].UpdateDisplay(players[i]);
            else playerCards[i].DisableDisplay();
        }
        
        var localId = NetworkManager.Singleton.LocalClientId;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != localId) continue;

            bool locked = players[i].IsLockedIn;
            bool hasSelection = characterDatabase.IsValidCharacterId(players[i].CharacterId);

            selectInfectedButton.interactable = !locked;
            selectGuardButton.interactable = !locked;
            lockInButton.interactable = !locked && hasSelection;

            break;
        }
    }
}
