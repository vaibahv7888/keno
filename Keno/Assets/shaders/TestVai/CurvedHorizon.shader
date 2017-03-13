// Curve shader
Shader"Vai/CurvedHorizon" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_QOffset ("Offset", Vector) = (-40,0,0,0)
		_Brightness ("Brightness", Float) = 2.48
		_Dist ("Distance", Float) = 140.0
	}

	SubShader {
	Tags { "Queue" = "Transparent"}
		Pass
		{

			
			CGPROGRAM
			Blend SrcAlpha OneMinusSrcAlpha
			
			#pragmavertexvert
			#pragmafragmentfrag
			#include "UnityCG.cginc"



			sampler2D _MainTex;
			float4 _QOffset;
			float _Dist;
			float _Brightness;

			structv2f {
				float4pos : SV_POSITION;
				float4uv : TEXCOORD0;
				float3viewDir : TEXCOORD1;
				fixed4color : COLOR;
			};

			v2fvert (appdata_fullv)
			{
				v2fo;
				float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
				float zOff = vPos.z/_Dist;
				vPos += _QOffset*zOff*zOff;
				o.pos = mul (UNITY_MATRIX_P, vPos);
				o.uv = v.texcoord;
				return o;
			}
			
			half4frag (v2fi) : COLOR0
			{
				half4 col = tex2D(_MainTex, i.uv.xy);// * _Brightness;
//				col *= UNITY_LIGHTMODEL_AMBIENT*_Brightness;
				return col;
			}
			
			ENDCG
		}
	}

	FallBack"Diffuse"
}