using UnityEngine;

public class ClearItems : MonoBehaviour
{
    [Header("Configuração da Área")]
    [Tooltip("BoxCollider que define a região onde os itens serão destruídos")]
    public BoxCollider areaCollider;

    [Header("Filtro de Tag")]
    [Tooltip("Tag dos objetos que serão destruídos dentro da área")]
    public string targetTag = "SpawnedItem";

    [Header("Delay (opcional)")]
    [Tooltip("Tempo em segundos para aguardar antes de destruir cada objeto")]
    public float destroyDelay = 0f;

    /// <summary>
    /// Chame este método manualmente (por exemplo, de um botão)
    /// para destruir todos os objetos com a tag targetTag dentro da área.
    /// </summary>
    public void ClearAllItems()
    {
        if (areaCollider == null)
        {
            Debug.LogError("ClearItems: areaCollider não foi atribuído.");
            return;
        }

        // Obtém os bounds do collider
        Bounds b = areaCollider.bounds;

        // Executa um OverlapBox usando o centro, extents e rotação do BoxCollider
        Collider[] hits = Physics.OverlapBox(
            b.center,
            b.extents,
            areaCollider.transform.rotation
        );

        int destroyed = 0;
        foreach (var col in hits)
        {
            if (col.CompareTag(targetTag))
            {
                Destroy(col.gameObject, destroyDelay);
                destroyed++;
            }
        }

        Debug.Log($"ClearItems: {destroyed} objeto(s) com tag '{targetTag}' destruído(s).");
    }

    // Desenha a área no editor para facilitar o ajuste
    void OnDrawGizmosSelected()
    {
        if (areaCollider == null) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Bounds b = areaCollider.bounds;
        Gizmos.matrix = Matrix4x4.TRS(b.center, areaCollider.transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, b.size);
    }
}
