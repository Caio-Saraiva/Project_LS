using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Chave de Tradução")]
    public string key; // chave usada para buscar o texto no dicionário

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Sempre que o objeto ativar (incluindo no início), atualiza o texto
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("LocalizationManager não encontrado ao tentar atualizar texto!");
            return;
        }

        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        string localizedValue = LocalizationManager.Instance.GetLocalizedValue(key);
        textMeshPro.text = localizedValue;
    }
}
