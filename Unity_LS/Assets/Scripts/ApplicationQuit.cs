using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
    /// <summary>
    /// Fecha a aplicação de acordo com a plataforma:
    /// - Editor: sai do modo Play
    /// - Standalone (Windows/Linux): fecha o executável
    /// - Android: fecha o app
    /// - WebGL: não é possível fechar a janela do navegador; apenas loga
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Para o modo Play no Editor
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE
            // Executável desktop
            Application.Quit();

#elif UNITY_ANDROID
            // App Android
            Application.Quit();

#elif UNITY_WEBGL
            // WebGL não permite fechar a aba; apenas avisa no console
            Debug.Log("WebGL: QuitGame() chamado, mas não é possível fechar a janela do navegador.");

#else
            // Outras plataformas: tenta fechar
            Application.Quit();
#endif
    }
}
