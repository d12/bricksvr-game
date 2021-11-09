Shader "Normal/Examples/Floor" {
     Properties {
         _MainTex ("Base (RGB)", 2D) = "white" {}
     }
     SubShader {
         Tags {"Queue" = "Geometry" "IgnoreProjector"="True" "RenderType" = "Transparent"}
         Cull Off
		 Fog { Mode Global }
         ZWrite On
         Blend SrcAlpha OneMinusSrcAlpha
 
         Pass {
             Tags { "LightMode" = "ForwardBase" }
             CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag
                 #pragma multi_compile_fwdbase
				 #pragma multi_compile_fog", UNITY_FOG_COORDS, UNITY_TRANSFER_FOG, UNITY_APPLY_FOG
                 #pragma fragmentoption ARB_fog_exp2
                 #pragma fragmentoption ARB_precision_hint_fastest
                 
                 #include "UnityCG.cginc"
                 #include "AutoLight.cginc"
                 
                 struct v2f {
                     float4    pos            : SV_POSITION;
                     float2    uv            : TEXCOORD0;
                     LIGHTING_COORDS(1,2)
                 };
 
                 float4 _MainTex_ST;
 
                 v2f vert(appdata_tan v) {
                     v2f o;
                     
                     o.pos = UnityObjectToClipPos( v.vertex);
                     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
                     TRANSFER_VERTEX_TO_FRAGMENT(o);
                     return o;
                 }
 
                 sampler2D _MainTex;
 
                 fixed4 frag(v2f i) : COLOR {
                     fixed atten = SHADOW_ATTENUATION(i);
                     fixed4 tex = tex2D(_MainTex, i.uv);
                     fixed4 c = tex * atten;
                     c.a = tex.a;
                     return c;
                 }
             ENDCG
         }
          Pass {
             Tags {"LightMode" = "ForwardAdd"}
             Blend One One
             CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag
                 #pragma multi_compile_fwdadd_fullshadows
				 #pragma multi_compile_fog", UNITY_FOG_COORDS, UNITY_TRANSFER_FOG, UNITY_APPLY_FOG
                 #pragma fragmentoption ARB_fog_exp2
                 #pragma fragmentoption ARB_precision_hint_fastest
                 
                 #include "UnityCG.cginc"
                 #include "AutoLight.cginc"
                 
                 struct v2f {
                     float4    pos            : SV_POSITION;
                     float2    uv            : TEXCOORD0;
                     LIGHTING_COORDS(1,2)
                 };
 
                 float4 _MainTex_ST;
 
                 v2f vert (appdata_tan v) {
                     v2f o;
                     
                     o.pos = UnityObjectToClipPos( v.vertex);
                     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
                     TRANSFER_VERTEX_TO_FRAGMENT(o);
                     return o;
                 }
 
                 sampler2D _MainTex;
 
                 fixed4 frag(v2f i) : COLOR {
                     fixed atten = SHADOW_ATTENUATION(i);
                     fixed4 tex = tex2D(_MainTex, i.uv);
                     fixed4 c = tex * atten;
                     c.a = tex.a;
                     return c;
                 }
             ENDCG
         }
     }
     FallBack "VertexLit"
}