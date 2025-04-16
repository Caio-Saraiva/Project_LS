using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    // Enum para selecionar o modo de movimenta��o do cursor.
    public enum CursorMode
    {
        PanelMode,      // Atualiza os eixos X e Y (usando ScreenToWorldPoint).
        HorizontalMode  // Atualiza os eixos X e Z (usando raycast em um plano horizontal).
    }

    [Header("Configura��o do Cursor")]
    [Tooltip("Prefab que ser� utilizado como cursor personalizado")]
    public GameObject cursorPrefab;

    [Tooltip("Modo de movimenta��o do cursor")]
    public CursorMode mode = CursorMode.PanelMode;

    [Tooltip("Dist�ncia da c�mera para posicionar o cursor (usada no PanelMode)")]
    public float distanceFromCamera = 10f;

    [Header("Customiza��o do Transform")]
    [Tooltip("Offset a ser adicionado � posi��o do cursor")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("Rota��o (Euler) a ser aplicada ao cursor")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Escala aplicada ao prefab do cursor")]
    public Vector3 cursorScale = Vector3.one;

    private GameObject cursorInstance;

    void Start()
    {
        // Esconde o cursor padr�o do sistema.
        Cursor.visible = false;

        // Instancia o prefab do cursor se estiver atribu�do.
        if (cursorPrefab != null)
        {
            // Instancia o cursor com a rota��o definida por rotationOffset.
            cursorInstance = Instantiate(cursorPrefab, Vector3.zero, Quaternion.Euler(rotationOffset));
            // Aplica a escala personalizada.
            cursorInstance.transform.localScale = cursorScale;
        }
        else
        {
            Debug.LogError("Prefab do cursor n�o foi atribu�do no Inspector!");
        }
    }

    void Update()
    {
        if (cursorInstance == null)
            return;

        Vector3 computedPosition = Vector3.zero;

        if (mode == CursorMode.PanelMode)
        {
            // Modo PanelMode (XY):
            // Usa a fun��o ScreenToWorldPoint para converter a posi��o do mouse.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceFromCamera;
            computedPosition = Camera.main.ScreenToWorldPoint(mousePos);
        }
        else if (mode == CursorMode.HorizontalMode)
        {
            // Modo HorizontalMode (XZ):
            // Realiza um raycast em um plano horizontal (com normal para cima).
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                computedPosition = ray.GetPoint(rayDistance);
            }
        }

        // Aplica o offset configurado.
        computedPosition += positionOffset;
        cursorInstance.transform.position = computedPosition;
        // Atualiza a rota��o do cursor conforme o rotationOffset definido.
        cursorInstance.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
