Shader "Vai/Curve/MyCurveUnlit" {
	Properties {
		_MainTex("Base", 2D ) = "white"{}
		_Color ("Color",Color) = (1,1,1,1)
		_ColorOverLifeTime ("ColorOverLifeTime",Color) = (1,1,1,1)
		
        _TilesX ("# Tiles in texture row", Float) = 16
        _TilesY ("# Tiles in texture column", Float) = 16
        
        _CurveSensitivity	("# _CurveSensitivity", Float) = 0.001
        _DirOfCurveX	("# DirOfCurve X", Float) = 1
        _DirOfCurveY	("# DirOfCurve Y", Float) = 1
        _CameraOffset	("CameraOffset (XY)", Float) = 20
        
        _FarClear ("# FarClear",Float) = 2000
        
        _Brightness ("Brightness",FLoat) = 2
    	
    	_Cutoff ("Alpha Cutoff", Float) = 0.5
  	}
    SubShader {
        Pass {
        	Cull Off
        	
        	AlphaTest Greater [_Cutoff]
        	
        	Tags { "LightMode" = "ForwardBase" } 
        
            GLSLPROGRAM
           
			uniform sampler2D _MainTex;
			uniform vec4 _Color;
			uniform float _Cutoff;
			
			uniform float _TilesX;
			uniform float _TilesY;
			
			uniform float _CurveSensitivity;
			uniform float _DirOfCurveX;
			uniform float _DirOfCurveY;
			uniform float _FarClear;
			uniform float _CameraOffset;
			
			varying float distance_z_sqr;
			varying vec4 pos;
			varying vec2 _texCoord;
			
			uniform float _Brightness;
			
			#ifdef VERTEX
            void main() 
            {
			   	//curve
//            	pos = gl_ModelViewMatrix * gl_Vertex;
//				//distance_z_sqr = pos.z*pos.z;
//				distance_z_sqr = (pos.z+_CameraOffset) * (pos.z+_CameraOffset);
// 				pos.x = pos.x + _CurveSensitivity * _DirOfCurveX * pos.z * -20.0;
// 				pos.y = pos.y + _CurveSensitivity * _DirOfCurveY * distance_z_sqr;
 
//            	gl_Position = gl_ProjectionMatrix * pos;
//				_texCoord = vec2(gl_MultiTexCoord0.x*_TilesX,gl_MultiTexCoord0.y*_TilesY);
            }
            #endif

			#ifdef FRAGMENT
            void main ()
			{
//				vec4 texel = texture2D(_MainTex,_texCoord) * _Color * _Brightness;
//				texel = mix(texel, vec4(0.5,0.5,0.5,0.5),  clamp(distance_z_sqr  / _FarClear,0.0,0.0));
//			   	gl_FragColor = texel;
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
         
         uniform float _CurveSensitivity;
         uniform float _DirOfCurveX;
		uniform float _DirOfCurveY;
		uniform float _FarClear;
		uniform float _CameraOffset;

         varying vec4 textureCoordinates;
         
         varying float distance_z_sqr;
         varying vec4 pos;

		uniform vec4 _Color;
		uniform float _Brightness;
		uniform vec4 _ColorOverLifeTime;

         #ifdef VERTEX
                  
         void main()
         {
        	pos = gl_ModelViewMatrix * gl_Vertex;
			//distance_z_sqr = pos.z*pos.z;
			distance_z_sqr = (pos.z+_CameraOffset) * (pos.z+_CameraOffset);
			pos.x = pos.x + _CurveSensitivity * _DirOfCurveX * pos.z * -20.0;
			pos.y = pos.y + _CurveSensitivity * _DirOfCurveY * distance_z_sqr;

            textureCoordinates = gl_MultiTexCoord0;
            
//            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            gl_Position = gl_ProjectionMatrix * pos;
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = 
               texture2D(_MainTex, vec2(textureCoordinates)) * mix(_Color, _ColorOverLifeTime, 0.5) * _Brightness;
         }
         
         #endif

         ENDGLSL
      }
    }
	//FallBack "Diffuse"
}
