Shader "Ramy/Shadow" {
	Properties {    	
  	}
    SubShader {
    Tags { "Queue" = "Transparent" } 
        Pass {
        	
        	ZWrite Off
       		Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
            GLSLPROGRAM
			varying vec4 localPos;
			
			#ifdef VERTEX
            void main() 
            {
            	localPos =  gl_ModelViewProjectionMatrix * gl_Vertex;
            	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            }
            #endif
			#ifdef FRAGMENT
            void main ()
			{
				float distance = 0.1*pow(length(localPos),2.0);
			   	gl_FragColor = vec4(1.0,1.0,1.0,1.0)*distance;
			}
			#endif
            ENDGLSL 
        }
    }
	//FallBack "Diffuse"
}
