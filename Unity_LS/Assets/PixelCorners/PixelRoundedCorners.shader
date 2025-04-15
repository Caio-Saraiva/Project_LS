Shader "PixelCorners/PixelRoundedCorners" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _WidthHeightRadiusDensity ("Rect: Width, Height, Radius, Density", Vector) = (100,100,40,10)
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            // Par�metro que cont�m: (largura, altura, raio, densidade)
            float4 _WidthHeightRadiusDensity;
            float4 _OuterUV;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                
                float width = _WidthHeightRadiusDensity.x;
                float height = _WidthHeightRadiusDensity.y;
                float radius = _WidthHeightRadiusDensity.z;
                float density = _WidthHeightRadiusDensity.w;

                // Converte a UV para espa�o em pixels do RectTransform
                float2 pixelPos = float2(i.texcoord.x * width, i.texcoord.y * height);
                bool inCorner = false;
                float2 localDiff = float2(0.0, 0.0);

                // Determina em qual canto estamos e calcula a diferen�a local em rela��o ao centro do arco
                if (pixelPos.x < radius && pixelPos.y < radius) {
                    // canto inferior-esquerdo
                    inCorner = true;
                    localDiff = float2(radius - pixelPos.x, radius - pixelPos.y);
                } else if (pixelPos.x > width - radius && pixelPos.y < radius) {
                    // canto inferior-direito
                    inCorner = true;
                    localDiff = float2(pixelPos.x - (width - radius), radius - pixelPos.y);
                } else if (pixelPos.x < radius && pixelPos.y > height - radius) {
                    // canto superior-esquerdo
                    inCorner = true;
                    localDiff = float2(radius - pixelPos.x, pixelPos.y - (height - radius));
                } else if (pixelPos.x > width - radius && pixelPos.y > height - radius) {
                    // canto superior-direito
                    inCorner = true;
                    localDiff = float2(pixelPos.x - (width - radius), pixelPos.y - (height - radius));
                }

                if (inCorner) {
                    // �Snap� a posi��o local a uma grade definida pela densidade
                    float2 cell = floor(localDiff / density);
                    float2 cellCenter = (cell + 0.5) * density;
                    // Se a dist�ncia do centro da c�lula � origem for maior que o raio, o pixel est� fora da �rea "pixelada" do canto
                    if (length(cellCenter) > radius) {
                        col.a = 0;
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
