Shader "Vai/GLSLAlphaTest" {
  Properties {
      _MainTex ("RGBA Texture Image", 2D) = "white" {} 
   }
   SubShader {
      Tags {"Queue" = "Transparent"} 

      Pass {	
         Cull Front // first render the back faces
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha 
            // blend based on the fragment's alpha value
         
         GLSLPROGRAM
                  
         uniform sampler2D _MainTex;	

         varying vec4 textureCoordinates; 

         #ifdef VERTEX
                  
         void main()
         {
            textureCoordinates = gl_MultiTexCoord0;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = 
               texture2D(_MainTex, vec2(textureCoordinates));
         }
         
         #endif

         ENDGLSL
      }

      Pass {	
         Cull Back // now render the front faces
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha 
            // blend based on the fragment's alpha value
         
         GLSLPROGRAM
                  
         uniform sampler2D _MainTex;	

         varying vec4 textureCoordinates; 

         #ifdef VERTEX
                  
         void main()
         {
            textureCoordinates = gl_MultiTexCoord0;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = 
               texture2D(_MainTex, vec2(textureCoordinates));
         }
         
         #endif

         ENDGLSL
      }
   }
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Transparent"
}