using UnityEngine;
using UnityEngine.Events;
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

    [Header("Events")]
    [Tooltip("Disparado quando o timer é iniciado")]
    public UnityEvent OnTimerStart;
    [Tooltip("Disparado quando o timer chega a zero")]
    public UnityEvent OnEnd;

    // estado interno
    private float remainingTime;
    private bool spawnRateUpdated = false;
    private bool isRunning = false;
    public static bool isGameOver = false;

    void Start()
    {
        // Inicializamos o UI mostrando o tempo total, mas sem iniciar a contagem
        remainingTime = totalGameTime;
        spawnRateUpdated = false;
        isGameOver = false;
        isRunning = false;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning)
            return;

        // Se terminou...
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isGameOver = true;
            isRunning = false;

            if (itemSpawner != null)
            {
                itemSpawner.enabled = false;
                Debug.Log("Tempo esgotado! Spawner desativado.");
            }

            UpdateTimerUI();
            OnEnd?.Invoke();
            return;
        }

        // Continua contando
        remainingTime -= Time.deltaTime;

        // Acelera spawn ao atingir threshold
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
        if (timerText == null) return;

        int totalSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Inicia (ou reinicia) a contagem, reseta flags e dispara OnTimerStart.
    /// </summary>
    public void StartTimer()
    {
        remainingTime = totalGameTime;
        spawnRateUpdated = false;
        isGameOver = false;
        isRunning = true;

        if (itemSpawner != null)
            itemSpawner.enabled = true;

        UpdateTimerUI();
        OnTimerStart?.Invoke();
    }
}
