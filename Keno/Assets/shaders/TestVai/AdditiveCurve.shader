// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Vai/Mobile/Particles/AdditiveCurve" {
	Properties {
	    _MainTex ("Particle Texture", 2D) = "white" {}
	    
	    _QOffset ("Offset", Vector) = (0,0,0,0)
		_Brightness ("Brightness", Float) = 0.0
		_Dist ("Distance", Float) = 100.0
		
		_CurveSensitivity	("CurveSensitivity", Float) = 0.001
		_CameraOffset	("CameraOffset (XY)", Float) = 20
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
//		Blend SrcAlpha One
//		Blend OneMinusDstColor one
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Color (0,0,0,0) }

		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader {
			Pass {
//				Blend SrcAlpha OneMinusSrcAlpha
				Blend SrcAlpha OneMinusSrcColor
//				Blend DstColor SrcColor
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"
		         
		         
					sampler2D _MainTex;
					float4 _QOffset;
					float _Dist;
					float _Brightness;
					float _CurveSensitivity;
					float _CameraOffset;
		         
		         	float4 _MainTex_ST;
		         	
					struct v2f {
						float4 pos : SV_POSITION;
						half2 uv : TEXCOORD0;
						float3 viewDir : TEXCOORD1;
						fixed4 color : COLOR;
					};
					
					v2f vert (appdata_full v)
					{
						v2f o;
						float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
		//				_Dist = (_Dist+_CameraOffset) * (_Dist+_CameraOffset);
		//				float zOff = vPos.z/_Dist;
		//				vPos += _QOffset * zOff * zOff * _CurveSensitivity;
						
						_Dist = (vPos.z+_CameraOffset) * (vPos.z+_CameraOffset);
						vPos += _CurveSensitivity * _QOffset * _Dist;
						o.pos = mul (UNITY_MATRIX_P, vPos);
						o.color = v.color;
						o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);		//v.texcoord;
						return o;
					}

					half4 frag (v2f i) : COLOR0
					{
						half4 col;
						half4 tex = tex2D(_MainTex, i.uv.xy) * i.color * _Brightness;
						
		                col.rgb = i.color.rgb * tex.rgb;
		                col.a = i.color.a * tex.a;
				
//						col.a = tex2D(_MainTex, i.color);
//						col *= UNITY_LIGHTMODEL_AMBIENT*_Brightness;		//Removed For NOT USING LIGHTING
						return col;
		        	}

		        ENDCG
//				SetTexture [_MainTex] {
//				combine texture * primary
//				}
			}
		}
	}
}