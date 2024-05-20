Shader "Unlit/Outline"
{
    Properties
    {
        _OutlineMap ("OutLineMap", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)
        _OutlineWidth("OutlineWidth", float) = 0.006
    }
    SubShader
    {
        CGINCLUDE
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        half4 MakeOutlineVertex(half4 locPos, half3 locNor, float width)
        {
            locPos = mul(UNITY_MATRIX_M,half4(locPos.xyz,1.0));
            float depth = UNITY_Z_0_FAR_FROM_CLIPSPACE(UnityWorldToClipPos(locPos).z);
            locPos = mul(UNITY_MATRIX_V,half4(locPos.xyz,1.0));
            locNor = mul(UNITY_MATRIX_V,half4(UnityObjectToWorldNormal(locNor),0)).xyz;
            locNor = normalize(locNor) * width * depth;
            locPos += half4(locNor, 0);
            return mul(UNITY_MATRIX_P,half4(locPos.xyz,1.0));
        }

        half4 MakeShiftedVertex(half4 locPos, half3 ShiftVec, float width)
        {
            locPos = mul(UNITY_MATRIX_M,half4(locPos.xyz,1.0));
            float depth = UNITY_Z_0_FAR_FROM_CLIPSPACE(UnityWorldToClipPos(locPos).z);
            locPos = mul(UNITY_MATRIX_V,half4(locPos.xyz,1.0));
            ShiftVec = normalize(ShiftVec) * width * depth;
            locPos += half4(ShiftVec, 0);
            return mul(UNITY_MATRIX_P,half4(locPos.xyz,1.0));
        }
        ENDCG

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Tags { "RenderType"="TransParent" }

        //モデルの形に合わせてステンシル
        Pass
        {
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
                float2 uv : TEXCOORD0;
            };

            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.a - 0.7);
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
        
        //輪郭
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeOutlineVertex(v.vertex, v.normal, _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭(収縮方向)
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeOutlineVertex(v.vertex, v.normal, -_OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭（上）
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(0,1,0), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭(下)
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(0,-1,0), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭（右）
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(1,0,0), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭(左)
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(-1,0,0), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭（奥）
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(0,0,1), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //輪郭(手前)
        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
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
                float2 uv : TEXCOORD0;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            sampler2D _OutlineMap;
            float4 _OutlineMap_ST;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = MakeShiftedVertex(v.vertex, half3(0,0,-1), _OutlineWidth);

                o.uv = TRANSFORM_TEX(v.uv, _OutlineMap);

				return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_OutlineMap, i.uv);
                clip(c.r*c.a - 0.7);
                return _OutlineColor;
            }
            ENDCG
        }

        //ステンシル上塗り
        Pass
        {
            ZTest Always
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
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            half _OutlineWidth;

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
