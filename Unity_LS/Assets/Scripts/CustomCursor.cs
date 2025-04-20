using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
    public enum CursorMode { PanelMode, HorizontalMode }

    [Header("Cursor Config")]
    public CursorMode mode = CursorMode.PanelMode;
    [Tooltip("Prefab que será usado como cursor (3D)")]
    public GameObject cursorPrefab;
    [Tooltip("Distância da câmera (XY mode)")]
    public float distanceFromCamera = 10f;

    [Header("Transform Custom")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3 cursorScale = Vector3.one;

    [Header("Movement Bounds")]
    [Tooltip("BoxCollider (não-isTrigger) para limitar o cursor")]
    public BoxCollider movementBounds;

    [Header("Ações de Movimento")]
    [Tooltip("Arraste aqui suas InputActions do tipo Vector2 (p.ex. LeftStick)")]
    public List<InputActionReference> moveActions;
    [Tooltip("Sensibilidade (pixeis/s) ao usar gamepad")]
    public float gamepadSpeed = 1000f;
    [Tooltip("Deadzone abaixo da qual ignora o input do stick")]
    public float moveDeadzone = 0.2f;

    GameObject cursorInstance;
    Camera cam;
    Vector2 screenPos;
    bool usingGamepad;

    void OnEnable()
    {
        foreach (var ar in moveActions)
            ar.action.Enable();
    }

    void OnDisable()
    {
        foreach (var ar in moveActions)
            ar.action.Disable();
    }

    void Start()
    {
        Cursor.visible = false;
        cam = Camera.main;
        screenPos = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : new Vector2(Screen.width / 2, Screen.height / 2);

        if (cursorPrefab != null)
        {
            cursorInstance = Instantiate(cursorPrefab,
                                         Vector3.zero,
                                         Quaternion.Euler(rotationOffset));
            cursorInstance.transform.localScale = cursorScale;
        }
        else Debug.LogError("Cursor Prefab não atribuído!");
    }

    void Update()
    {
        // lê o somatório de todos os bindings de move
        Vector2 gp = Vector2.zero;
        foreach (var ar in moveActions)
            gp += ar.action.ReadValue<Vector2>();

        // detecta se estamos usando stick ou mouse
        if (gp.magnitude > moveDeadzone)
            usingGamepad = true;
        else if (Mouse.current != null &&
                 Mouse.current.delta.ReadValue() != Vector2.zero)
            usingGamepad = false;

        // aplica o movimento
        if (usingGamepad)
            MoveWithGamepad(gp);
        else
            MoveWithMouse();
    }

    void MoveWithMouse()
    {
        screenPos = Mouse.current.position.ReadValue();
        UpdateInstancePosition();
    }

    void MoveWithGamepad(Vector2 gp)
    {
        screenPos += gp * (gamepadSpeed * Time.deltaTime);
        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
        UpdateInstancePosition();
    }

    void UpdateInstancePosition()
    {
        if (cursorInstance == null) return;

        Vector3 worldPos = Vector3.zero;
        if (mode == CursorMode.PanelMode)
        {
            var m = new Vector3(screenPos.x, screenPos.y, distanceFromCamera);
            worldPos = cam.ScreenToWorldPoint(m);
        }
        else
        {
            var plane = new Plane(Vector3.up, Vector3.zero);
            var ray = cam.ScreenPointToRay(screenPos);
            if (plane.Raycast(ray, out var d))
                worldPos = ray.GetPoint(d);
        }

        worldPos += positionOffset;
        if (movementBounds != null)
            worldPos = movementBounds.ClosestPoint(worldPos);

        cursorInstance.transform.position = worldPos;
        cursorInstance.transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
