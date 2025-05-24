using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    [Header("Timings")]
    [Tooltip("Tempo de fade in em segundos.")]
    public float fadeInDuration = 0.2f;
    [Tooltip("Tempo estático em segundos.")]
    public float stayDuration = 0.5f;
    [Tooltip("Tempo de fade out em segundos.")]
    public float fadeOutDuration = 0.5f;

    [Header("Motion")]
    [Tooltip("Unidades que o texto sobe durante toda a animação.")]
    public float riseDistance = 1f;

    [Header("Orientation")]
    [Tooltip("Rotação extra em Euler para ajustar como o texto encara a câmera.")]
    public Vector3 rotationOffset = Vector3.zero;

    private CanvasGroup canvasGroup;
    private TMP_Text tmpText;
    private Vector3 startPos;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        tmpText = GetComponentInChildren<TMP_Text>();
        if (tmpText == null) Debug.LogError("FloatingTextUI precisa de um TMP_Text filho.");

        canvasGroup.alpha = 0f;
        startPos = transform.position;
    }

    /// <summary>
    /// Use este método imediatamente após Instanciar o prefab.
    /// </summary>
    public void Show(int value)
    {
        tmpText.text = "+" + value.ToString();
        StartCoroutine(Lifecycle());
    }

    private IEnumerator Lifecycle()
    {
        float elapsed = 0f;
        float total = fadeInDuration + stayDuration + fadeOutDuration;

        // Fade In + rise
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            canvasGroup.alpha = t;
            transform.position = startPos + Vector3.up * (riseDistance * (elapsed / total));
            FaceCamera();
            yield return null;
        }

        // Stay
        while (elapsed < fadeInDuration + stayDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = startPos + Vector3.up * (riseDistance * (elapsed / total));
            FaceCamera();
            yield return null;
        }

        // Fade Out
        float fadeStart = fadeInDuration + stayDuration;
        while (elapsed < total)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01((elapsed - fadeStart) / fadeOutDuration);
            canvasGroup.alpha = 1f - t;
            transform.position = startPos + Vector3.up * (riseDistance * (elapsed / total));
            FaceCamera();
            yield return null;
        }

        Destroy(gameObject);
    }

    private void FaceCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position)
                             * Quaternion.Euler(rotationOffset);
    }
}
