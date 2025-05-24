// Assets/Shaders/PSX.shader
Shader "Hidden/PSX"
{
    Properties { _MainTex ("Tela", 2D) = "white" {} }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 screen = _ScreenParams.xy;
                // pixelizar 4×
                float2 uv = floor(i.uv * screen / 4.0) * (4.0 / screen);
                fixed4 col = tex2D(_MainTex, uv);
                // quantização 4 bits
                col.rgb = floor(col.rgb * 15.0) / 15.0;
                // dithering simples
                float d = (frac(sin(dot(i.uv, float2(12.9898,78.233))) * 43758.5453) - 0.5) / 255.0;
                col.rgb += d;
                return col;
            }
            ENDCG
        }
    }
}
