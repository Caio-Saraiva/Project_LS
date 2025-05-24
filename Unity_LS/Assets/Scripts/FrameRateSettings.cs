using System.Collections;
using UnityEngine;
using TMPro;

public class FrameRateSettings : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [Tooltip("Cap the engine’s target frame rate (vSync is disabled)")]
    [Range(0, 240)]
    public int maxRate = 90;

    [Tooltip("If > 0, manually limit the frame rate to this value")]
    [Range(0, 240)]
    public int targetFrameRate = 90;

    [Header("FPS Display")]
    [Tooltip("UI Text to show the measured FPS")]
    public TextMeshProUGUI fpsText;

    [Tooltip("Seconds between FPS updates")]
    public float fpsUpdateInterval = 1f;

    int _frameCount;
    float _fpsTimer;

    void Awake()
    {
        // disable vsync so Application.targetFrameRate is respected
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxRate;
    }

    void OnValidate()
    {
        // if you change it in the Inspector, apply immediately
        Application.targetFrameRate = maxRate;
    }

    void Start()
    {
        if (targetFrameRate > 0)
            StartCoroutine(FrameLimiter());

        if (fpsText != null)
            StartCoroutine(FPSCounter());
    }

    void Update()
    {
        // count frames for FPS display
        _frameCount++;
    }

    IEnumerator FrameLimiter()
    {
        // simple coroutine to avoid running faster than targetFrameRate
        while (true)
        {
            yield return new WaitForEndOfFrame();
            float wait = (1f / targetFrameRate) - Time.deltaTime;
            if (wait > 0f)
                yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator FPSCounter()
    {
        while (true)
        {
            // wait interval
            yield return new WaitForSeconds(fpsUpdateInterval);
            // calculate and display
            float fps = _frameCount / fpsUpdateInterval;
            fpsText.text = Mathf.RoundToInt(fps).ToString();
            // reset counter
            _frameCount = 0;
        }
    }
}
