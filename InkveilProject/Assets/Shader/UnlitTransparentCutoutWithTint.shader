Shader "Inkveil/UnlitTransparentCutoutWithTint"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}         // Ö÷ÌùÍ¼
        _ColorTint ("Color Tint", Color) = (1,1,1,1)       // µþ¼ÓÑÕÉ«
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5         // ¼ôÇÐãÐÖµ
    }
    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite On
            AlphaTest Greater [_Cutoff]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ColorTint;
            float _Cutoff;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                clip(texColor.a - _Cutoff);
                return texColor * _ColorTint;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent Cutout"
}
