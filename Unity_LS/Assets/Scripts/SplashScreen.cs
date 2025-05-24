using UnityEngine;
using TMPro;

public class SplashScreen : MonoBehaviour
{
    [Header("Splash Text")]
    [Tooltip("O TextMeshProUGUI que sofrerá a intermitência")]
    public TextMeshProUGUI targetText;

    [Header("Intermitência")]
    [Tooltip("Intervalo em segundos entre cada toggle/fade")]
    [Range(0f, 5f)]
    public float interval = 1f;

    [Tooltip("Se marcado, alterna instantaneamente entre alpha=0 e alpha=255 a cada intervalo;\n" +
             "se desmarcado, faz fade in/out suave usando Lerp")]
    public bool clampMode = false;

    // estado interno
    float _timer;
    bool _isOpaque;

    void Start()
    {
        if (targetText != null)
            SetAlpha(0);     // começa transparente
    }

    void Update()
    {
        if (targetText == null || interval <= 0f)
            return;

        _timer += Time.deltaTime;

        if (clampMode)
        {
            // a cada 'interval' segundos, alterna entre visível/invisível
            if (_timer >= interval)
            {
                _timer -= interval;
                _isOpaque = !_isOpaque;
                SetAlpha(_isOpaque ? 255 : 0);
            }
        }
        else
        {
            // fade in/out: ping-pong em [0..interval], normalizado para [0..1], depois *255
            float t = Mathf.PingPong(_timer, interval) / interval;
            SetAlpha(Mathf.RoundToInt(t * 255f));
        }
    }

    /// <summary>
    /// Ajusta apenas o canal alpha do targetText (0 a 255).
    /// </summary>
    void SetAlpha(int alpha)
    {
        var c = targetText.color;
        c.a = alpha / 255f;
        targetText.color = c;
    }
}
