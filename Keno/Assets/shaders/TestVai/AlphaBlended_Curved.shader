Shader "Vai/Particles/AlphaBlended_Curved"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        
         _QOffset ("Offset", Vector) = (0,0,0,0)
		_Brightness ("Brightness", Float) = 0.0
		_Dist ("Distance", Float) = 100.0
		
		_CurveSensitivity	("CurveSensitivity", Float) = 0.001
		_CameraOffset	("CameraOffset (XY)", Float) = 20
    }
   
 
 
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
 
        Pass {
        
            Cull Back
            ZWrite Off
            Blend srcAlpha OneMinusSrcAlpha
           
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
           
			float4 _QOffset;
			float _Dist;
			float _Brightness;
			float _CurveSensitivity;
			float _CameraOffset;
           
            // Struct Input || VertOut
            struct appdata {
                half4 vertex : POSITION;
                half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
           
            //VertIn
            struct v2f {
                half4 pos : POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
            };
 
            v2f vert (appdata v)
            {
                v2f o;
                
                float4 vPos = mul (UNITY_MATRIX_MV, v.vertex);
                
				_Dist = (vPos.z+_CameraOffset) * (vPos.z+_CameraOffset);
				vPos += _CurveSensitivity * _QOffset * _Dist;
				o.pos = mul (UNITY_MATRIX_P, vPos);

                
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
  
                return o;
            }
           
 
            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col;
                fixed4 tex = tex2D(_MainTex, i.texcoord);
 
                col.rgb = i.color.rgb * tex.rgb;
                col.a = i.color.a * tex.a;
                return col;
               
            }
            ENDCG          
        }
    }
    FallBack "Diffuse"
}