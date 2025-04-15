using UnityEngine;

public class Mouse3DCursor : MonoBehaviour
{
    // Dist�ncia da c�mera onde o cursor 3D ficar� (para c�meras em perspectiva).
    public float distanceFromCamera = 10f;

    void Start()
    {
        // Esconde o cursor do sistema.
        Cursor.visible = false;
    }

    void Update()
    {
        // Obt�m a posi��o atual do mouse na tela.
        Vector3 mousePosition = Input.mousePosition;
        // Define a dist�ncia no eixo z.
        mousePosition.z = distanceFromCamera;
        // Converte para coordenadas de mundo.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        // Move o prefab para a posi��o calculada.
        transform.position = worldPosition;
    }
}
