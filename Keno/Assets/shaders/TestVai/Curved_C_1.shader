Shader "Vai/Curved_C_1" { 
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_Color ("Color",Color) = (1,1,1,1)
		
		_QOffset ("Offset", Vector) = (0,0,0,0)
		_Brightness ("Brightness", Float) = 1.0
		_Dist ("Distance", Float) = 100.0
		
		_CurveSensitivity	("CurveSensitivity", Float) = 0.001
		_CameraOffset	("CameraOffset (XY)", Float) = 20
	}
	
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass
		{
         
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         
         
			sampler2D _MainTex;
			float4 _QOffset;
			float4 _Color;
			float _Dist;
			float _Brightness;
			float _CurveSensitivity;
			float _CameraOffset;
         
			struct v2f {
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD1;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
				
				//Orignal code for curve.
//				float zOff = vPos.z/_Dist;
//				vPos += _QOffset * zOff * zOff * _CurveSensitivity;

				if(vPos.z <= -_CameraOffset) {
					_Dist = (vPos.z+_CameraOffset) * (vPos.z+_CameraOffset);
					vPos += _CurveSensitivity * _QOffset * _Dist;
					
//					vPos += _CurveSensitivity * _QOffset * _Dist;
				} else {
					_Dist = (vPos.z+_CameraOffset) * (vPos.z+_CameraOffset);
					vPos.y += _CurveSensitivity * _QOffset.y * 2.0 * _Dist;

					_Dist = vPos.z * vPos.z;
//					vPos += -_CurveSensitivity * _QOffset * _Dist;

				}

//					_Dist = vPos.z * vPos.z;
//					vPos.x += _CurveSensitivity * _QOffset.x * _Dist;
				

				o.pos = mul (UNITY_MATRIX_P, vPos);
				o.uv = v.texcoord;
				return o;
			}
			
			half4 frag (v2f i) : COLOR0
			{
				half4 col = tex2D(_MainTex, i.uv.xy) * _Brightness * _Color;
//				col *= UNITY_LIGHTMODEL_AMBIENT*_Brightness;		//Removed For NOT USING LIGHTING
				return col;
        	}
        	ENDCG
//        	SetTexture [_MainTex] {
//				combine texture * primary
//				}
    	}
 	}
	FallBack "Diffuse"
 }