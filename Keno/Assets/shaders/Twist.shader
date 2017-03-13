Shader "Ramy/Twist" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Twist ("Twist", float) = 1
		_HitPoint("HitPoint", Vector) = (0.0,0.0,0.0)
	}
	SubShader {
	Pass{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members darken)
#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		uniform float _Twist;
		uniform float3 _HitPoint;

		struct v2f {
                float4 pos : SV_POSITION;
                fixed3 color : COLOR0;
                float4 uv:TEXCOORD0;
                float4 darken;
                float4 __pos;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                float4 pos = v.vertex;
                
                //float3 hit = v.normal*_HitPoint
                o.darken = 1;
                if(length(_HitPoint.xyz) > 0)
                {
                	pos.xyz += v.normal*_Twist*1/clamp(distance(_HitPoint.xyz,pos.xyz),0.1,1.0);
                }
                //o.darken = clamp(1-pos.y,0,2);
                o.__pos = pos;
                o.pos = mul (UNITY_MATRIX_MVP, pos);
                o.uv =  float4(v.texcoord.xy*10,0,0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
//           		 if( length(_HitPoint)>0 && distance(_HitPoint.xyz,i.__pos.xyz) <= _Twist){
//                
//                	i.darken=2;
//                	i.darken.r = 10;
//                	//pos.z *= 0.5;
//                }
                return fixed4 (tex2D(_MainTex,i.uv.xy)) * i.darken;
            }
		ENDCG
		}
	}
	FallBack "Diffuse"
}
