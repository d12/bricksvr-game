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

    // Add Dynamic OCC Later, Source: https://github.com/przemyslawzaworski/Unity-GPU-Based-Occlusion-Culling

    //SubShader
	//{
	//	Cull Off
	//	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	//	Pass
	//	{
	//		Blend SrcAlpha OneMinusSrcAlpha
	//		ZWrite Off
	//		CGPROGRAM
	//		#pragma vertex VSMain
	//		#pragma fragment PSMain
	//		#pragma target 5.0

	//		RWStructuredBuffer<float4> _Writer : register(u1);
	//		StructuredBuffer<float4> _Reader;
	//		int _Debug;

	//		float4 VSMain (float4 vertex : POSITION, out uint instance : TEXCOORD0, uint id : SV_VertexID) : SV_POSITION
	//		{
	//			instance = _Reader[id].w;
	//			return mul (UNITY_MATRIX_VP, float4(_Reader[id].xyz, 1.0));
	//		}

	//		[earlydepthstencil]
	//		float4 PSMain (float4 vertex : SV_POSITION, uint instance : TEXCOORD0) : SV_TARGET
	//		{
	//			_Writer[instance] = vertex;
	//			return float4(0.0, 0.0, 1.0, 0.2 * _Debug);
	//		}
	//		ENDCG
	//	}
	//}
}
