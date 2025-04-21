using UnityEngine;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText, scoreText;

    public void Initialize(string name)
    {
        if (nameText != null)
            nameText.text = name;
        else
            Debug.LogError("NameText is not assigned in PlayerCard.");

        ChangeScore(0);
    }

    public void ChangeScore(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
        else
            Debug.LogError("ScoreText is not assigned in PlayerCard.");
    }
}