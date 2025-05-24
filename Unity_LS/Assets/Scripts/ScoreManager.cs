using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Configurações de Pontuação")]
    public int currentScore = 0;

    [System.Serializable]
    public class ScoreText
    {
        [Tooltip("Score text fields.")]
        public TextMeshProUGUI scoreText;
    }

    [Header("Configuração dos textos de Score")]
    [Tooltip("Opcional: UI Text para exibir a pontuação")]
    public List<ScoreText> scoreText = new List<ScoreText>();

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
            foreach (var item in scoreText)
            {
                item.scoreText.text = currentScore.ToString();
            }
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        foreach (var item in scoreText)
        {
            item.scoreText.text = currentScore.ToString();
        }
    }
}
