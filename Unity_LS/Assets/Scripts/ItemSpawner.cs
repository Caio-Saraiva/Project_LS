using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("�reas de Spawn")]
    [Tooltip("�rea onde os itens ser�o spawnados periodicamente (ex.: no ar).")]
    public BoxCollider spawnArea;
    [Tooltip("�rea onde os itens ser�o spawnados no in�cio do jogo (pr�ximo ao ch�o).")]
    public BoxCollider initialSpawnArea;
    [Tooltip("Dist�ncia m�nima entre itens spawnados inicialmente.")]
    public float minDistanceBetweenInitialSpawnItems = 1f;

    [System.Serializable]
    public class SpawnablePrefab
    {
        [Tooltip("Prefab a ser spawnado.")]
        public GameObject prefab;
        [Tooltip("Porcentagem (chance) de o prefab ser selecionado.")]
        [Range(0, 100)]
        public float spawnChance = 100f;
    }

    [Header("Configura��o dos Prefabs")]
    [Tooltip("Lista dos prefabs e suas chances de serem spawnados.")]
    public List<SpawnablePrefab> spawnablePrefabs = new List<SpawnablePrefab>();

    [Header("Configura��o do Spawn")]
    [Tooltip("N�mero m�ximo de itens permitidos na cena.")]
    public int maxItems = 10;
    [Tooltip("Intervalo de tempo entre cada spawn peri�dico (em segundos).")]
    public float spawnInterval = 2f;

    // Timer interno para spawn peri�dico
    private float spawnTimer;

    void Start()
    {
        // Realiza o spawn inicial dos itens no ch�o
        if (initialSpawnArea != null)
        {
            SpawnInitialItems();
        }
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

    // Spawn peri�dico � instancia itens de forma aleat�ria na �rea de spawn definida.
    void TrySpawnItem()
    {
        // Se j� houver o n�mero m�ximo de itens na cena, n�o spawna nada.
        GameObject[] currentItems = GameObject.FindGameObjectsWithTag("SpawnedItem");
        if (currentItems.Length >= maxItems)
        {
            Debug.Log("Limite de itens atingido (spawn peri�dico).");
            return;
        }

        if (spawnArea == null)
        {
            Debug.LogWarning("�rea de Spawn n�o definida!");
            return;
        }

        Vector3 randomPosition = GetRandomPositionInCollider(spawnArea);
        GameObject prefabToSpawn = SelectPrefabByChance();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Nenhum prefab selecionado para spawn.");
            return;
        }

        GameObject spawnedItem = Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
        spawnedItem.tag = "SpawnedItem";
        Debug.Log("Spawned (peri�dico): " + spawnedItem.name + " at " + randomPosition);

        // Aplica um impulso aleat�rio para evitar empilhamento muito r�gido
        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomImpulse = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            rb.AddForce(randomImpulse, ForceMode.Impulse);
        }
    }

    // Spawn inicial � instancia itens na �rea inicial (pr�ximo ao ch�o) at� atingir o limite.
    void SpawnInitialItems()
    {
        List<Vector3> chosenPositions = new List<Vector3>();
        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = maxItems * 10; // para evitar loop infinito se n�o encontrar posi��es v�lidas

        while (spawnedCount < maxItems && attempts < maxAttempts)
        {
            attempts++;
            Vector3 candidatePos = GetRandomPositionInCollider(initialSpawnArea);

            // Verifica se a posi��o candidata est� distante o suficiente dos itens j� posicionados
            bool tooClose = false;
            foreach (Vector3 pos in chosenPositions)
            {
                if (Vector3.Distance(candidatePos, pos) < minDistanceBetweenInitialSpawnItems)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                chosenPositions.Add(candidatePos);
                GameObject prefabToSpawn = SelectPrefabByChance();
                if (prefabToSpawn != null)
                {
                    GameObject spawnedItem = Instantiate(prefabToSpawn, candidatePos, Quaternion.identity);
                    spawnedItem.tag = "SpawnedItem";
                    Debug.Log("Initial Spawned: " + spawnedItem.name + " at " + candidatePos);
                }
                spawnedCount++;
            }
        }
    }

    // Retorna uma posi��o aleat�ria dentro dos limites do BoxCollider fornecido.
    Vector3 GetRandomPositionInCollider(BoxCollider col)
    {
        Vector3 min = col.bounds.min;
        Vector3 max = col.bounds.max;
        Vector3 randPos = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
        return randPos;
    }

    // Seleciona um prefab com base em chances ponderadas.
    GameObject SelectPrefabByChance()
    {
        float totalChance = 0f;
        foreach (var sp in spawnablePrefabs)
        {
            totalChance += sp.spawnChance;
        }

        if (totalChance <= 0)
            return null;

        float randomValue = Random.Range(0f, totalChance);
        float cumulative = 0f;
        foreach (var sp in spawnablePrefabs)
        {
            cumulative += sp.spawnChance;
            if (randomValue <= cumulative)
                return sp.prefab;
        }
        return null;
    }
}
