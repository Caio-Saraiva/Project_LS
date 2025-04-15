using UnityEngine;

public class Mouse3DCursorPlane : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        // Define um plano horizontal (normal aponta para cima) passando pela origem.
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        // Cria um ray que vai da posição do mouse na tela através da câmera.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (groundPlane.Raycast(ray, out distance))
        {
            // Obtém o ponto de interseção no mundo.
            Vector3 worldPosition = ray.GetPoint(distance);
            // Atualiza a posição do objeto 3D.
            transform.position = worldPosition;
        }
    }
}
