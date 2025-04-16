using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleItem : MonoBehaviour
{
    // Evento disparado quando o item � destru�do, passando os pontos correspondentes.
    public static event Action<int> OnItemDestroyed;

    [Header("Propriedades do Item")]
    [Tooltip("N�mero de hits necess�rios para destruir o item (apenas ap�s aterrissar).")]
    public int durability = 3;
    [Tooltip("Pontos concedidos quando o item � destru�do.")]
    public int points = 100;

    [Header("Efeitos")]
    [Tooltip("Lista de sons reproduzidos quando o item � atingido pelo martelo (ser� escolhido randomicamente).")]
    public List<AudioClip> hitSounds;
    [Tooltip("Lista de sons reproduzidos quando o item � destru�do (ser� escolhido randomicamente).")]
    public List<AudioClip> destroySounds;
    [Tooltip("Prefab de efeito (ex: part�culas) a ser instanciado na destrui��o.")]
    public GameObject destroyEffectPrefab;

    // Flag que indica se o item j� tocou o ch�o.
    private bool hasLanded = false;
    // Flag que permite que o item seja atingido (ativa ap�s aterrissar e um breve delay).
    private bool canBeHit = false;

    // M�todo para registrar um hit; o som � reproduzido independentemente do estado do jogo.
    public void ReceiveHit()
    {
        // Reproduz um som aleat�rio da lista de hitSounds, se houver.
        if (hitSounds != null && hitSounds.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, hitSounds.Count);
            AudioSource.PlayClipAtPoint(hitSounds[randomIndex], transform.position);
        }

        // Se o jogo acabou, apenas reproduz o som e n�o altera a durabilidade.
        if (GameTimer.isGameOver)
        {
            Debug.Log($"{gameObject.name}: O jogo acabou, n�o � poss�vel destruir mais itens, mas som de hit foi reproduzido.");
            return;
        }

        // Se o item ainda n�o pode ser atingido, ignora o hit.
        if (!canBeHit)
            return;

        durability--;
        Debug.Log($"{gameObject.name} recebeu um hit. Durabilidade restante: {durability}");

        if (durability <= 0)
        {
            DestroyItem();
        }
    }

    // M�todo que trata a destrui��o do item, disparando o som, efeito e evento de pontua��o.
    private void DestroyItem()
    {
        // Se o jogo acabou, n�o processa a destrui��o para evitar altera��o indevida na pontua��o.
        if (GameTimer.isGameOver)
        {
            Debug.Log($"{gameObject.name}: O jogo acabou, destrui��o n�o processada.");
            return;
        }

        Debug.Log($"{gameObject.name} destru�do! Pontos concedidos: {points}");

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

    // Detec��o de colis�es f�sicas.
    private void OnCollisionEnter(Collision collision)
    {
        // Se colide com o ch�o (tag "Ground") e ainda n�o foi marcado como tendo aterrissado, ativa o hit.
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            Debug.Log($"{gameObject.name} tocou o ch�o.");
            StartCoroutine(EnableHitDetection());
        }

        // Se colide com o martelo (tag "Hammer"), chama ReceiveHit.
        // Agora, mesmo que o jogo tenha acabado, o som de hit ser� reproduzido.
        if (collision.gameObject.CompareTag("Hammer"))
        {
            if (canBeHit || GameTimer.isGameOver)
            {
                ReceiveHit();
            }
            else
            {
                Debug.Log($"{gameObject.name} foi tocado pelo martelo, mas ainda n�o pode ser atingido.");
            }
        }
    }

    // Coroutine que, ap�s um breve delay, permite que o item receba hits.
    private IEnumerator EnableHitDetection()
    {
        yield return new WaitForSeconds(0.1f);
        canBeHit = true;
        Debug.Log($"{gameObject.name} agora pode ser atingido.");
    }
}
