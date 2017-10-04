// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/shad_UIFitnessGraphBasic"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "black" {}
		_FitnessTex ("Fitness Texture", 2D) = "black" {}
		_ZoomFactorX ("ZoomFactorX", Range(0.01,2)) = 1.0
		_ZoomFactorY ("ZoomFactorY", Range(0.01,2)) = 1.0
		_WarpFactorX ("ZoomFactorX", Range(0.10,4)) = 1.0
		_WarpFactorY ("ZoomFactorY", Range(0.10,4)) = 1.0		
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			uniform fixed4 _Color;
			uniform fixed _ZoomFactorX;
			uniform fixed _ZoomFactorY;
			uniform fixed _WarpFactorX;
			uniform fixed _WarpFactorY;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _FitnessTex;

			fixed4 frag(v2f IN) : SV_Target
			{
			    // TEMPORARY!!!!!
				//fixed tempZoomX = 0.5;  // Replace with _ZoomFactorX !!
				//fixed tempZoomY = 0.2;  // Replace with _ZoomFactorY !!!
				
				
				float lineWidth = 0.01 * _ZoomFactorY;	
				float gridLineWidthX = 0.002 * _ZoomFactorX;	
				float gridLineWidthY = 0.002 * _ZoomFactorY;	
				float gridDivisions = 10;	
				float gridLineSpacing = 1 / gridDivisions;
				
				half2 finalCoords = IN.texcoord;
				float initialScore = tex2D(_FitnessTex, 0).x;
				float latestScore = tex2D(_FitnessTex, 1).x;
				float pivotY = (initialScore + latestScore) / 2;
				if((pivotY + (_ZoomFactorY * 0.5)) > 1) {
					pivotY = 1 - (_ZoomFactorY * 0.5);
				}
				if((pivotY - (_ZoomFactorY * 0.5)) < 0) {
					pivotY = (_ZoomFactorY * 0.5);
				}
				finalCoords.x = (finalCoords.x * _ZoomFactorX) + (1 - _ZoomFactorX);
				finalCoords.y = (finalCoords.y * _ZoomFactorY) + (pivotY - (_ZoomFactorY * 0.5));
				//latestScore = (latestScore * _ZoomFactorY) + (latestScore - (_ZoomFactorY * 0.5));
				// color.x is the Raw Fitness VALUE (0-1), baked into red channel
				// color.y is the weighted Fitness VALUE (0-1), baked into green channel
				
				
				half4 color = tex2D(_FitnessTex, finalCoords.x) * IN.color;
				clip (color.a - 0.01);
				half4 bgColor = half4(0.21, 0.21, 0.21, 1.0);
				half4 onColor = half4(0.24, 0.33, 0.26, 1.0);
				half4 gridColor = half4(0.14, 0.14, 0.14, 1.0);
				half4 rChannelColor = half4(1.0, 0.0, 0.0, 1.0);
				half4 gChannelColor = half4(0.0, 1.0, 0.0, 1.0);
				half4 bChannelColor = half4(0.0, 0.0, 1.0, 1.0);
				
				half4 pixColor = bgColor;
				
				
				float distR = abs(finalCoords.y - color.r);
				float distG = abs(finalCoords.y - color.g);
				float distB = abs(finalCoords.y - color.b);
				
				
				//if(finalCoords.y < color.x) {  // if vertical position is less than fitnessRawScore:
				//	pixColor = onColor;
				//}
				if(finalCoords.y > initialScore) {
					if(finalCoords.y < latestScore) {	
						pixColor = onColor;
					}
				}
				
				// Create GRIDLINES:
				for(int i = 0; i < gridDivisions; i++) {
					float linePos = i * gridLineSpacing;
					float gridDistX = abs(finalCoords.x - linePos);
					if(gridDistX < gridLineWidthX) {
						pixColor = gridColor;
					}
					float gridDistY = abs(finalCoords.y - linePos);
					if(gridDistY < gridLineWidthY) {
						pixColor = gridColor;
					}
				}
				
				if(distR < lineWidth) {
					pixColor = rChannelColor;
				}
				if(distG < lineWidth) {
					pixColor = gChannelColor;
				}				
				if(distB < lineWidth) {
					pixColor = bChannelColor;
				}
				float distZero = abs(finalCoords.y - 0);
				if(distZero < lineWidth) {
					pixColor = half4(0.8, 0.1, 0.1, 1.0);
				}
				float distOne = abs(finalCoords.y - 1.0);
				if(distOne < lineWidth) {
					pixColor = half4(0.1, 0.8, 0.1, 1.0);
				}	
							
				//float distLastScore = abs(finalCoords.y - latestScore);
				//if(distLastScore < lineWidth) {
				//	pixColor = half4(0.1, 0.1, 0.8, 1.0);
				//}
				
				//return float4(0.5, 0.5, 0.5, 1.0);
				return pixColor;
			}
		ENDCG
		}
	}
}
