using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Menu Panel")]
    public GameObject menuPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Button player1Button;
    public Button player2Button;

    [Header("Waiting Panel")]
    public GameObject waitingPanel;
    public TextMeshProUGUI waitingText;

    void Start()
    {
        player1Button.onClick.AddListener(SelectPlayer1);
        player2Button.onClick.AddListener(SelectPlayer2);

        if (menuPanel != null) menuPanel.SetActive(true);
        if (waitingPanel != null) waitingPanel.SetActive(false);

        if (titleText != null) titleText.text = "PONG";
        if (subtitleText != null) subtitleText.text = "Select Your Player";
    }

    void SelectPlayer1()
    {
        NetworkManager.Singleton.StartHost();

        if (menuPanel != null) menuPanel.SetActive(false);
        if (waitingPanel != null) waitingPanel.SetActive(true);
        if (waitingText != null) waitingText.text = "Waiting for Player 2...";
    }

    void SelectPlayer2()
    {
        NetworkManager.Singleton.StartClient();

        if (menuPanel != null) menuPanel.SetActive(false);
        if (waitingPanel != null) waitingPanel.SetActive(true);
        if (waitingText != null) waitingText.text = "Connecting to Player 1...";
    }
}