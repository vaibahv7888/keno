Shader "Ramy/Diffuse" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
		_Color ("Color",Color) = (1,1,1,1)
        _Speed ("Speed", Float) = 0
        _TilesX ("# Tiles in texture row", Float) = 16
        _TilesY ("# Tiles in texture column", Float) = 16
        
        _FarClear ("# FarClear",Float) = 2000
    	
  	}
    SubShader {
        Pass {
        	Tags { "LightMode" = "ForwardBase" } 
            GLSLPROGRAM
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			
			uniform float _TilesX;
			uniform float _TilesY;
			
			//lights
			
			uniform vec4 _WorldSpaceLightPos0; 
         	uniform vec4 _LightColor0; 
			
			varying vec2 _texCoord;
			varying vec4 pos;
			varying float distance_z_sqr;
			
			
			float NdotL;
			
			//lighting
			varying vec4 _diffuse;
			
			#ifdef VERTEX
            void main() 
            {
            	pos = gl_ModelViewMatrix * gl_Vertex; 		
            	gl_Position = gl_ProjectionMatrix * pos;
				_texCoord = vec2(gl_MultiTexCoord0.x*_TilesX,gl_MultiTexCoord0.y*_TilesY);
            	
            	//lambert basic lighting
            	vec3 ambientLighting = vec3(gl_LightModel.ambient)*2.0;
            	vec3 _normal = normalize(gl_Normal * gl_NormalMatrix);
            	
            	NdotL = dot(_normal ,normalize(_WorldSpaceLightPos0).xyz);	
            	_diffuse = clamp(_Color * vec4(ambientLighting,0.0)* (5.0 * _LightColor0 * NdotL),0.0,1.0);
            	
            	float NdotL1 = dot(_normal ,normalize(vec3(-_WorldSpaceLightPos0.x,-_WorldSpaceLightPos0.y,-_WorldSpaceLightPos0.z)));	
            	_diffuse += clamp(_Color * vec4(ambientLighting,0.0)* (5.0 * _LightColor0 * (NdotL1)),0.0,1.0);            	
            }
            #endif

			#ifdef FRAGMENT
            void main ()
			{
				vec4 texel = texture2D(_MainTex,_texCoord) * _diffuse;
			   	gl_FragColor = texel;
			}
			#endif
            ENDGLSL
        }
    }
	//FallBack "Diffuse"
}
