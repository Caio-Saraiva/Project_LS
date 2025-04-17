using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleItem : MonoBehaviour
{
    public static event Action<int> OnItemDestroyed;

    [Header("Durabilidade e Pontos")]
    [Tooltip("Número de hits necessários para destruir o item.")]
    public int durability = 3;
    [Tooltip("Pontos concedidos quando o item é destruído.")]
    public int points = 100;

    [Header("Modelos 3D")]
    [Tooltip("Prefab do modelo 3D inicial.")]
    public GameObject originalModelPrefab;
    [Tooltip("Lista de prefabs de estágios de destruição.")]
    public List<GameObject> destroyStagePrefabs;

    [Header("Efeitos")]
    [Tooltip("Prefab de efeito instanciado em cada hit (mudança de estágio).")]
    public GameObject hitEffectPrefab;
    [Tooltip("Prefab de efeito instanciado na destruição final.")]
    public GameObject destroyEffectPrefab;

    [Header("Áudio")]
    [Tooltip("Clipes de áudio para hits.")]
    public List<AudioClip> hitSounds;
    [Tooltip("Clipes de áudio para destruição.")]
    public List<AudioClip> destroySounds;

    [Header("Tags de Colisão")]
    [Tooltip("Tag usada para identificar o martelo.")]
    public string hammerTag = "Hammer";
    [Tooltip("Tag usada para identificar o chão.")]
    public string groundTag = "Ground";

    // Flags internas
    private bool hasLanded = false;
    private bool canBeHit = false;
    private int initialDurability;
    private GameObject currentModelInstance;

    void Start()
    {
        initialDurability = durability;
        SpawnInitialModel();
    }

    private void SpawnInitialModel()
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
        // Sempre toca o som de hit, mesmo após game over
        if (hitSounds != null && hitSounds.Count > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        // Efeito de hit
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // Se não pousou ou o jogo acabou, não reduz durabilidade
        if (!canBeHit || GameTimer.isGameOver)
            return;

        durability--;
        Debug.Log($"{name} recebeu hit. Durab.: {durability}/{initialDurability}");

        if (durability > 0)
            UpdateModelStage();
        else
            DestroyItem();
    }

    private void UpdateModelStage()
    {
        int stageIndex = (initialDurability - durability) - 1;
        if (destroyStagePrefabs != null
            && stageIndex >= 0
            && stageIndex < destroyStagePrefabs.Count)
        {
            Destroy(currentModelInstance);
            currentModelInstance = Instantiate(
                destroyStagePrefabs[stageIndex],
                transform.position,
                transform.rotation,
                transform
            );
        }
    }

    private void DestroyItem()
    {
        if (GameTimer.isGameOver)
            return;

        Debug.Log($"{name} destruído! +{points} pts");
        OnItemDestroyed?.Invoke(points);

        // Efeito de destruição
        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

        // Som de destruição
        if (destroySounds != null && destroySounds.Count > 0)
        {
            var clip = destroySounds[UnityEngine.Random.Range(0, destroySounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Pouso no chão
        if (!hasLanded && collision.gameObject.CompareTag(groundTag))
        {
            hasLanded = true;
            StartCoroutine(EnableHitDetection());
        }

        // Colisão com martelo
        if (collision.gameObject.CompareTag(hammerTag))
        {
            ReceiveHit();
        }
    }

    private IEnumerator EnableHitDetection()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHit = true;
    }
}
