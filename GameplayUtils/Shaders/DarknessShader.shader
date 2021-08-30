// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "AmoaebaUtils/DarknessShader"
{
    Properties
    {
        _outerColor("Outer Color", Color) = (0,0.5843137254901961,1,1)
        _innerColor("Inner Color", Color) = (0.1176470588235294,0,0.5882352941176471,1)
        _innerRadius("Inner Radius", Range(0, 1)) = 0.5
        _border("Fade Border", Range(0.00001, 1)) = 0.01
        _xOffset("XOffset", Range(-1,2)) = 0.5
        _yOffset("YOffset", Range(-1,2)) = 0.5
        _opacity("Opacity", Range(0,1)) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects

            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            float _border;

            float4 _outerColor;
            float4 _innerColor;
            float _innerRadius;
            float _xOffset;
            float _yOffset;
            float _opacity;

            struct v2f
            {
                float2 uv : TEXCOORD0;
            };

            v2f vert(
                float4 vertex : POSITION, // vertex position input
                float2 uv : TEXCOORD0, // texture coordinate input
                out float4 outpos : SV_POSITION // clip space position output
            )
            {
                v2f o;
                o.uv = uv;
                outpos = UnityObjectToClipPos(vertex);
                return o;
            }

            float2 antialias(float radius, float borderSize, float dist)
            {
                float t = smoothstep(radius + borderSize, radius - borderSize, dist);
                return t;
            }

            fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
                float4 col;
                float2 center = float2((_xOffset)*_ScreenParams.x,(_yOffset)*_ScreenParams.y); //_ScreenParams.xy / 2;

                float maxradius = length(_ScreenParams.xy / 2);

                float radius = maxradius*(_innerRadius);

                float dis = distance(screenPos.xy, center);

                float aliasVal = antialias(radius, _border*1000, dis);
                col = lerp(_outerColor, _innerColor, aliasVal);
                col.a = col.a * _opacity;
                return col;

            }
            ENDCG
        }
    }
}