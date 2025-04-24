using UnityEngine;
using UnityEngine.EventSystems;

public class UIPointerDebugger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData e)
    {
        Debug.Log("Pointer entrou em: " + gameObject.name);
    }
    public void OnPointerClick(PointerEventData e)
    {
        Debug.Log("Pointer clicou em: " + gameObject.name);
    }
}
