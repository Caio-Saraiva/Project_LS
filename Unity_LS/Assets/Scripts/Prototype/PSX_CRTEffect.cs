using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PSX_CRTEffect : MonoBehaviour
{
    [Header("Passo 1: PSX")]
    [Tooltip("Material com shader Hidden/PSX")]
    public Material psxMaterial;

    [Header("Passo 2: CRT")]
    [Tooltip("Material com shader Hidden/CRT")]
    public Material crtMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // PASSO 1: pixelizar e quantizar
        int w = src.width / 4;
        int h = src.height / 4;
        RenderTexture tmp = RenderTexture.GetTemporary(w, h, 0, src.format);
        tmp.filterMode = FilterMode.Point;
        Graphics.Blit(src, tmp);                  // baixa resolução
        Graphics.Blit(tmp, dest, psxMaterial);    // aplica PSX
        RenderTexture.ReleaseTemporary(tmp);

        // PASSO 2: scanlines, vignette, aberration
        RenderTexture tmp2 = RenderTexture.GetTemporary(dest.width, dest.height, 0, src.format);
        Graphics.Blit(dest, tmp2, crtMaterial);
        Graphics.Blit(tmp2, dest);
        RenderTexture.ReleaseTemporary(tmp2);
    }
}
