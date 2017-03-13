Shader "Ramy/MySkyBox" {
	Properties {
		_SkyBoxImage ("Base (RGB)", Cube) = "white" {}
		_AmbientColor ("Ambient Color", Color) = (1,1,1,1)
	}
	  SubShader {
      Tags { "Queue" = "Background" }
 
      Pass {   
         ZWrite Off
      	// Cull Front
 
         GLSLPROGRAM
 
         // User-specified uniform
         uniform samplerCube _SkyBoxImage;
         uniform vec4 _AmbientColor;
         
         varying vec3 _texCoord;
 
         #ifdef VERTEX
 
         void main()
         {
      		_texCoord = gl_MultiTexCoord0.xyz;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;       
         }
 
         #endif
 
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = _AmbientColor*textureCube(_SkyBoxImage, _texCoord);
         }
 
         #endif
         ENDGLSL
    }
   }
	//FallBack "Diffuse"
}
