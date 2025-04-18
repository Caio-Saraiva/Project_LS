using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelateFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelateSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material pixelateMaterial = null;
    }

    public PixelateSettings settings = new PixelateSettings();

    class PixelatePass : ScriptableRenderPass
    {
        Material mat;
        int tempRT = Shader.PropertyToID("_PixelateTemp");
        RTHandle sourceHandle;

        public PixelatePass(Material m)
        {
            mat = m;
        }

        public void Setup(RTHandle src)
        {
            sourceHandle = src;
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData data)
        {
            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Pixelate");
            int w = data.cameraData.camera.scaledPixelWidth;
            int h = data.cameraData.camera.scaledPixelHeight;

            // RT temporário (point filtering por ser pixel)
            cmd.GetTemporaryRT(tempRT, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            // Blit aplicando pixelate
            cmd.Blit(sourceHandle, tempRT, mat);
            // Blit de volta
            cmd.Blit(tempRT, sourceHandle);
            cmd.ReleaseTemporaryRT(tempRT);

            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    PixelatePass pass;

    public override void Create()
    {
        pass = new PixelatePass(settings.pixelateMaterial)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        pass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(pass);
    }
}
