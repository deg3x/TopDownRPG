Shader "Custom/UIAnimationShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _BgColor("Background Color", Color) = (0.1, 0.1, 0.1, 1)
        _FillAmount("Fill Amount", Range(0, 1)) = 1.0
        _AspectRatio("XY Size Ratio", float) = 1
        _BorderSize("Border Size", Range(0, 1)) = 0.5
        _BorderColor("Border Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                return o;
            }

            fixed4 _Color;
            fixed4 _BgColor;
            float _FillAmount;
            float _AspectRatio;
            float _BorderSize;
            fixed4 _BorderColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;

                float borderSize = _BorderSize * 0.2f;
                
                float borderLimX = borderSize / _AspectRatio;
                float borderLimY = borderSize;

                if (i.uv.x > borderLimX && i.uv.x < 1 - borderLimX && i.uv.y > borderLimY && i.uv.y < 1 - borderLimY)
                {
                    if (i.uv.x > _FillAmount && i.uv.x < _FillAmount + borderLimX)
                    {
                        col = _BorderColor;
                    }
                    else
                    {
                        col = i.uv.x <= _FillAmount ? _Color : _BgColor;
                    }
                }
                else
                {
                    col = _BorderColor;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
