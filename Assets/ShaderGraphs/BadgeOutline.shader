
Shader "UI/BadgeOutline"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _FillColor ("Fill Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,1,1,1)
        _BorderThickness ("Border Thickness", Range(0,1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _FillColor;
            float4 _BorderColor;
            float _BorderThickness;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).a;

                float fill = smoothstep(1.0 - _BorderThickness, 1.0, alpha);
                float expanded = smoothstep(1.0 - _BorderThickness * 2.0, 1.0, alpha);
                float border = expanded - fill;

                float4 finalColor = _FillColor * fill + _BorderColor * border;
                finalColor.a = alpha;
                return finalColor;
            }
            ENDCG
        }
    }
}
