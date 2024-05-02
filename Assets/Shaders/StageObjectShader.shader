Shader "Unlit/StageObjectShader"
{
    Properties
    {
		[Enum(Off, 0, On, 1)]
        _ZWrite("ZWrite", float) = 1
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull("Cull", Float) = 2
		_FloorTex("_FloorTex", 2D) = "black" { }
		_WallTex("_WallTex", 2D) = "black" { }
		_BGTex("_BGTex", 2D) = "black" { }

    }

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float floordot : DOT0;
		float bgdot : DOT1;
	};

	sampler2D _FloorTex;
	sampler2D _WallTex;
	sampler2D _BGTex;
	float4 _FloorTex_ST;
	float4 _WallTex_ST;
	float4 _BGTex_ST;


	ENDCG

    SubShader
    {
        Cull [_Cull]
		ZWrite [_ZWrite]

        Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //頂点をMVP行列変換
				o.uv = TRANSFORM_TEX(v.uv, _FloorTex); //テクスチャスケールとオフセットを加味

				float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal)); //モデル座標系の法線をビュー座標系に変換
				o.bgdot = dot(norm,normalize(float3(0,0,1)));
				o.floordot = dot(norm,normalize(float3(0,1,0)));
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 c; //UVをもとにテクスチャカラーをサンプリング

				float intensity = step(0.7, i.floordot);
				c = tex2D(_FloorTex, i.uv);

				if(i.bgdot > 0.7)
				{
					c = tex2D(_BGTex, i.uv);
				}
				else if(i.floordot > 0.7)
				{
					c = tex2D(_FloorTex, i.uv);
				}
				else
				{
					c = tex2D(_WallTex, i.uv);
				}
				return c;
			}
			ENDCG
		}
    }
}
