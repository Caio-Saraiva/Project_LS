using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public enum CursorMode
    {
        PanelMode,      // XY via ScreenToWorldPoint
        HorizontalMode  // XZ via Raycast num plano
    }

    [Header("Configura��o do Cursor")]
    public GameObject cursorPrefab;
    public CursorMode mode = CursorMode.PanelMode;
    public float distanceFromCamera = 10f;

    [Header("Customiza��o do Transform")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 cursorScale = Vector3.one;

    [Header("Limites de Movimento")]
    [Tooltip("BoxCollider (n�o-isTrigger) que define a �rea onde o martelo pode se mover")]
    public BoxCollider movementBounds;

    private GameObject cursorInstance;

    void Start()
    {
        Cursor.visible = false;
        if (cursorPrefab != null)
        {
            cursorInstance = Instantiate(cursorPrefab, Vector3.zero, Quaternion.Euler(rotationOffset));
            cursorInstance.transform.localScale = cursorScale;
        }
        else Debug.LogError("Cursor Prefab n�o atribu�do!");
    }

    void Update()
    {
        if (cursorInstance == null) return;

        Vector3 computedPosition = Vector3.zero;
        if (mode == CursorMode.PanelMode)
        {
            Vector3 m = Input.mousePosition;
            m.z = distanceFromCamera;
            computedPosition = Camera.main.ScreenToWorldPoint(m);
        }
        else // HorizontalMode
        {
            Plane p = new Plane(Vector3.up, Vector3.zero);
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (p.Raycast(r, out float d))
                computedPosition = r.GetPoint(d);
        }

        // offset e clamp
        computedPosition += positionOffset;
        if (movementBounds != null)
        {
            // Garante que fique dentro do box (se estiver fora, trunca para a borda)
            computedPosition = movementBounds.ClosestPoint(computedPosition);
        }

        cursorInstance.transform.position = computedPosition;
        cursorInstance.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
