using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject canvasRoot;
    [SerializeField] private GameObject scoreboard;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TMP_InputField destCodeInput;
    [SerializeField] private TMP_Text myCodeText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private ushort startingPort = 7777;

    private bool isConnected = false;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        hostBtn.onClick.AddListener(() =>
        {
            string ip = GetLocalIP();
            ushort tryPort = startingPort;
            bool success = false;

            statusText.text = "Starting host..."; // Show a status message

            for (int i = 0; i < 100; i++)
            {
                if (IsPortAvailable(tryPort))
                {
                    var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                    transport.SetConnectionData(ip, tryPort);

                    if (NetworkManager.Singleton.StartHost())
                    {
                        success = true;
                        isConnected = true;
                        string myCode = ConnectionCodeManager.Encode(ip, tryPort);
                        myCodeText.text = myCode;
                        GUIUtility.systemCopyBuffer = myCode; // 🍓 copy to clipboard!
                        HideUI();
                        statusText.text = "Host started successfully!"; // Show success message
                        break;
                    }
                }
                tryPort++;
            }

            if (!success)
            {
                statusText.text = "Failed to start host!"; // Show failure message
                Debug.LogError("Couldn't find a free port to host on!");
            }
        });

        clientBtn.onClick.AddListener(() =>
        {
            string inputCode = destCodeInput.text;
            ConnectionCodeManager.Decode(inputCode, out string ip, out ushort decodedPort);

            statusText.text = "Attempting to connect..."; // Show status message

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, decodedPort);
            NetworkManager.Singleton.StartClient();

            // We wait a bit to check if the connection was successful
            // You may want to listen for the connection status here (or use events)
            Invoke("CheckConnection", 2.0f); // Call CheckConnection after a short delay

            // Hide the UI immediately after pressing connect
            HideUI();
        });

        exitBtn.onClick.AddListener(() =>
        {
            if (isConnected)
            {
                NetworkManager.Singleton.Shutdown();
                ScoreBoardManager.PlayerLeft(NetworkManager.Singleton.LocalClientId.ToString());
                isConnected = false;
                statusText.text = "Disconnected from the network.";
            }
            ShowUI();
        });

        ShowUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from network events
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        ScoreBoardManager.PlayerJoined(clientId.ToString());
    }

    private void OnClientDisconnected(ulong clientId)
    {
        ScoreBoardManager.PlayerLeft(clientId.ToString());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle UI visibility when pressing ESC
            bool visible = canvasRoot.activeSelf;
            if (visible)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
            bool scoreboardVisible = scoreboard.activeSelf;
            if (scoreboardVisible)
            {
                scoreboard.SetActive(false);
            }
            else
            {
                scoreboard.SetActive(true);
            }
        }
    }

    private void ShowUI()
    {
        canvasRoot.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideUI()
    {
        canvasRoot.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CheckConnection()
    {
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
        {
            statusText.text = "Connected to the server!"; // Success message
        }
        else
        {
            statusText.text = "Failed to connect to server."; // Failure message
        }
    }

    private string GetLocalIP()
    {
        string localIP = "127.0.0.1";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();
        }
        return localIP;
    }

    private bool IsPortAvailable(int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.Stop();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
