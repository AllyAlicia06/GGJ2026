using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartServer();
        }));
        hostButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartHost();
        }
        ));
        clientButton.onClick.AddListener((() =>
        {
            NetworkManager.Singleton.StartClient();
        }
        ));
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
