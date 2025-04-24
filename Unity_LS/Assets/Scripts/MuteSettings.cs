using UnityEngine;

public class MuteSettings : MonoBehaviour
{
    [Header("Target & Materials")]
    [Tooltip("GameObject cujo material ser� trocado conforme o estado de mute")]
    public GameObject target;

    [Tooltip("Material usado quando o �udio estiver ativo (unmuted)")]
    public Material unmutedMaterial;

    [Tooltip("Material usado quando o �udio estiver mutado (muted)")]
    public Material mutedMaterial;

    bool _isMuted;

    void Awake()
    {
        // inicializa de acordo com o estado atual do AudioListener
        _isMuted = AudioListener.pause;
        ApplyState();
    }

    /// <summary>
    /// Alterna entre mute e unmute, pausa/despausa o �udio e troca o material do target.
    /// </summary>
    public void ToggleMute()
    {
        _isMuted = !_isMuted;
        AudioListener.pause = _isMuted;
        ApplyState();
    }

    void ApplyState()
    {
        if (target == null)
        {
            Debug.LogWarning("MuteSettings: 'target' n�o foi atribu�do.");
            return;
        }

        var rend = target.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("MuteSettings: GameObject alvo n�o possui Renderer.");
            return;
        }

        // troca o material conforme o estado
        rend.material = _isMuted ? mutedMaterial : unmutedMaterial;
    }
}
