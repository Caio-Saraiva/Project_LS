using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleItem : MonoBehaviour
{
    public static event Action<int> OnItemDestroyed;

    // =======================
    // CONTROLE GLOBAL
    // =======================
    private static bool _destructibleEnabled = false;
    /// <summary>
    /// Liga/desliga o modo destrutível para TODOS os itens.
    /// </summary>
    public static void SetDestructibleEnabled(bool enabled)
    {
        _destructibleEnabled = enabled;
    }

    // =======================
    // INSPECTOR FIELDS
    // =======================
    [Header("Durabilidade & Pontos")]
    [Tooltip("Número de hits necessários para destruir o item.")]
    public int durability = 3;
    [Tooltip("Pontos concedidos quando o item é destruído.")]
    public int points = 100;

    [Header("Modelos 3D")]
    [Tooltip("Modelo inicial do item.")]
    public GameObject originalModelPrefab;
    [Tooltip("Modelos para cada estágio de destruição (durability-1 itens).")]
    public List<GameObject> destroyStagePrefabs;

    [Header("Efeitos & Áudio")]
    [Tooltip("Prefab de efeito (partículas) ao receber hit.")]
    public GameObject hitEffectPrefab;
    [Tooltip("Prefab de efeito ao ser destruído.")]
    public GameObject destroyEffectPrefab;
    [Tooltip("Sons aleatórios ao receber hit.")]
    public List<AudioClip> hitSounds;
    [Tooltip("Sons aleatórios ao ser destruído.")]
    public List<AudioClip> destroySounds;

    [Header("Tags de Colisão")]
    [Tooltip("Tag usada para identificar o martelo.")]
    public string hammerTag = "Hammer";
    [Tooltip("Tag usada para identificar o chão.")]
    public string groundTag = "Ground";

    [Header("Feedback de Escala ao Hit")]
    [Range(0.1f, 1f)]
    [Tooltip("Escala relativa ao original durante o hit (1 = sem alteração).")]
    public float collisionSize = 0.85f;
    [Tooltip("Duração total do efeito de squish (ida e volta).")]
    public float collisionSizeTime = 0.2f;

    [Header("Floating Points UI")]
    [Tooltip("Prefab de FloatingTextUI (Canvas WS + TextMeshProUGUI) para mostrar pontos.")]
    public GameObject floatingTextUIPrefab;
    [Tooltip("Offset em Y para posicionar o texto acima do item.")]
    public Vector3 floatingTextUIOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("Rotação extra em Euler para o texto billboard.")]
    public Vector3 floatingTextUIRotOffset = Vector3.zero;

    // =======================
    // ESTADO INTERNO
    // =======================
    bool hasLanded = false;
    bool canBeHit = false;
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
        if (originalModelPrefab == null) return;
        currentModelInstance = Instantiate(
            originalModelPrefab,
            transform.position,
            transform.rotation,
            transform
        );
    }

    /// <summary>
    /// Deve ser chamado ao detectar hit (colisão com martelo).
    /// </summary>
    public void ReceiveHit()
    {
        // Toca som de hit
        if (hitSounds != null && hitSounds.Count > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        // Efeito visual de hit
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // Feedback de escala
        StartCoroutine(ScalePulse());

        // Se não estiver pronto ou destrutível desativado ou game over, aborta
        if (!canBeHit || !_destructibleEnabled || GameTimer.isGameOver)
            return;

        // Decrementa durabilidade
        durability--;
        Debug.Log($"{name} recebeu hit. Durabilidade restante: {durability}/{initialDurability}");

        // Atualiza estágio ou destrói
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

        // Encolhe
        while (t < half)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, target, t / half);
            yield return null;
        }

        // Volta ao normal
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
        if (destroyStagePrefabs != null &&
            idx >= 0 && idx < destroyStagePrefabs.Count)
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
        // Som de destruição
        if (destroySounds != null && destroySounds.Count > 0)
        {
            var clip = destroySounds[UnityEngine.Random.Range(0, destroySounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        // Efeito de destruição
        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

        // Floating text de pontos
        if (floatingTextUIPrefab != null)
        {
            var pos = transform.position + floatingTextUIOffset;
            var go = Instantiate(floatingTextUIPrefab, pos, Quaternion.identity);
            var ft = go.GetComponent<FloatingTextUI>();
            if (ft != null)
            {
                ft.rotationOffset = floatingTextUIRotOffset;
                ft.Show(points);
            }
        }

        OnItemDestroyed?.Invoke(points);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Marca aterrissagem
        if (!hasLanded && collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = true;
            StartCoroutine(EnableHitDetection());
        }

        // Ao tocar o martelo, chama ReceiveHit
        if (collision.gameObject.CompareTag(hammerTag))
            ReceiveHit();
    }

    IEnumerator EnableHitDetection()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHit = true;
    }
}
