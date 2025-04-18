Shader "Hidden/URP/Pixelate"
{
    Properties
    {
        _MainTex     ("Tela", 2D) = "white" {}
        _Downsample  ("Downsample Factor", Float) = 4.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4   _MainTex_TexelSize;
            float    _Downsample;

            fixed4 frag(v2f_img i) : SV_Target
            {
                // transforma UV para grid reduzido
                float2 screen = _ScreenParams.xy;
                float2 uv = i.uv;
                uv *= screen / _Downsample;
                uv = floor(uv) * (_Downsample / screen);
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
