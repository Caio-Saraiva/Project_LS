using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleItem : MonoBehaviour
{
    public static event Action<int> OnItemDestroyed;

    [Header("Durabilidade & Pontos")]
    [Tooltip("Número de hits necessários para destruir o item.")]
    public int durability = 3;
    [Tooltip("Pontos concedidos quando o item é destruído.")]
    public int points = 100;

    [Header("Modelos 3D")]
    public GameObject originalModelPrefab;
    public List<GameObject> destroyStagePrefabs;

    [Header("Efeitos & Áudio")]
    public GameObject hitEffectPrefab;
    public GameObject destroyEffectPrefab;
    public List<AudioClip> hitSounds;
    public List<AudioClip> destroySounds;

    [Header("Tags de Colisão")]
    public string hammerTag = "Hammer";
    public string groundTag = "Ground";

    [Header("Feedback de Escala ao Hit")]
    [Range(0.1f, 1f)] public float collisionSize = 0.85f;
    public float collisionSizeTime = 0.2f;

    [Header("Floating Points UI")]
    [Tooltip("Prefab de FloatingTextUI (Canvas WS + TextMeshProUGUI).")]
    public GameObject floatingTextUIPrefab;
    [Tooltip("Offset em Y para posicionar o texto acima do item.")]
    public Vector3 floatingTextUIOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("Rotação extra em Euler para o texto billboard.")]
    public Vector3 floatingTextUIRotOffset = Vector3.zero;

    // estado interno
    bool hasLanded;
    bool canBeHit;
    int initialDurability;
    Vector3 initialScale;
    GameObject currentModelInstance;

    void Start()
    {
        initialDurability = durability;
        initialScale = transform.localScale;
        SpawnInitialModel();
    }

    void SpawnInitialModel()
    {
        if (originalModelPrefab != null)
        {
            currentModelInstance = Instantiate(
                originalModelPrefab,
                transform.position,
                transform.rotation,
                transform
            );
        }
    }

    public void ReceiveHit()
    {
        // 1) som de hit
        if (hitSounds != null && hitSounds.Count > 0)
            AudioSource.PlayClipAtPoint(
                hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)],
                transform.position
            );

        // 2) efeito de hit
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // 3) pulso suave de escala
        StartCoroutine(ScalePulse());

        // 4) só desconta durabilidade se já pousou e não for game over
        if (!canBeHit || GameTimer.isGameOver) return;

        durability--;
        Debug.Log($"{name} recebeu hit. Durabilidade: {durability}/{initialDurability}");

        if (durability > 0)
            UpdateModelStage();
        else
            DestroyItem();
    }

    IEnumerator ScalePulse()
    {
        float half = collisionSizeTime * 0.5f;
        Vector3 target = initialScale * collisionSize;
        float t = 0f;

        // encolher
        while (t < half)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, target, t / half);
            yield return null;
        }

        // crescer de volta
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(target, initialScale, t / half);
            yield return null;
        }

        transform.localScale = initialScale;
    }

    void UpdateModelStage()
    {
        int idx = (initialDurability - durability) - 1;
        if (destroyStagePrefabs != null
            && idx >= 0
            && idx < destroyStagePrefabs.Count)
        {
            if (currentModelInstance != null)
                Destroy(currentModelInstance);

            currentModelInstance = Instantiate(
                destroyStagePrefabs[idx],
                transform.position,
                transform.rotation,
                transform
            );
        }
    }

    void DestroyItem()
    {
        if (GameTimer.isGameOver) return;

        Debug.Log($"{name} destruído! +{points} pts");
        OnItemDestroyed?.Invoke(points);

        // **Aqui** instanciamos o prefab separado de FloatingTextUI:
        if (floatingTextUIPrefab != null)
        {
            Vector3 spawnPos = transform.position + floatingTextUIOffset;
            GameObject go = Instantiate(floatingTextUIPrefab, spawnPos, Quaternion.identity);
            FloatingTextUI ft = go.GetComponent<FloatingTextUI>();
            if (ft != null)
            {
                ft.rotationOffset = floatingTextUIRotOffset;
                ft.Show(points);
            }
        }

        // efeito final e som…
        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        if (destroySounds != null && destroySounds.Count > 0)
            AudioSource.PlayClipAtPoint(
                destroySounds[UnityEngine.Random.Range(0, destroySounds.Count)],
                transform.position
            );

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // pousou no chão?
        if (!hasLanded && collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = true;
            StartCoroutine(EnableHitDetection());
        }

        // colidiu com martelo?
        if (collision.gameObject.CompareTag(hammerTag))
            ReceiveHit();
    }

    IEnumerator EnableHitDetection()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHit = true;
    }
}
