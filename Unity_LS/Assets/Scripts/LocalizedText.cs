using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Chave de Tradu��o")]
    public string key; // chave usada para buscar o texto no dicion�rio

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Sempre que o objeto ativar (incluindo no in�cio), atualiza o texto
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("LocalizationManager n�o encontrado ao tentar atualizar texto!");
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
