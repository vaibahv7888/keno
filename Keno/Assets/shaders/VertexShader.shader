Shader "Ramy/VertexShader" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
        _Speed ("Speed", Float) = 0
    }
    SubShader {
        Pass {
            GLSLPROGRAM
            uniform float _Speed;
			uniform float _Time;
			uniform sampler2D _MainTex;
			uniform vec4 _WorldSpaceCameraPos;
			varying vec2 texCoord;
			varying vec4 pos;
			#ifdef VERTEX
            void main() 
            {
            	pos = gl_Vertex;
 
 				float distance_z = 0.04*abs(_WorldSpaceCameraPos.z - pos.z);
 				pos.x = pos.x + distance_z*distance_z;
 
            	gl_Position = gl_ModelViewProjectionMatrix * pos;
				texCoord = gl_MultiTexCoord0.xy;
            }
            #endif


			#ifdef FRAGMENT
            void main ()
			{
				vec4 texel = texture2D(_MainTex,texCoord);
			
				float angle = -10.0*_Time+atan(pos.z/pos.x);
				
				if(mod(angle,45.0*3.14/180.0)<0.5 && abs(pos.y)<1.0)
					texel.rgba = 2.0*texel.rgba;
				//float rad = pow(pos.x,2.0) + 0.0 +  pow(pos.z,2.0);
				
				//if(rad <= 0.25 && rad >= 0.245)
					texel.rgba -= 0.5*texel.rgba;
			   	gl_FragColor = texel;
			}
			#endif
            ENDGLSL
        }
    }
	FallBack "Diffuse"
}
