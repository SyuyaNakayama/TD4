Shader "Unlit/Character"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Float) = 2
        [Enum(Off, 0, On, 1)]
        _ZWrite("ZWrite", float) = 1
        _ZWriteAlpha("_ZWriteAlpha", float) = 0.5
        [Enum(Off, 0, On, 1)]
        _ZWriteLessAlpha("ZWriteLessAlpha", float) = 0
        _MainTex ("Texture", 2D) = "black" {}
        _Overlay ("Overlay", 2D) = "black" {}
        _MainAlpha("MainAlpha",Range(0,1)) = 1
        _AddColor("AddColor", Color) = (1,1,1,0)
    }
    SubShader
    {
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        //半透明
        Pass
        {
            Cull [_Cull]
            ZTEST LEqual
            ZWrite [_ZWriteLessAlpha]

            Stencil
            {
                Ref -1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
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
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv) * i.color;
                clip(1 - texcol.a - _ZWriteAlpha);
                fixed4 col = texcol * (1 - _AddColor.w) + (_AddColor) * _AddColor.w;
                col.a = texcol.a * _MainAlpha;
                return col;
            }
            ENDCG
        }

        //不透明
        Pass
        {
            Cull [_Cull]
            ZTEST LEqual
            ZWrite [_ZWrite]

            Stencil
            {
                Ref -1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
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
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv) * i.color;
                clip(texcol.a - _ZWriteAlpha);
                fixed4 col = texcol * (1 - _AddColor.w) + (_AddColor) * _AddColor.w;
                col.a = texcol.a * _MainAlpha;
                return col;
            }
            ENDCG
        }

        //半透明
        Pass
        {
            Cull Back
            ZTEST Always
            ZWrite Off

            Stencil
            {
                Ref -1
                Comp Equal
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _Overlay;
            float4 _Overlay_ST;
            half4 _AddColor;
            float _MainAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Overlay);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_Overlay, i.uv);

                fixed4 col = texcol * i.color * (1 - _AddColor.w) + (_AddColor) * _AddColor.w;
                col.a = texcol.a * _MainAlpha;
                return col;
            }
            ENDCG
        }

        //ステンシルを消す
        Pass
        {
            Cull [_Cull]
            ZTEST Always
            ZWrite Off

            Stencil
            {
                Ref 0
                Comp Always
                Pass Replace
            }

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
                o.uv = (0,0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return (0,0,0,0);
            }
            ENDCG
        }
    }
}