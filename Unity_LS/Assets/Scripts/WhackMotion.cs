using UnityEngine;
using System.Collections;

public class WhackMotion : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [Tooltip("Rotação inicial (Euler) do objeto antes da batida")]
    public Vector3 initialRotationEuler = Vector3.zero;
    [Tooltip("Rotação final (Euler) que o objeto atinge no ápice da batida")]
    public Vector3 finalRotationEuler = new Vector3(0, 0, -90);

    [Header("Configuração da Animação")]
    [Tooltip("Duração total da animação de batida (em segundos)")]
    public float animationDuration = 0.5f;

    private bool isWhacking = false;

    void Start()
    {
        // Garante que o objeto inicia com a rotação inicial definida.
        transform.rotation = Quaternion.Euler(initialRotationEuler);
    }

    void Update()
    {
        // Ao clicar com o botão esquerdo do mouse e se não estiver em processo de batida, inicia a animação.
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

        // Primeira fase: movimento da rotação inicial para a final.
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, t);
            yield return null;
        }

        // Segunda fase: movimento de retorno da rotação final para a inicial.
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            transform.rotation = Quaternion.Lerp(finalRotation, initialRotation, t);
            yield return null;
        }

        // Garante que a rotação final seja exatamente a rotação inicial.
        transform.rotation = initialRotation;

        isWhacking = false;
    }
}
