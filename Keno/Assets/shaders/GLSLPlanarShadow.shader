﻿Shader "GLSL planar shadow" {
   Properties {
      _Color ("Object's Color", Color) = (0,1,0,1)
      _ShadowColor ("Shadow's Color", Color) = (0,0,0,1)
   }
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } // rendering of object
 
         GLSLPROGRAM
 
         // User-specified properties
         uniform vec4 _Color; 
 
         #ifdef VERTEX
 
         void main()
         {                                
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
 
         #endif
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = _Color;
         }
       
         #endif

         ENDGLSL
      }

      Pass {   
         Tags { "LightMode" = "ForwardBase" } 
            // rendering of projected shadow
         Offset -1.0, -2.0 
            // make sure shadow polygons are on top of shadow receiver
            
         GLSLPROGRAM

         // User-specified uniforms
         uniform vec4 _ShadowColor;
         uniform mat4 _World2Receiver; // set by script

         // The following built-in uniforms ) 
         // are also defined in "UnityCG.glslinc", 
         // i.e. one could #include "UnityCG.glslinc" 
         uniform mat4 _Object2World; // model matrix
         uniform mat4 _World2Object; // inverse model matrix
         uniform vec4 unity_Scale; // w = 1/uniform scale; 
            // should be multiplied to _World2Object 
         vec4 _WorldSpaceLightPos0; 
            // position or direction of light source
                  
         #ifdef VERTEX
         
         void main()
         {      
         	_WorldSpaceLightPos0 = vec4(1,1,-1,0);      
            mat4 modelMatrix = _Object2World;
            mat4 modelMatrixInverse = _World2Object * unity_Scale.w;
            modelMatrixInverse[3][3] = 1.0; 
            mat4 viewMatrix = gl_ModelViewMatrix * modelMatrixInverse;

            vec4 lightDirection;
            if (0.0 != _WorldSpaceLightPos0.w) 
            {
               // point or spot light
               lightDirection = normalize(
                  modelMatrix * gl_Vertex - _WorldSpaceLightPos0);
            } 
            else 
            {
               // directional light
               lightDirection = -normalize(_WorldSpaceLightPos0); 
            }
            
            vec4 vertexInWorldSpace = modelMatrix * gl_Vertex;
            vec4 world2ReceiverRow1 = 
               vec4(_World2Receiver[0][1], _World2Receiver[1][1], 
               _World2Receiver[2][1], _World2Receiver[3][1]);
            float distanceOfVertex = 
               dot(world2ReceiverRow1, vertexInWorldSpace); 
               // = (_World2Receiver * vertexInWorldSpace).y 
               // = height over plane 
            float lengthOfLightDirectionInY = 
               dot(world2ReceiverRow1, lightDirection); 
               // = (_World2Receiver * lightDirection).y 
               // = length in y direction

            if (distanceOfVertex > 0.0 && lengthOfLightDirectionInY < 0.0)
            {
               lightDirection = lightDirection 
                  * (distanceOfVertex / (-lengthOfLightDirectionInY));
            }
            else
            {
               lightDirection = vec4(0.0, 0.0, 0.0, 0.0); 
                  // don't move vertex
            }

            gl_Position = gl_ProjectionMatrix * (viewMatrix 
               * (vertexInWorldSpace + lightDirection));
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = _ShadowColor;
         }
         
         #endif

         ENDGLSL
      }
   }
}