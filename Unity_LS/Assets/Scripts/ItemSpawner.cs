using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Áreas de Spawn")]
    [Tooltip("Área onde os itens serão spawnados periodicamente (ex.: no ar).")]
    public BoxCollider spawnArea;
    [Tooltip("Área onde os itens serão spawnados no início do jogo (próximo ao chão).")]
    public BoxCollider initialSpawnArea;

    [Header("Dispersão")]
    [Tooltip("Distância mínima desejada entre quaisquer dois itens na cena.")]
    public float MinDistanceBetweenItems = 1f;

    [System.Serializable]
    public class SpawnablePrefab
    {
        [Tooltip("Prefab a ser spawnado.")]
        public GameObject prefab;
        [Tooltip("Porcentagem (chance) de o prefab ser selecionado.")]
        [Range(0, 100)] public float spawnChance = 100f;
    }

    [Header("Configuração dos Prefabs")]
    [Tooltip("Lista dos prefabs e suas chances de serem spawnados.")]
    public List<SpawnablePrefab> spawnablePrefabs = new List<SpawnablePrefab>();

    [Header("Configuração do Spawn")]
    [Tooltip("Número máximo de itens permitidos na cena.")]
    public int maxItems = 10;
    [Tooltip("Intervalo de tempo entre cada spawn periódico (em segundos).")]
    public float spawnInterval = 2f;
    [Tooltip("Número de amostras a tentar para encontrar o local mais vazio.")]
    public int samplingAttempts = 5;

    [Header("Random Rotation")]
    [Tooltip("Habilita rotação aleatória em degraus nos eixos selecionados.")]
    public bool randomRotation = false;
    [Tooltip("Permite rotação aleatória no eixo X.")]
    public bool randomRotationX = false;
    [Tooltip("Permite rotação aleatória no eixo Y.")]
    public bool randomRotationY = true;
    [Tooltip("Permite rotação aleatória no eixo Z.")]
    public bool randomRotationZ = false;
    [Tooltip("Tamanho do passo (em graus) para a rotação aleatória.")]
    public float randomRotationStep = 45f;

    private float spawnTimer;

    void Start()
    {
        if (initialSpawnArea != null)
            SpawnInitialItems();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnItem();
        }
    }

    void TrySpawnItem()
    {
        var existing = GameObject.FindGameObjectsWithTag("SpawnedItem");
        if (existing.Length >= maxItems || spawnArea == null)
            return;

        // Coleta posições XZ dos itens já no chão
        List<Vector2> existingXZ = new List<Vector2>(existing.Length);
        foreach (var go in existing)
        {
            var p = go.transform.position;
            existingXZ.Add(new Vector2(p.x, p.z));
        }

        Vector3 bestPos = Vector3.zero;
        float bestScore = -1f;

        // Amostras para encontrar posição mais longe
        for (int i = 0; i < samplingAttempts; i++)
        {
            Vector3 candidate = GetRandomPositionInCollider(spawnArea);
            Vector2 c2 = new Vector2(candidate.x, candidate.z);

            // Distância mínima até qualquer item existente
            float minDist = float.MaxValue;
            foreach (var e in existingXZ)
            {
                float d = Vector2.Distance(c2, e);
                if (d < minDist) minDist = d;
            }
            // Se não houver itens, treat as infinite
            if (existingXZ.Count == 0) minDist = float.MaxValue;

            // Se passar do threshold, escolhe imediatamente
            if (minDist >= MinDistanceBetweenItems)
            {
                bestPos = candidate;
                bestScore = minDist;
                break;
            }

            // Senão, mantém melhor candidato
            if (minDist > bestScore)
            {
                bestScore = minDist;
                bestPos = candidate;
            }
        }

        // Instancia
        var prefab = SelectPrefabByChance();
        if (prefab == null) return;

        var obj = Instantiate(prefab, bestPos, Quaternion.identity);
        obj.tag = "SpawnedItem";
        ApplyRandomRotation(obj);

        // Pequeno impulso
        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(new Vector3(
                Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)
            ), ForceMode.Impulse);
    }

    void SpawnInitialItems()
    {
        var chosen = new List<Vector3>();
        int spawned = 0, attempts = 0, maxAtt = maxItems * 10;

        while (spawned < maxItems && attempts < maxAtt)
        {
            attempts++;
            var pos = GetRandomPositionInCollider(initialSpawnArea);
            bool tooClose = false;
            foreach (var p in chosen)
                if (Vector3.Distance(p, pos) < MinDistanceBetweenItems)
                {
                    tooClose = true;
                    break;
                }
            if (tooClose) continue;

            var prefab = SelectPrefabByChance();
            if (prefab != null)
            {
                var obj = Instantiate(prefab, pos, Quaternion.identity);
                obj.tag = "SpawnedItem";
                ApplyRandomRotation(obj);
            }
            chosen.Add(pos);
            spawned++;
        }
    }

    Vector3 GetRandomPositionInCollider(BoxCollider col)
    {
        var mn = col.bounds.min;
        var mx = col.bounds.max;
        return new Vector3(
            Random.Range(mn.x, mx.x),
            Random.Range(mn.y, mx.y),
            Random.Range(mn.z, mx.z)
        );
    }

    GameObject SelectPrefabByChance()
    {
        float tot = 0f;
        foreach (var sp in spawnablePrefabs) tot += sp.spawnChance;
        if (tot <= 0f) return null;
        float r = Random.Range(0f, tot), acc = 0f;
        foreach (var sp in spawnablePrefabs)
        {
            acc += sp.spawnChance;
            if (r <= acc) return sp.prefab;
        }
        return null;
    }

    void ApplyRandomRotation(GameObject obj)
    {
        if (!randomRotation) return;
        int maxStep = Mathf.FloorToInt(360f / randomRotationStep);
        float x = 0, y = 0, z = 0;
        if (randomRotationX) x = Random.Range(0, maxStep + 1) * randomRotationStep;
        if (randomRotationY) y = Random.Range(0, maxStep + 1) * randomRotationStep;
        if (randomRotationZ) z = Random.Range(0, maxStep + 1) * randomRotationStep;
        obj.transform.rotation = Quaternion.Euler(x, y, z);
    }
}
