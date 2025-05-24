using UnityEngine;

public class Mouse3DCursor : MonoBehaviour
{
    // Distância da câmera onde o cursor 3D ficará (para câmeras em perspectiva).
    public float distanceFromCamera = 10f;

    void Start()
    {
        // Esconde o cursor do sistema.
        Cursor.visible = false;
    }

    void Update()
    {
        // Obtém a posição atual do mouse na tela.
        Vector3 mousePosition = Input.mousePosition;
        // Define a distância no eixo z.
        mousePosition.z = distanceFromCamera;
        // Converte para coordenadas de mundo.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Move o prefab para a posição calculada.
        transform.position = worldPosition;
    }
}
