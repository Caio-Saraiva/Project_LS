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

    [Header("Blink Visual")]
    [Tooltip("Cor padrão do texto do timer.")]
    public Color defaultColor = Color.white;
    [Tooltip("Cor alternativa para piscar.")]
    public Color blinkColor = Color.red;
    [Tooltip("Quando o tempo restante for igual ou menor que este valor (s), começa o blink.")]
    public float blinkStartThreshold = 15f;
    [Tooltip("Intervalo em segundos entre cada troca de cor.")]
    public float blinkInterval = 0.5f;

    [Header("Events")]
    [Tooltip("Disparado quando o timer é iniciado")]
    public UnityEvent OnTimerStart;
    [Tooltip("Disparado quando o timer chega a zero")]
    public UnityEvent OnEnd;

    // estado interno do timer
    float remainingTime;
    bool spawnRateUpdated = false;
    bool isRunning = false;
    public static bool isGameOver = false;

    // blink internals
    bool blinkActive = false;
    float blinkTimer = 0f;
    bool blinkStateOn = false;

    // modo destrutível manual
    bool destructibleActive = false;

    void Start()
    {
        // inicializa tudo parado
        remainingTime = totalGameTime;
        spawnRateUpdated = false;
        isGameOver = false;
        isRunning = false;

        blinkActive = false;
        blinkTimer = 0f;
        blinkStateOn = false;
        if (timerText != null)
            timerText.color = defaultColor;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isRunning)
            return;

        if (remainingTime <= 0f)
        {
            // fim do tempo
            remainingTime = 0f;
            isRunning = false;
            isGameOver = true;
            if (itemSpawner != null) itemSpawner.enabled = false;
            UpdateTimerUI();

            // desativa destrutível
            DisableDestructible();
            OnEnd?.Invoke();
            return;
        }

        remainingTime -= Time.deltaTime;

        // acelera spawn
        if (itemSpawner != null && !spawnRateUpdated && remainingTime <= spawnSpeedupThreshold)
        {
            itemSpawner.spawnInterval = newSpawnInterval;
            spawnRateUpdated = true;
        }

        UpdateTimerUI();
        HandleBlink();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;
        int totalSeconds = Mathf.CeilToInt(remainingTime);
        timerText.text = string.Format("{0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
    }

    void HandleBlink()
    {
        if (timerText == null) return;
        if (!blinkActive && remainingTime <= blinkStartThreshold)
        {
            blinkActive = true;
            blinkTimer = 0f;
            blinkStateOn = false;
        }

        if (blinkActive)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                blinkTimer = 0f;
                blinkStateOn = !blinkStateOn;
                timerText.color = blinkStateOn ? blinkColor : defaultColor;
            }
        }
    }

    /// <summary>
    /// Inicia (ou reinicia) o timer e ativa o modo destrutível.
    /// </summary>
    public void StartTimer()
    {
        remainingTime = totalGameTime;
        spawnRateUpdated = false;
        isGameOver = false;
        isRunning = true;
        blinkActive = false;
        blinkTimer = 0f;
        blinkStateOn = false;
        if (timerText != null) timerText.color = defaultColor;
        if (itemSpawner != null) itemSpawner.enabled = true;

        // ativa destrutível no reset
        EnableDestructible();

        OnTimerStart?.Invoke();
        UpdateTimerUI();
    }

    /// <summary>
    /// Ativa o modo destrutível globalmente (DestructibleItem).
    /// </summary>
    public void EnableDestructible()
    {
        destructibleActive = true;
        DestructibleItem.SetDestructibleEnabled(true);
    }

    /// <summary>
    /// Desativa o modo destrutível globalmente.
    /// </summary>
    public void DisableDestructible()
    {
        destructibleActive = false;
        DestructibleItem.SetDestructibleEnabled(false);
    }

    /// <summary>
    /// Inverte o estado do modo destrutível globalmente.
    /// </summary>
    public void ToggleDestructible()
    {
        destructibleActive = !destructibleActive;
        DestructibleItem.SetDestructibleEnabled(destructibleActive);
    }
}
