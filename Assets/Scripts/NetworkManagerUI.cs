using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;

    [Header("Panel")]
    public GameObject buttonPanel; // the panel holding the buttons

    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        HideButtons();
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        HideButtons();
    }

    void HideButtons()
    {
        if (buttonPanel != null)
            buttonPanel.SetActive(false);
    }
}