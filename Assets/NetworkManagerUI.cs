using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]private Button serverButn;
    [SerializeField]private Button hostButn;
    [SerializeField]private Button clientButn;

    private void Awake()
    {
        serverButn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        hostButn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        clientButn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
