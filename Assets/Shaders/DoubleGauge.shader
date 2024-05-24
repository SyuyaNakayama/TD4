Shader "Unlit/DoubleGauge"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackGroundColor("BackGroundColor", Color) = (0, 0, 0, 1)
        _EdgeColor("EdgeColor", Color) = (1, 1, 1, 1)
        _EdgeWidth("EdgeWidth", Range(0,0.5)) = 0.05
        _GaugeColor1("GaugeColor1", Color) = (0, 0, 1, 1)
        _FillAmount1("FillAmount1", Range(0,1)) = 1
        _GaugeColor2("GaugeColor2", Color) = (1, 0, 0, 1)
        _FillAmount2("FillAmount2", Range(0,1)) = 1
        _AddColor("AddColor", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

        Cull Off
		Lighting Off
        ZTest Always
		Blend One OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 _BackGroundColor;
            half4 _EdgeColor;
            half _EdgeWidth;
            half4 _GaugeColor1;
            half _FillAmount1;
            half4 _GaugeColor2;
            half _FillAmount2;
            half4 _AddColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv);
                float intensity = saturate(step(i.uv.x, _EdgeWidth)
                 - ((1 - step(i.uv.x, _EdgeWidth / 2)) * step(0.5, i.uv.y)))
                 + (1 - step(i.uv.x, 1 - _EdgeWidth));
                fixed4 col = _EdgeColor * intensity + _BackGroundColor * (1 - intensity);
                float gaugelength = 1 - _EdgeWidth * 2;
                float gaugeamount = _FillAmount2 * gaugelength;
                intensity = (step(_EdgeWidth, i.uv.x) * step(i.uv.x, _EdgeWidth + gaugeamount));
                col = _GaugeColor2 * intensity + col * (1 - intensity);
                gaugeamount = _FillAmount1 * gaugelength;
                intensity = (step(_EdgeWidth, i.uv.x) * step(i.uv.x, _EdgeWidth + gaugeamount));
                col = _GaugeColor1 * intensity + col * (1 - intensity);

                col.rgb = _AddColor.rgb * _AddColor.a + col.rgb * (1 - _AddColor.a);

                col.a *= texcol.a;
                return col;
            }
            ENDCG
        }
    }
}
