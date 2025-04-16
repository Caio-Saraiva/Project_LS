using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Configura��es de Pontua��o")]
    public int currentScore = 0;

    [Tooltip("Opcional: UI Text para exibir a pontua��o")]
    public TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        DestructibleItem.OnItemDestroyed += AddScore;
    }

    private void OnDisable()
    {
        DestructibleItem.OnItemDestroyed -= AddScore;
    }

    private void AddScore(int points)
    {
        currentScore += points;
        Debug.Log("Pontos adicionados: " + points + ". Total: " + currentScore);
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
}
