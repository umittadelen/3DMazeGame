using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    private static ScoreBoardManager instance;

    [SerializeField] private GameObject scoreboard;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerCardParent;

    private Dictionary<string, PlayerCard> _playerCards = new Dictionary<string, PlayerCard>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        scoreboard.SetActive(false); // Hide scoreboard by default
    }

    public static void PlayerJoined(string clientID)
    {
        if (instance == null)
        {
            Debug.LogError("ScoreBoardManager instance is not initialized.");
            return;
        }

        if (instance.playerCardPrefab == null || instance.playerCardParent == null)
        {
            Debug.LogError("PlayerCardPrefab or PlayerCardParent is not assigned in ScoreBoardManager.");
            return;
        }

        if (!instance._playerCards.ContainsKey(clientID))
        {
            PlayerCard newPlayerCard = Instantiate(instance.playerCardPrefab, instance.playerCardParent);
            instance._playerCards.Add(clientID, newPlayerCard);
            newPlayerCard.Initialize(clientID);
        }
    }

    public static void PlayerLeft(string clientID)
    {
        if (instance == null)
        {
            Debug.LogError("ScoreBoardManager instance is not initialized.");
            return;
        }

        if (instance._playerCards.TryGetValue(clientID, out PlayerCard playerCard))
        {
            Destroy(playerCard.gameObject);
            instance._playerCards.Remove(clientID);
        }
    }

    private void Update()
    {
        if (scoreboard == null)
        {
            Debug.LogError("Scoreboard is not assigned in ScoreBoardManager.");
            return;
        }

        scoreboard.SetActive(Input.GetKey(KeyCode.Tab));
    }

    public static void UpdatePlayerScore(string clientID, int score)
    {
        if (instance == null)
        {
            Debug.LogError("ScoreBoardManager instance is not initialized.");
            return;
        }

        instance.UpdatePlayerScoreServerRpc(clientID, score);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerScoreServerRpc(string clientID, int score)
    {
        if (_playerCards.ContainsKey(clientID))
        {
            _playerCards[clientID].ChangeScore(score);
            UpdatePlayerScoreClientRpc(clientID, score);
        }
        else
        {
            Debug.LogWarning($"ClientID {clientID} not found in _playerCards.");
        }
    }

    [ClientRpc]
    private void UpdatePlayerScoreClientRpc(string clientID, int score)
    {
        if (_playerCards.ContainsKey(clientID))
        {
            _playerCards[clientID].ChangeScore(score);
        }
        else
        {
            Debug.LogWarning($"ClientID {clientID} not found in _playerCards.");
        }
    }
}