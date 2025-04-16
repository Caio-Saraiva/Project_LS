using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    // Enum para selecionar o modo de movimentação do cursor.
    public enum CursorMode
    {
        PanelMode,      // Atualiza os eixos X e Y (usando ScreenToWorldPoint).
        HorizontalMode  // Atualiza os eixos X e Z (usando raycast em um plano horizontal).
    }

    [Header("Configuração do Cursor")]
    [Tooltip("Prefab que será utilizado como cursor personalizado")]
    public GameObject cursorPrefab;

    [Tooltip("Modo de movimentação do cursor")]
    public CursorMode mode = CursorMode.PanelMode;

    [Tooltip("Distância da câmera para posicionar o cursor (usada no PanelMode)")]
    public float distanceFromCamera = 10f;

    [Header("Customização do Transform")]
    [Tooltip("Offset a ser adicionado à posição do cursor")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("Rotação (Euler) a ser aplicada ao cursor")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Escala aplicada ao prefab do cursor")]
    public Vector3 cursorScale = Vector3.one;

    private GameObject cursorInstance;

    void Start()
    {
        // Esconde o cursor padrão do sistema.
        Cursor.visible = false;

        // Instancia o prefab do cursor se estiver atribuído.
        if (cursorPrefab != null)
        {
            // Instancia o cursor com a rotação definida por rotationOffset.
            cursorInstance = Instantiate(cursorPrefab, Vector3.zero, Quaternion.Euler(rotationOffset));
            // Aplica a escala personalizada.
            cursorInstance.transform.localScale = cursorScale;
        }
        else
        {
            Debug.LogError("Prefab do cursor não foi atribuído no Inspector!");
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
            // Usa a função ScreenToWorldPoint para converter a posição do mouse.
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
        // Atualiza a rotação do cursor conforme o rotationOffset definido.
        cursorInstance.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
