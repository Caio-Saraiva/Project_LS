using UnityEngine;

public class SingleSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab que será instanciado")]
    public GameObject prefabToSpawn;

    [Tooltip("Transform que marca o ponto exato de spawn")]
    public Transform spawnPoint;

    [Header("Detection Area")]
    [Tooltip("Collider que define a área onde contamos quantos objetos já existem")]
    public Collider detectionArea;

    [Tooltip("Tag usada para identificar os objetos spawnados")]
    public string detectionTag = "SpawnedItem";

    [Header("Spawn Control")]
    [Tooltip("Máximo de objetos permitidos simultaneamente dentro da área")]
    public int maxSpawns = 1;

    [Tooltip("Tempo de espera (em segundos) entre um spawn e a próxima detecção")]
    public float spawnInterval = 5f;

    // estado interno de cooldown
    bool _inCooldown = false;
    float _cooldownTime = 0f;

    void Update()
    {
        // se estamos em cooldown, conta o tempo
        if (_inCooldown)
        {
            _cooldownTime += Time.deltaTime;
            if (_cooldownTime >= spawnInterval)
            {
                _inCooldown = false;
                _cooldownTime = 0f;
            }
            return;
        }

        // não estamos em cooldown: detecta quantos itens existem
        if (prefabToSpawn == null || spawnPoint == null || detectionArea == null)
            return;

        var b = detectionArea.bounds;
        Collider[] hits = Physics.OverlapBox(
            b.center,
            b.extents,
            detectionArea.transform.rotation
        );

        int count = 0;
        foreach (var c in hits)
            if (c.CompareTag(detectionTag))
                count++;

        // se estiver vazio, spawn e entra em cooldown
        if (count < maxSpawns)
        {
            var go = Instantiate(
                prefabToSpawn,
                spawnPoint.position,
                spawnPoint.rotation
            );
            go.tag = detectionTag;
            _inCooldown = true;
            _cooldownTime = 0f;
        }
    }

    // para ver a área no Editor
    void OnDrawGizmosSelected()
    {
        if (detectionArea == null) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        var b = detectionArea.bounds;
        Gizmos.matrix = Matrix4x4.TRS(b.center, detectionArea.transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, b.size);
    }
}
