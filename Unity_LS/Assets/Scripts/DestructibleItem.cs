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
    [Tooltip("Lista de prefabs de estágios de destruição (ordem: após 1º hit, 2º hit, …).")]
    public List<GameObject> destroyStagePrefabs;

    [Header("Efeitos e Áudio")]
    public List<AudioClip> hitSounds;
    public List<AudioClip> destroySounds;
    public GameObject destroyEffectPrefab;

    // flags internas
    private bool hasLanded = false;
    private bool canBeHit = false;

    // para trocar o modelo
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
        else if (transform.childCount > 0)
        {
            currentModelInstance = transform.GetChild(0).gameObject;
        }
        else
        {
            Debug.LogWarning($"{name}: nenhum modelo inicial atribuído ou filho encontrado.");
        }
    }

    public void ReceiveHit()
    {
        // mesmo em gameOver, tocamos som de hit
        if (hitSounds != null && hitSounds.Count > 0)
        {
            var clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        // sem mais lógica se não aterrissou, ou se é gameOver
        if (!canBeHit || GameTimer.isGameOver) return;

        durability--;
        Debug.Log($"{name} recebeu hit. Durabilidade restante: {durability}");

        if (durability > 0)
        {
            UpdateModelStage();
        }
        else
        {
            DestroyItem();
        }
    }

    private void UpdateModelStage()
    {
        int hitsTaken = initialDurability - durability;
        int stageIndex = hitsTaken - 1; // 1º hit → índice 0, etc.

        if (
            destroyStagePrefabs != null &&
            stageIndex >= 0 &&
            stageIndex < destroyStagePrefabs.Count
        )
        {
            // remove modelo antigo
            if (currentModelInstance != null)
                Destroy(currentModelInstance);

            // instancia próximo estágio
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
        {
            Debug.Log($"{name}: fim de jogo, destruição ignorada.");
            return;
        }

        Debug.Log($"{name} destruído! +{points} pontos");
        OnItemDestroyed?.Invoke(points);

        // som de destruição
        if (destroySounds != null && destroySounds.Count > 0)
        {
            var clip = destroySounds[UnityEngine.Random.Range(0, destroySounds.Count)];
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        // efeito visual
        if (destroyEffectPrefab != null)
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            StartCoroutine(EnableHitDetection());
        }

        if (collision.gameObject.CompareTag("Hammer"))
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
