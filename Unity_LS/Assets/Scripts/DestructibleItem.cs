using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleItem : MonoBehaviour
{
    // Evento disparado quando o item é destruído, passando os pontos correspondentes.
    public static event Action<int> OnItemDestroyed;

    [Header("Propriedades do Item")]
    [Tooltip("Número de hits necessários para destruir o item (apenas após aterrissar).")]
    public int durability = 3;
    [Tooltip("Pontos concedidos quando o item é destruído.")]
    public int points = 100;

    [Header("Efeitos")]
    [Tooltip("Lista de sons reproduzidos quando o item é atingido pelo martelo (será escolhido randomicamente).")]
    public List<AudioClip> hitSounds;
    [Tooltip("Lista de sons reproduzidos quando o item é destruído (será escolhido randomicamente).")]
    public List<AudioClip> destroySounds;
    [Tooltip("Prefab de efeito (ex: partículas) a ser instanciado na destruição.")]
    public GameObject destroyEffectPrefab;

    // Flag que indica se o item já tocou o chão.
    private bool hasLanded = false;
    // Flag que permite que o item seja atingido (ativa após aterrissar e um breve delay).
    private bool canBeHit = false;

    // Método para registrar um hit; o som é reproduzido independentemente do estado do jogo.
    public void ReceiveHit()
    {
        // Reproduz um som aleatório da lista de hitSounds, se houver.
        if (hitSounds != null && hitSounds.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, hitSounds.Count);
            AudioSource.PlayClipAtPoint(hitSounds[randomIndex], transform.position);
        }

        // Se o jogo acabou, apenas reproduz o som e não altera a durabilidade.
        if (GameTimer.isGameOver)
        {
            Debug.Log($"{gameObject.name}: O jogo acabou, não é possível destruir mais itens, mas som de hit foi reproduzido.");
            return;
        }

        // Se o item ainda não pode ser atingido, ignora o hit.
        if (!canBeHit)
            return;

        durability--;
        Debug.Log($"{gameObject.name} recebeu um hit. Durabilidade restante: {durability}");

        if (durability <= 0)
        {
            DestroyItem();
        }
    }

    // Método que trata a destruição do item, disparando o som, efeito e evento de pontuação.
    private void DestroyItem()
    {
        // Se o jogo acabou, não processa a destruição para evitar alteração indevida na pontuação.
        if (GameTimer.isGameOver)
        {
            Debug.Log($"{gameObject.name}: O jogo acabou, destruição não processada.");
            return;
        }

        Debug.Log($"{gameObject.name} destruído! Pontos concedidos: {points}");

        if (destroySounds != null && destroySounds.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, destroySounds.Count);
            AudioSource.PlayClipAtPoint(destroySounds[randomIndex], transform.position);
        }

        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }

        OnItemDestroyed?.Invoke(points);
        Destroy(gameObject);
    }

    // Detecção de colisões físicas.
    private void OnCollisionEnter(Collision collision)
    {
        // Se colide com o chão (tag "Ground") e ainda não foi marcado como tendo aterrissado, ativa o hit.
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            Debug.Log($"{gameObject.name} tocou o chão.");
            StartCoroutine(EnableHitDetection());
        }

        // Se colide com o martelo (tag "Hammer"), chama ReceiveHit.
        // Agora, mesmo que o jogo tenha acabado, o som de hit será reproduzido.
        if (collision.gameObject.CompareTag("Hammer"))
        {
            if (canBeHit || GameTimer.isGameOver)
            {
                ReceiveHit();
            }
            else
            {
                Debug.Log($"{gameObject.name} foi tocado pelo martelo, mas ainda não pode ser atingido.");
            }
        }
    }

    // Coroutine que, após um breve delay, permite que o item receba hits.
    private IEnumerator EnableHitDetection()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHit = true;
        Debug.Log($"{gameObject.name} agora pode ser atingido.");
    }
}
