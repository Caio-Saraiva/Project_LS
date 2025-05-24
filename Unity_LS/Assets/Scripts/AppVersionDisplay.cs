using UnityEngine;
using TMPro;

public class AppVersionDisplay : MonoBehaviour
{
    [Header("Referência do Campo de Texto")]
    public TextMeshProUGUI versionText;

    private void Start()
    {
        if (versionText != null)
        {
            // Preenche o campo de texto com a versão do aplicativo
            versionText.text = $"ver. {Application.version}";
        }
        else
        {
            Debug.LogWarning("Campo TextMeshProUGUI não atribuído no Inspector.");
        }
    }
}
