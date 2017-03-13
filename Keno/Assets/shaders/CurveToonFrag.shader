Shader "Ramy/Curve/FragToon" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
		_Color ("Color",Color) = (1,1,1,1)
        _Speed ("Speed", Float) = 0
        _TilesX ("# Tiles in texture row", Float) = 16
        _TilesY ("# Tiles in texture column", Float) = 16
        
        _CurveSensitivity	("# _CurveSensitivity", Float) = 0.001
        _DirOfCurveX	("# DirOfCurve X", Float) = 1
        _DirOfCurveY	("# DirOfCurve Y", Float) = 1
        
        _FarClear ("# FarClear",Float) = 2000
    	
  	}
    SubShader {
        Pass {
        
        	Tags { "LightMode" = "ForwardBase" } 
        
            GLSLPROGRAM
            uniform float _Speed;
			uniform float _Time;
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			
			uniform float _TilesX;
			uniform float _TilesY;
			
			uniform float _CurveSensitivity;
			uniform float _DirOfCurveX;
			uniform float _DirOfCurveY;
			
			uniform float _FarClear;
			
			//lights
			
			uniform vec4 _WorldSpaceLightPos0; 
         	uniform vec4 _LightColor0; 
			
			varying vec3 normal;
			varying vec2 _texCoord;
			varying vec4 pos;
			varying float distance_z_sqr;
			
			
			float NdotL;
			
			//lighting
			varying vec4 _diffuse;
			
			#ifdef VERTEX
            void main() 
            {
            
                //lambert basic lighting
            	vec3 _normal = normalize(gl_Normal * gl_NormalMatrix);
            	NdotL = dot(_normal ,normalize(_WorldSpaceLightPos0).xyz);	
            	
            	vec3 ambientLighting = vec3(gl_LightModel.ambient)*2.0;
            	_diffuse = _Color * vec4(ambientLighting,0.0)* (5.0 * _LightColor0 * NdotL);
 
            	//tiling
            	_texCoord = vec2(gl_MultiTexCoord0.x*_TilesX,gl_MultiTexCoord0.y*_TilesY);
            	
            	//for toon shading
            	normal = gl_Normal;
                                  
			   	//curve
            	pos = gl_ModelViewMatrix * gl_Vertex;
				distance_z_sqr = pow(pos.z,2.0);
 				pos.x = pos.x + _CurveSensitivity * _DirOfCurveX * distance_z_sqr;
 				pos.y = pos.y + _CurveSensitivity * _DirOfCurveY * distance_z_sqr;
 
            	gl_Position = gl_ProjectionMatrix * pos;
				
            	
            }
            #endif

			#ifdef FRAGMENT
            void main ()
			{
				vec4 color;
				vec3 n = normalize(normal);
				float intensity;
				intensity = dot(vec3(_WorldSpaceLightPos0),n);

				if (intensity > 0.95)
					color = vec4(1.0,0.5,0.5,1.0);
				else if (intensity > 0.5)
					color = vec4(0.6,0.3,0.3,1.0);
				else if (intensity > 0.25)
					color = vec4(0.4,0.2,0.2,1.0);
				else
					color = vec4(0.2,0.1,0.1,1.0);
			
				vec4 texel = texture2D(_MainTex,_texCoord) * _diffuse;
				texel = mix(texel, vec4(0.5,0.5,0.5,1),  clamp(distance_z_sqr  / _FarClear,0.0,1.0));
				
				gl_FragColor = texel * color;
			   	
			}
			#endif
            ENDGLSL
        }
    }
	FallBack "Ramy/Curve/Diffuse"
}
