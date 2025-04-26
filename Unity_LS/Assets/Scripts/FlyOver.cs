using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.Events;

public class FlyOver : MonoBehaviour
{
    [Header("Virtual Camera")]
    [Tooltip("O CinemachineCamera que será movido")]
    public CinemachineCamera vcam;

    [System.Serializable]
    public struct Waypoint
    {
        [Tooltip("Transform de posição")]
        public Transform point;
        [Tooltip("Euler angles de rotação na chegada")]
        public Vector3 rotation;
        [Tooltip("Escala na chegada")]
        public Vector3 scale;
    }

    [Header("Waypoints de FlyOver")]
    [Tooltip("Lista de waypoints (posição, rotação e escala) em loop")]
    public List<Waypoint> waypoints = new List<Waypoint>();

    [Header("Segment Settings")]
    [Tooltip("Duração em segundos de cada transição entre waypoints")]
    public float segmentDuration = 5f;

    [Header("Posição Final do Jogo")]
    [Tooltip("Transform da posição final da câmera")]
    public Transform finalPoint;
    [Tooltip("Euler angles da rotação final")]
    public Vector3 finalRotation;
    [Tooltip("Escala final")]
    public Vector3 finalScale = Vector3.one;
    [Tooltip("Duração em segundos da transição final")]
    public float toGameDuration = 2f;

    [Header("Events")]
    [Tooltip("Disparado quando a transição final (ToTheGame) terminar")]
    public UnityEvent OnToTheGameComplete;

    int _currentIndex = 0;
    float _segmentTimer = 0f;
    bool _isFlying = true;
    bool _hasMovedToGame = false;

    void Start()
    {
        if (vcam == null)
            vcam = GetComponent<CinemachineCamera>();

        // comece no primeiro waypoint
        if (waypoints.Count > 0)
        {
            var wp = waypoints[0];
            vcam.transform.position = wp.point.position;
            vcam.transform.rotation = Quaternion.Euler(wp.rotation);
            vcam.transform.localScale = wp.scale;
        }
    }

    void Update()
    {
        if (_isFlying)
            UpdateFlyOver();
    }

    void UpdateFlyOver()
    {
        if (waypoints.Count < 2) return;

        _segmentTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_segmentTimer / segmentDuration);

        int nextIndex = (_currentIndex + 1) % waypoints.Count;
        var startWP = waypoints[_currentIndex];
        var endWP = waypoints[nextIndex];
        var T = vcam.transform;

        // Lerp posição, rotação e escala com mesmo t
        T.position = Vector3.Lerp(startWP.point.position, endWP.point.position, t);
        T.rotation = Quaternion.Slerp(Quaternion.Euler(startWP.rotation),
                                        Quaternion.Euler(endWP.rotation),
                                        t);
        T.localScale = Vector3.Lerp(startWP.scale, endWP.scale, t);

        if (_segmentTimer >= segmentDuration)
        {
            _segmentTimer = 0f;
            _currentIndex = nextIndex;
        }
    }

    /// <summary>
    /// Interrompe o loop e inicia a transição final para finalPoint.
    /// </summary>
    public void ToTheGame()
    {
        if (!_isFlying || _hasMovedToGame) return;
        _isFlying = false;
        StartCoroutine(MoveToFinalRoutine());
    }

    IEnumerator MoveToFinalRoutine()
    {
        var T = vcam.transform;
        Vector3 startP = T.position;
        Quaternion startR = T.rotation;
        Vector3 startS = T.localScale;

        Vector3 targetP = finalPoint.position;
        Quaternion targetR = Quaternion.Euler(finalRotation);
        Vector3 targetS = finalScale;

        float elapsed = 0f;
        while (elapsed < toGameDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / toGameDuration);

            T.position = Vector3.Lerp(startP, targetP, t);
            T.rotation = Quaternion.Slerp(startR, targetR, t);
            T.localScale = Vector3.Lerp(startS, targetS, t);

            yield return null;
        }

        // garante valores exatos no fim
        T.position = targetP;
        T.rotation = targetR;
        T.localScale = targetS;
        _hasMovedToGame = true;

        // dispara todos os listeners configurados no Inspector
        OnToTheGameComplete?.Invoke();
    }
}
