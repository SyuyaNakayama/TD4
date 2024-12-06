Shader "Unlit/HiddenObject"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Float) = 2
        _MainTex ("Texture", 2D) = "black" {}
        _MainAlpha("MainAlpha",Range(0,1)) = 1
        _AddColor("AddColor", Color) = (1,1,1,0)
    }
    SubShader
    {
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull [_Cull]
            ZTEST Greater
            ZWrite Off

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
            half4 _AddColor;
            float _MainAlpha;
            float _ZWriteAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv);
                fixed4 col = texcol * (1 - _AddColor.w) + (_AddColor) * _AddColor.w;
                col.a = texcol.a * _MainAlpha;
                return col;
            }
            ENDCG
        }
    }
}