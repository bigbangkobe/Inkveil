
Shader "Custom/TwirlPortal_BuiltinEquivalent"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Mask", 2D) = "gray" {}
        _TwirlStrength ("Twirl Strength", Range(0, 10)) = 3.0
        _TwirlSpeed ("Twirl Speed", Range(-5, 5)) = 1.0
        _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0.0, 0.5)) = 0.1
        _Threshold ("Alpha Threshold", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        sampler2D _AlphaTex;
        float _TwirlStrength;
        float _TwirlSpeed;
        float _Threshold;
        float4 _EdgeColor;
        float _EdgeWidth;

        struct Input
        {
            float2 uv_MainTex;
        };

        float2 TwirlUV(float2 uv, float strength, float time)
        {
            float2 center = float2(0.5, 0.5);
            float2 offset = uv - center;
            float dist = length(offset);
            float angle = strength * dist + time;
            float cosA = cos(angle);
            float sinA = sin(angle);
            float2 rotated = float2(
                offset.x * cosA - offset.y * sinA,
                offset.x * sinA + offset.y * cosA
            );
            return rotated + center;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Time.y * _TwirlSpeed;
            float2 uv = TwirlUV(IN.uv_MainTex, _TwirlStrength, time);

            float4 mainCol = tex2D(_MainTex, uv);
            float alphaVal = tex2D(_AlphaTex, IN.uv_MainTex).r;

            float diff = alphaVal - _Threshold;
            float edge = smoothstep(0, _EdgeWidth, diff);
            float alpha = step(_Threshold, alphaVal);

            o.Albedo = lerp(_EdgeColor.rgb, mainCol.rgb, edge);
            o.Alpha = alpha;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
