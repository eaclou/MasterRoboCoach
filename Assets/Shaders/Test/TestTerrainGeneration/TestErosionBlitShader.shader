Shader "Effects/TestErosionBlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			#include "UnityCG.cginc"

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
			float4 _MainTex_TexelSize;
			int _PixelsWidth;
			int _PixelsHeight;

			fixed4 frag (v2f i) : SV_Target
			{	
				//float w, h;
				//_MainTex.GetDimensions(w, h);

				float2 _Pixels = float2((float)_PixelsWidth, (float)_PixelsHeight);  // why the fuck did I need to mult by 2?? -- something weird with Bilinear Sampling?
				
				// Cell centre
				//float2 uv = round(i.uv * _Pixels) / _Pixels;
 
				 // Neighbour cells
				float s = 1 / _Pixels;
				float4 cl = tex2D(_MainTex, i.uv + float2(-s, 0)); // Centre Left
				float4 tc = tex2D(_MainTex, i.uv + float2(0, -s)); // Top Centre
				float4 cc = tex2D(_MainTex, i.uv + float2(0, 0)); // Centre Centre
				float4 bc = tex2D(_MainTex, i.uv + float2(0, +s)); // Bottom Centre
				float4 cr = tex2D(_MainTex, i.uv + float2(+s, 0)); // Centre Right

				float waterHeight = 0.2;

				float waterGradLeft = (cl.x + cl.b * waterHeight) - (cc.x + cc.b * waterHeight);
				float waterGradRight = (cr.x + cr.b * waterHeight) - (cc.x + cc.b * waterHeight);
				float waterGradUp = (tc.x + tc.b * waterHeight) - (cc.x + cc.b * waterHeight);
				float waterGradDown = (bc.x + bc.b * waterHeight) - (cc.x + cc.b * waterHeight);

				float gradAvg = length(float2((waterGradLeft - waterGradRight) / 2, (waterGradDown - waterGradUp) / 2));
				float flowIn = max(0,waterGradLeft) + max(0,waterGradRight) + max(0,waterGradUp) + max(0,waterGradDown);
				float flowOut = (min(0,waterGradLeft) + min(0,waterGradRight) + min(0,waterGradUp) + min(0,waterGradDown));
				float strength = 1;
				float groundStrength = 0.5 * gradAvg;

				float newWaterAmount = max(0, cc.b + (flowIn + flowOut) * strength);
				float newGroundAmount = cc.x + 0.2 * (cl.x + tc.x + bc.x + cr.x + cc.x) * groundStrength;

				float dx = gradAvg;
				float dz = (waterGradDown + waterGradUp) / 2;
				 // Diffusion step
				float factor = groundStrength * (	0.25 * (cl.x + tc.x + bc.x + cr.x)	- cc.x);
				cc.x += factor;

				float r = cc.x;
				float g = dx * 5;

				float b = newWaterAmount;

				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col = float4(r,g,b,1); //1 - col;
				return col;
			}
			ENDCG
		}
	}
}
