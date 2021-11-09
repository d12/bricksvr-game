Shader "Custom/PerInstanceColor"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _TexOffset ("Texture Offset", float) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular Color", Color) = (1,1,1,1)
        _ScratchWeighting ("Scratch Weighting", Range(0,1)) = 0.0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows
        // Use Shader model 3.0 target
        #pragma target 3.0
        sampler2D _MainTex;
        struct Input {
            float2 uv_MainTex;
        };
        half _Glossiness;
        half _ScratchWeighting;
        half3 _Specular;
        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float, _TexOffset)
        UNITY_INSTANCING_BUFFER_END(Props)
        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            float offset = (UNITY_ACCESS_INSTANCED_PROP(Props, _TexOffset));
            fixed4 tx = tex2D (_MainTex, (IN.uv_MainTex + float2(offset, -offset)));
            fixed4 c =  (UNITY_ACCESS_INSTANCED_PROP(Props, _Color) * (fixed4(1.0, 1.0, 1.0, 1.0) + (tx * _ScratchWeighting)));

            o.Albedo = c.rgb;
            o.Specular = _Specular;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
