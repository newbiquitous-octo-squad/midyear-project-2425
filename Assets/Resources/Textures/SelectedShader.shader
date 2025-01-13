Shader "Custom/BorderShader"
{
    Properties
    {
        _BorderColor ("Border Color", Color) = (80,80,80,1)
        _BorderWidth ("Border Width", Range(0, 0.5)) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _BorderColor;
            float _BorderWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the border
                float2 uv = i.uv;
                float border = step(_BorderWidth, uv.x) * step(_BorderWidth, uv.y) *
                               step(uv.x, 1.0 - _BorderWidth) * step(uv.y, 1.0 - _BorderWidth);

                // If inside the border, make it transparent
                if (border > 0)
                    discard;

                return _BorderColor;
            }
            ENDCG
        }
    }
}