using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]  // para rodar antes do UI Input Module
public class UIActionsEnabler : MonoBehaviour
{
    [Tooltip("Arraste aqui seu InputActions Asset com o mapa 'UI'")]
    public InputActionAsset uiActions;

    void Awake()
    {
        if (uiActions != null)
            uiActions.Enable();
        else
            Debug.LogWarning("UIActionsEnabler: arraste o seu DefaultInputActions no inspector.");
    }
}
