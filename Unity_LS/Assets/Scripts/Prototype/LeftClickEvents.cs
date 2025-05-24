using UnityEngine;
using UnityEngine.Events;

public class LeftClickEvents : MonoBehaviour
{
    // Array de eventos configuráveis pelo Inspector.
    // Você pode adicionar quantos eventos desejar.
    public UnityEvent[] eventosAoClicar;

    void Update()
    {
        // Verifica se o botão esquerdo do mouse foi pressionado.
        if (Input.GetMouseButtonDown(0))
        {
            // Percorre todos os eventos definidos e os invoca.
            foreach (UnityEvent evento in eventosAoClicar)
            {
                if (evento != null)
                {
                    evento.Invoke();
                }
            }
        }
    }
}
