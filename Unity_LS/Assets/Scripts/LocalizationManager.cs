using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [Header("Arquivo de Localização (JSON)")]
    public TextAsset localizationJson; // Você seleciona o arquivo JSON no Inspector

    private Dictionary<string, string> localizedText = new Dictionary<string, string>();
    private Dictionary<string, Dictionary<string, string>> fullLocalization;

    private string currentLanguage = "en";

    private const string LanguagePrefKey = "SelectedLanguage";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (localizationJson != null)
            {
                LoadFullLocalization();
            }

            if (PlayerPrefs.HasKey(LanguagePrefKey))
            {
                string savedLanguage = PlayerPrefs.GetString(LanguagePrefKey);
                ChangeLanguage(savedLanguage);
            }
            else
            {
                ChangeLanguage(currentLanguage);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadFullLocalization()
    {
        // Usa Newtonsoft para desserializar
        fullLocalization = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(localizationJson.text);
    }

    public void ChangeLanguage(string language)
    {
        if (fullLocalization == null)
        {
            Debug.LogError("Arquivo de localização não carregado!");
            return;
        }

        if (!fullLocalization.ContainsKey(language))
        {
            Debug.LogError($"Idioma '{language}' não encontrado no JSON.");
            return;
        }

        localizedText = fullLocalization[language];

        currentLanguage = language;
        PlayerPrefs.SetString(LanguagePrefKey, currentLanguage);
        PlayerPrefs.Save();

        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        LocalizedText[] localizedTexts = FindObjectsOfType<LocalizedText>(true); // true para incluir objetos inativos

        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
            return localizedText[key];
        else
            return key;
    }
}
