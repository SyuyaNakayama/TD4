Shader "Unlit/ShadowShader"
{
    Properties
    {
        _ShadowColor("OutlineColor", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        ENDCG

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        //モデルの形に合わせてステンシル
        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM

            struct appdata
            {
                half4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
        
        //影
        Pass
        {
            Cull Off
            ZTest GEqual
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp Equal
            }

            CGPROGRAM

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            half4 _ShadowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _ShadowColor;
            }
            ENDCG
        }

        //ステンシル上塗り
        Pass
        {
            Cull Off
            ZTest Always
            ZWrite Off
            ColorMask 0

            Stencil
            {
                Ref 0
                Comp Always
                Pass Replace
            }

            CGPROGRAM

                        struct appdata
            {
                half4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
