// Assets/Shaders/CRT.shader
Shader "Hidden/CRT"
{
    Properties { _MainTex("Tela", 2D) = "white" {} }
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
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 screen = _ScreenParams.xy;
                // scanlines
                float scan = sin(i.uv.y * screen.y * 1.5) * 0.04;
                col.rgb -= scan;
                // vignette
                float2 uv = i.uv * 2 - 1;
                float vig = dot(uv, uv);
                col.rgb *= smoothstep(1.0, 0.5, vig);
                // aberração cromática
                float2 off = float2(0.002, 0);
                float r = tex2D(_MainTex, i.uv + off).r;
                float g = col.g;
                float b = tex2D(_MainTex, i.uv - off).b;
                return fixed4(r, g, b, 1);
            }
            ENDCG
        }
    }
}
