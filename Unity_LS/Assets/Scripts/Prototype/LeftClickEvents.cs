using UnityEngine;
using UnityEngine.Events;

public class LeftClickEvents : MonoBehaviour
{
    // Array de eventos configur�veis pelo Inspector.
    // Voc� pode adicionar quantos eventos desejar.
    public UnityEvent[] eventosAoClicar;

    void Update()
    {
        // Verifica se o bot�o esquerdo do mouse foi pressionado.
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
