Shader "Custom/FadeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeAmount ("Fade Amount", Range(0, 1)) = 0.5
        _StencilRef ("Stencil Reference", Range(0, 255)) = 1
        _StencilComp ("Stencil Comparison", Float) = 8 // Always
        _StencilOp ("Stencil Operation", Float) = 0 // Keep
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilOp]
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _FadeAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float fade = smoothstep(0.0, _FadeAmount, i.uv.x);
                col.a *= fade; // 알파값을 좌우에 따라 조절
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

