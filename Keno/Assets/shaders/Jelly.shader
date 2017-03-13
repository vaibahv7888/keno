Shader "Ramy/Jelly" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
		_Color ("Color",Color) = (1,1,1,1)
		_forcePoint ("ForcePoint", Vector) = (5,5,5,1)
    	
  	}
    SubShader {
        Pass {
        	Tags { "LightMode" = "ForwardBase" } 
            GLSLPROGRAM
            uniform float _Speed;
			uniform float _Time;
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			uniform vec4 _forcePoint;
			uniform mat4 _ObjectToWorld; 
			
			uniform vec4 _WorldSpaceLightPos0; 
         	uniform vec4 _LightColor0; 
			
			varying vec2 _texCoord;
			varying vec3 pos;
			varying float distance_z_sqr;
			
			
			float NdotL;
			
			//lighting
			varying vec4 _diffuse;
			
			#ifdef VERTEX
            void main() 
            {
            	pos = (gl_Vertex).xyz;
            	vec3 dir = (_ObjectToWorld * _forcePoint).xyz - (_ObjectToWorld * gl_Vertex).xyz;
				float distance = length(dir);
				
 				pos += dir * 10.0*1.0/pow(distance,2.0);
 		
 		
            	gl_Position = gl_ModelViewProjectionMatrix * vec4(pos,1);
				_texCoord = vec2(gl_MultiTexCoord0.x,gl_MultiTexCoord0.y);
            	
            	//lambert basic lighting
            	vec3 _normal = normalize(gl_Normal * gl_NormalMatrix);
            	NdotL = dot(_normal ,normalize(_WorldSpaceLightPos0).xyz);	
            	
            	vec3 ambientLighting = vec3(gl_LightModel.ambient)*2.0;
            	_diffuse = _Color * vec4(ambientLighting,0.0)* (5.0 * _LightColor0 * NdotL);
            	
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
