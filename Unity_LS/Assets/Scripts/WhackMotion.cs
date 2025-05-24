using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhackMotion : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    public Vector3 initialRotationEuler = Vector3.zero;
    public Vector3 finalRotationEuler = new Vector3(0, 0, -90);

    [Header("Configuração da Animação")]
    public float animationDuration = 0.5f;

    [Header("Ações de Whack")]
    [Tooltip("Arraste aqui todas as InputActions (p.ex. Button South) que devem disparar o whack")]
    public List<InputActionReference> whackActions;

    bool isWhacking = false;

    void OnEnable()
    {
        foreach (var ar in whackActions)
            ar.action.Enable();
    }

    void OnDisable()
    {
        foreach (var ar in whackActions)
            ar.action.Disable();
    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(initialRotationEuler);
    }

    void Update()
    {
        if (isWhacking) return;

        bool triggerDetected = false;

        // Detecta botões (gamepad, teclado, etc)
        foreach (var ar in whackActions)
        {
            if (ar.action.triggered)
            {
                triggerDetected = true;
                break;
            }
        }

        // Detecta toques (tela touchscreen)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            triggerDetected = true;
        }

        if (triggerDetected)
        {
            StartCoroutine(PerformWhack());
        }
    }

    IEnumerator PerformWhack()
    {
        isWhacking = true;
        float half = animationDuration * 0.5f;
        float elapsed = 0f;
        Quaternion startQ = Quaternion.Euler(initialRotationEuler);
        Quaternion endQ = Quaternion.Euler(finalRotationEuler);

        // Vai até o end
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            transform.rotation = Quaternion.Lerp(startQ, endQ, t);
            yield return null;
        }

        // Volta pro start
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / half);
            transform.rotation = Quaternion.Lerp(endQ, startQ, t);
            yield return null;
        }

        transform.rotation = startQ;
        isWhacking = false;
    }
}
