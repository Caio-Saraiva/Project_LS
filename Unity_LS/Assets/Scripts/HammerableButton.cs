using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HammerableButton : MonoBehaviour
{
    [Header("Modelos 3D")]
    public GameObject originalModelPrefab;

    [Header("Prefab Instantiation Offsets")]
    [Tooltip("Deslocamento de posição (local) ao instanciar o modelo")]
    public Vector3 modelPositionOffset = Vector3.zero;
    [Tooltip("Deslocamento de rotação (Euler, local) ao instanciar o modelo")]
    public Vector3 modelRotationOffset = Vector3.zero;
    [Tooltip("Multiplicador de escala ao instanciar o modelo")]
    public Vector3 modelScaleOffset = Vector3.one;

    [Header("Efeitos & Áudio")]
    public GameObject hitEffectPrefab;
    public List<AudioClip> hitSounds;

    [Header("Tags de Colisão")]
    public string hammerTag = "Hammer";

    [Header("Feedback de Escala ao Hit")]
    [Range(0.1f, 1f)]
    [Tooltip("Escala relativa ao original quando pressionado (1 = sem alteração)")]
    public float collisionSize = 0.85f;
    [Tooltip("Tempo total de ida e volta do squish")]
    public float collisionSizeTime = 0.2f;
    public bool squishX = true;
    public bool squishY = true;
    public bool squishZ = true;

    [Header("Events")]
    [Tooltip("Disparado quando o martelo encosta")]
    public UnityEvent OnPressed;
    [Tooltip("Disparado quando o martelo sai")]
    public UnityEvent OnReleased;

    // estado interno
    GameObject currentModelInstance;
    Vector3 _modelInitialScale;
    bool _isPressed = false;

    void Start()
    {
        SpawnInitialModel();

        if (currentModelInstance != null)
            _modelInitialScale = currentModelInstance.transform.localScale;
        else
            Debug.LogWarning("HammerableButton: modelo não instanciado.");
    }

    void OnEnable()
    {
        // sempre que o GameObject for reativado:
        _isPressed = false;

        // e garante que o modelo volte à escala original
        if (currentModelInstance != null)
            currentModelInstance.transform.localScale = _modelInitialScale;
    }

    void SpawnInitialModel()
    {
        if (originalModelPrefab == null) return;

        Vector3 spawnPos = transform.TransformPoint(modelPositionOffset);
        Quaternion spawnRot = transform.rotation * Quaternion.Euler(modelRotationOffset);

        currentModelInstance = Instantiate(
            originalModelPrefab,
            spawnPos,
            spawnRot,
            transform
        );

        // aplica multiplicador de escala
        var baseScale = currentModelInstance.transform.localScale;
        currentModelInstance.transform.localScale = new Vector3(
            baseScale.x * modelScaleOffset.x,
            baseScale.y * modelScaleOffset.y,
            baseScale.z * modelScaleOffset.z
        );
    }

    public void ReceiveHit()
    {
        if (_isPressed) return;
        _isPressed = true;

        // som de hit
        if (hitSounds != null && hitSounds.Count > 0)
            AudioSource.PlayClipAtPoint(
                hitSounds[Random.Range(0, hitSounds.Count)],
                transform.position
            );

        // efeito de hit
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // pulso suave de escala no modelo
        if (currentModelInstance != null)
            StartCoroutine(ScalePulse());

        // dispara evento
        OnPressed?.Invoke();
    }

    IEnumerator ScalePulse()
    {
        float half = collisionSizeTime * 0.5f;
        Vector3 target = _modelInitialScale;
        if (squishX) target.x *= collisionSize;
        if (squishY) target.y *= collisionSize;
        if (squishZ) target.z *= collisionSize;

        float t = 0f;
        // encolher
        while (t < half)
        {
            t += Time.deltaTime;
            currentModelInstance.transform.localScale = Vector3.Lerp(
                _modelInitialScale, target, t / half
            );
            yield return null;
        }

        // crescer de volta
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            currentModelInstance.transform.localScale = Vector3.Lerp(
                target, _modelInitialScale, t / half
            );
            yield return null;
        }

        // garante escala original
        currentModelInstance.transform.localScale = _modelInitialScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(hammerTag))
            ReceiveHit();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(hammerTag))
        {
            _isPressed = false;
            OnReleased?.Invoke();
        }
    }
}
