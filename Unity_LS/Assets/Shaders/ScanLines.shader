Shader "UI/ScanLines"
{
    Properties
    {
        _MainTex    ("Texture",       2D)    = "white" {}
        _LineColor  ("Line Color",    Color) = (0,1,0,1)
        _Thickness  ("Thickness (px)", Float) = 1
        _Speed      ("Speed",         Float) = 10
        _Intensity  ("Intensity",     Range(0,1)) = 0.3
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "DisableBatching"="True" 
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            // usa o vert_img/v2f_img já prontos para UI
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4   _MainTex_ST;
            fixed4   _LineColor;
            float    _Thickness;
            float    _Speed;
            float    _Intensity;

            fixed4 frag(v2f_img IN) : SV_Target
            {
                // pega cor base da UI
                fixed4 col = tex2D(_MainTex, IN.uv);

                // calcula posição Y em pixels + deslocamento (_Time.y é o tempo em s)
                float pos = IN.uv.y * _ScreenParams.y + _Time.y * _Speed;

                // a cada Thickness*2 pixels, pinta uma linha
                if (fmod(pos, _Thickness * 2.0) < _Thickness)
                    col.rgb = lerp(col.rgb, _LineColor.rgb, _Intensity);

                return col;
            }
            ENDCG
        }
    }
}
