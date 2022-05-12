Shader "Custom/SimpleOutline"
{
    Properties
    {
        [HDR] _OutlineColor("OutlineColor", Color) = (0.0, 0.0, 0.0, 1.0)
        _OutlineThickness("Outline Thickness", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            fixed4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineEnabled;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness * _OutlineEnabled);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
