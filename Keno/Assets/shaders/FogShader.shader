Shader "Ramy/FogShader" {
	Properties {
        _FogColor	("# FogColor", Color) = (1,1,1,1)
        _FogStart	("# FogStart", Float) = 0
        _FogEnd		("# FogEnd", Float) = 0
    }
    SubShader {
        Pass {
            GLSLPROGRAM
			uniform vec4 _WorldSpaceCameraPos;

			uniform float _FogStart;
			uniform float _FogEnd;
			uniform vec4 _FogColor;
			
			varying vec2 texCoord;
			varying vec4 pos;
			varying float distance_z;
			
			#ifdef VERTEX
            void main() 
            {
            	vec4 vert = gl_Vertex;
            	pos = gl_Vertex;
				distance_z = abs(_WorldSpaceCameraPos.z - pos.z);
            	gl_Position = gl_ModelViewProjectionMatrix * pos;
            }
            #endif


			#ifdef FRAGMENT
            void main ()
			{
				vec4 texel = gl_FragColor;
				float f = (_FogEnd - distance_z)/(_FogEnd - _FogStart);
				f = clamp(f,0.0,1.0);
				vec4 fogc = (1.0-f)*_FogColor;
			   	gl_FragColor = texel*f + fogc;
			}
			#endif
            ENDGLSL
        }
    }
	//FallBack "Diffuse"
}
