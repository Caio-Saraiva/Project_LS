using UnityEngine;
using TMPro;

public class AppVersionDisplay : MonoBehaviour
{
    [Header("Refer�ncia do Campo de Texto")]
    public TextMeshProUGUI versionText;

    private void Start()
    {
        if (versionText != null)
        {
            // Preenche o campo de texto com a vers�o do aplicativo
            versionText.text = $"ver. {Application.version}";
        }
        else
        {
            Debug.LogWarning("Campo TextMeshProUGUI n�o atribu�do no Inspector.");
        }
    }
}
