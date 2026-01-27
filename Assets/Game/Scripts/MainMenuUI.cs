using System;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private UgsRelayLobbyController controller;
    
    [Header("Host UI")]
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private TMP_Text lobbyCodeText;
    
    [Header("Join UI")]
    [SerializeField] private TMP_InputField joinCodeInput;
    
    [Header("Common UI")]
    [SerializeField] private TMP_Text statusText;

    private bool joining;

    private void Awake()
    {
        if(!controller) controller = FindFirstObjectByType<UgsRelayLobbyController>();
        if (!lobbyCodeText) lobbyCodeText.text = "Lobby Code: -";
        SetStatus("Ready");
    }

    public async void HostClicked()
    {
        try
        {
            SetStatus("Hosting...");
            string lobbyName = lobbyNameInput ?  lobbyNameInput.text : "My Lobby";

            string lobbyCode = await controller.HostAsync(lobbyName);
            
            if(lobbyCodeText) lobbyCodeText.text = $"Lobby Code: {lobbyCode}";
            SetStatus("Host started");
        }
        catch (System.Exception e)
        {
            SetStatus("Host failed " + e.Message);
            Debug.LogException(e);
        }
    }

    public async void JoinClicked()
    {
        if (joining) return;
        joining = true;

        try
        {
            string code = joinCodeInput ? joinCodeInput.text.Trim() : "";
            if (string.IsNullOrWhiteSpace(code))
            {
                SetStatus("Enter lobby code");
                return;
            }

            SetStatus("Joining...");
            await controller.JoinLobbyAsync(code);
            SetStatus("Joined");
        }
        catch (System.Exception e)
        {
            SetStatus("Join failed " + e.Message);
            Debug.LogException(e);
        }
        finally
        {
            joining = false;
        }
    }

    private void SetStatus(string status)
    {
        if(statusText) statusText.text = status;
        Debug.Log(status);
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
