Shader "TerrainBlit/TerrainBlitCenterHeightTextures"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}  // Original Heights
		_CenterTex  ("Texture", 2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
						
						
			#include "UnityCG.cginc"
			#include "Assets/Shaders/Inc/NoiseShared.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _CenterTex;
			
			int _PixelsWidth;
			int _PixelsHeight;
			float4 _GridBounds;	

						
			fixed4 frag (v2f i) : SV_Target
			{	
								
				float centerAltitude = tex2D(_CenterTex, float2(0.5, 0.5));
				
				float4 baseHeight = tex2D(_MainTex, i.uv);

				baseHeight.x -= centerAltitude;
				
				return baseHeight;				
			}



			ENDCG
		}
	}
}
