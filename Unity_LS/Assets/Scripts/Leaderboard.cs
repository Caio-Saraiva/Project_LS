using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct LeaderboardEntry
{
    public string name;
    public int score;
}

public class Leaderboard : MonoBehaviour
{
    [Header("UI Prefab & Parent")]
    public GameObject entryPrefab;
    public Transform entriesParent;

    [Header("Storage Settings")]
    public string nameKeyPrefix = "Leaderboard_Name_";
    public string scoreKeyPrefix = "Leaderboard_Score_";
    public int maxEntries = 5;

    [Header("New Score Input")]
    public TextMeshProUGUI[] nameSlots = new TextMeshProUGUI[3];
    public TextMeshProUGUI pointsText;

    [Header("Debug / Reset")]
    [Tooltip("Se verdadeiro, limpa todos os dados de prefs ao iniciar")]
    public bool clearOnStart = false;

    List<LeaderboardEntry> _entries = new List<LeaderboardEntry>();

    void Start()
    {
        if (clearOnStart)
            ClearAllData();
        // não chama Load() automaticamente
    }

    /// <summary>
    /// Carrega do PlayerPrefs e reconstrói a UI. Chame manualmente.
    /// </summary>
    public void Load()
    {
        _entries.Clear();
        for (int i = 0; i < maxEntries; i++)
        {
            string nk = nameKeyPrefix + i;
            string sk = scoreKeyPrefix + i;
            if (PlayerPrefs.HasKey(nk) && PlayerPrefs.HasKey(sk))
            {
                _entries.Add(new LeaderboardEntry
                {
                    name = PlayerPrefs.GetString(nk),
                    score = PlayerPrefs.GetInt(sk)
                });
            }
            else break;
        }

        // Limpa filhos antigos (usando Destroy(), não DestroyImmediate)
        for (int i = entriesParent.childCount - 1; i >= 0; i--)
            Destroy(entriesParent.GetChild(i).gameObject);

        // Recria exatamente maxEntries linhas
        for (int i = 0; i < maxEntries; i++)
        {
            var go = Instantiate(entryPrefab, entriesParent);
            var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
            TextMeshProUGUI posT = null, nameT = null, scoreT = null;
            foreach (var t in texts)
            {
                var n = t.name.ToLower();
                if (n.Contains("position")) posT = t;
                else if (n.Contains("name")) nameT = t;
                else if (n.Contains("score")) scoreT = t;
            }

            posT?.SetText((i + 1).ToString("00") + ".");
            if (i < _entries.Count)
            {
                nameT?.SetText(_entries[i].name);
                scoreT?.SetText(_entries[i].score.ToString());
            }
            else
            {
                nameT?.SetText("---");
                scoreT?.SetText("0");
            }
        }
    }

    /// <summary>
    /// Grava uma nova entrada nos PlayerPrefs. Não atualiza a UI automaticamente.
    /// </summary>
    public void SaveScore()
    {
        // Monta a sigla
        string sigla = "";
        foreach (var slot in nameSlots)
        {
            string t = (slot != null ? slot.text : " ").PadRight(1);
            sigla += t.Substring(0, 1);
        }
        sigla = sigla.ToUpper();

        if (pointsText == null || !int.TryParse(pointsText.text, out int pts))
        {
            Debug.LogWarning("Leaderboard: pointsText inválido.");
            return;
        }

        // Carrega existentes
        var list = new List<LeaderboardEntry>();
        for (int i = 0; i < maxEntries; i++)
        {
            string nk = nameKeyPrefix + i;
            string sk = scoreKeyPrefix + i;
            if (PlayerPrefs.HasKey(nk) && PlayerPrefs.HasKey(sk))
                list.Add(new LeaderboardEntry
                {
                    name = PlayerPrefs.GetString(nk),
                    score = PlayerPrefs.GetInt(sk)
                });
            else break;
        }

        // Adiciona, ordena e limita
        list.Add(new LeaderboardEntry { name = sigla, score = pts });
        list.Sort((a, b) => b.score.CompareTo(a.score));
        if (list.Count > maxEntries)
            list.RemoveRange(maxEntries, list.Count - maxEntries);

        // Grava
        for (int i = 0; i < list.Count; i++)
        {
            PlayerPrefs.SetString(nameKeyPrefix + i, list[i].name);
            PlayerPrefs.SetInt(scoreKeyPrefix + i, list[i].score);
        }
        for (int i = list.Count; i < maxEntries; i++)
        {
            PlayerPrefs.DeleteKey(nameKeyPrefix + i);
            PlayerPrefs.DeleteKey(scoreKeyPrefix + i);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Limpa todas as chaves do leaderboard no PlayerPrefs.
    /// </summary>
    public void ClearAllData()
    {
        for (int i = 0; i < maxEntries; i++)
        {
            PlayerPrefs.DeleteKey(nameKeyPrefix + i);
            PlayerPrefs.DeleteKey(scoreKeyPrefix + i);
        }
        PlayerPrefs.Save();
    }
}
