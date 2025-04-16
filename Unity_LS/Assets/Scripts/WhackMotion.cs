using UnityEngine;
using System.Collections;

public class WhackMotion : MonoBehaviour
{
    [Header("Configura��es de Rota��o")]
    [Tooltip("Rota��o inicial (Euler) do objeto antes da batida")]
    public Vector3 initialRotationEuler = Vector3.zero;
    [Tooltip("Rota��o final (Euler) que o objeto atinge no �pice da batida")]
    public Vector3 finalRotationEuler = new Vector3(0, 0, -90);

    [Header("Configura��o da Anima��o")]
    [Tooltip("Dura��o total da anima��o de batida (em segundos)")]
    public float animationDuration = 0.5f;

    private bool isWhacking = false;

    void Start()
    {
        // Garante que o objeto inicia com a rota��o inicial definida.
        transform.rotation = Quaternion.Euler(initialRotationEuler);
    }

    void Update()
    {
        // Ao clicar com o bot�o esquerdo do mouse e se n�o estiver em processo de batida, inicia a anima��o.
        if (Input.GetMouseButtonDown(0) && !isWhacking)
        {
            StartCoroutine(PerformWhack());
        }
    }

    IEnumerator PerformWhack()
    {
        isWhacking = true;
        float halfDuration = animationDuration / 2f;
        float elapsed = 0f;
        Quaternion initialRotation = Quaternion.Euler(initialRotationEuler);
        Quaternion finalRotation = Quaternion.Euler(finalRotationEuler);

        // Primeira fase: movimento da rota��o inicial para a final.
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, t);
            yield return null;
        }

        // Segunda fase: movimento de retorno da rota��o final para a inicial.
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.rotation = Quaternion.Lerp(finalRotation, initialRotation, t);
            yield return null;
        }

        // Garante que a rota��o final seja exatamente a rota��o inicial.
        transform.rotation = initialRotation;

        isWhacking = false;
    }
}
