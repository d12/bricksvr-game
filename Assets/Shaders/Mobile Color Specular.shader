Shader "Mobile/Color Specular" {
Properties {
    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
    _Color("Color", Color) = (1,1,1,1)
}
SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass noforwardadd halfasview interpolateview
inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
    fixed diff = max (0, dot (s.Normal, lightDir));
    fixed nh = max (0, dot (s.Normal, halfDir));
    fixed spec = pow (nh, s.Specular*128) * s.Gloss;

    fixed4 c;
    c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
    UNITY_OPAQUE_ALPHA(c.a);
    return c;
}
half _Shininess;
fixed4 _Color;
struct Input {
    float3 pos;
};
void surf (Input IN, inout SurfaceOutput o) {
    o.Albedo = _Color.rgb;
    o.Gloss = _Color.a;
    o.Alpha = _Color.a;
    o.Specular = _Shininess;
}
ENDCG
}
FallBack "Mobile/VertexLit"
}