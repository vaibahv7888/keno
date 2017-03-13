Shader "Ramy/CameraShader" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
    }
    SubShader {
    
    	Tags { "RenderType" = "Opaque" }
    
        Pass {
            GLSLPROGRAM
			uniform sampler2D _MainTex;
			
			varying vec2 texCoord;			
			#ifdef VERTEX
            void main() 
            {
            	vec4 pos = gl_Vertex; 
            	gl_Position = gl_ModelViewProjectionMatrix * pos;
				texCoord = gl_MultiTexCoord0.xy;
            }
            #endif


			#ifdef FRAGMENT
            void main ()
			{
				vec4 texel = texture2D(_MainTex,texCoord);
			   	gl_FragColor = texel;
			}
			#endif
            ENDGLSL
        }
    }
	//FallBack "Diffuse"
}
