using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    [Tooltip("Duração total do jogo, em segundos.")]
    public float totalGameTime = 120f;

    [Tooltip("Quando o tempo restante chegar a esse valor, o spawn ficará mais rápido.")]
    public float spawnSpeedupThreshold = 30f;

    [Tooltip("Novo intervalo (em segundos) para o spawn de itens quando atingir o threshold.")]
    public float newSpawnInterval = 1f;

    [Header("Referências")]
    [Tooltip("Texto do TMP que exibirá o tempo restante.")]
    public TextMeshProUGUI timerText;

    [Tooltip("Referência ao script ItemSpawner que gerencia o spawn dos itens.")]
    public ItemSpawner itemSpawner;

    private float remainingTime;
    private bool spawnRateUpdated = false;

    public static bool isGameOver = false;

    void Start()
    {
        remainingTime = totalGameTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            // Aqui definimos que o jogo acabou.
            isGameOver = true;
            if (itemSpawner != null)
            {
                itemSpawner.enabled = false;
                Debug.Log("Tempo esgotado! Spawner desativado.");
            }
            UpdateTimerUI();
            return;
        }

        remainingTime -= Time.deltaTime;

        if (itemSpawner != null && !spawnRateUpdated && remainingTime <= spawnSpeedupThreshold)
        {
            itemSpawner.spawnInterval = newSpawnInterval;
            spawnRateUpdated = true;
            Debug.Log("Spawn interval atualizado para " + newSpawnInterval + " segundos.");
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
