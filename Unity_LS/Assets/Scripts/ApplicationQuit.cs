using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
    /// <summary>
    /// Fecha a aplica��o de acordo com a plataforma:
    /// - Editor: sai do modo Play
    /// - Standalone (Windows/Linux): fecha o execut�vel
    /// - Android: fecha o app
    /// - WebGL: n�o � poss�vel fechar a janela do navegador; apenas loga
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Para o modo Play no Editor
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE
            // Execut�vel desktop
            Application.Quit();

#elif UNITY_ANDROID
            // App Android
            Application.Quit();

#elif UNITY_WEBGL
            // WebGL n�o permite fechar a aba; apenas avisa no console
            Debug.Log("WebGL: QuitGame() chamado, mas n�o � poss�vel fechar a janela do navegador.");

#else
            // Outras plataformas: tenta fechar
            Application.Quit();
#endif
    }
}
