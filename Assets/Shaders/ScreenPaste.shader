// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ScreenPaste"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Float) = 2
        [Enum(Off, 0, On, 1)]
        _OffsetObjectPosition("OffsetObjectPosition", Float) = 1
        _MainTex ("Texture", 2D) = "white" {}
        _MainAlpha("MainAlpha",Range(0,1)) = 1
        _MinDepth("MinDepth",Range(0,1)) = 0.017
        _MaxDepth("MaxDepth",Range(0,1)) = 0.022
    }
    SubShader
    {
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull [_Cull]
            ZTEST LEqual
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                half depth : TEXCOORD1;
                float4 objpos : TEXCOORD2;
            };

            float _OffsetObjectPosition;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MainAlpha;
            float _MinDepth;
            float _MaxDepth;


            v2f vert(appdata v, float4 pos : POSITION ,out float4 wpos : SV_POSITION ,  float2 uv : TEXCOORD0) {
                v2f o;
                o.uv = uv;
                wpos = UnityObjectToClipPos(pos);

                COMPUTE_EYEDEPTH(o.depth);
                o.depth *= _ProjectionParams.w;

                o.objpos = ComputeScreenPos(UnityObjectToClipPos(float4(0, 0, 0, 1)));
                return o;
            }

            float4 frag(v2f i , UNITY_VPOS_TYPE vpos : VPOS) : SV_TARGET
            {
                vpos.xy /= _ScreenParams.xy;
                vpos.xy -= i.objpos.xy / max(i.objpos.w,0.5f) * _OffsetObjectPosition;

                fixed4 col = tex2D(_MainTex, vpos.xy * _MainTex_ST.xy + _MainTex_ST.zw);
                col.a = col.a * _MainAlpha * smoothstep(_MaxDepth, _MinDepth,i.depth);
                return col;
            }
            ENDCG
        }
    }
}