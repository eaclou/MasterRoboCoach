Shader "TerrainBlit/TerrainBlitStrataAdjustments"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}  // Original Heights
		_NewTex  ("Texture", 2D) = "black" {}
		_MaskTex1 ("Texture", 2D) = "white" {}
		_MaskTex2 ("Texture", 2D) = "white" {}
		_FlowTex ("Texture", 2D) = "gray" {}
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

			// Default to Textures, which should have no effect if they are not Set (default values)
			#pragma shader_feature _USE_NEW_NOISE
			#pragma shader_feature _USE_MASK1_NOISE
			#pragma shader_feature _USE_MASK2_NOISE
			#pragma shader_feature _USE_FLOW_NOISE
						
			#include "UnityCG.cginc"
			#include "Assets/Shaders/Inc/NoiseShared.cginc"
			#include "Assets/Shaders/Inc/StrataRemap.cginc"
			
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

			struct RockStrataData {
				float3 color;
				float hardness;
			};

			sampler2D _MainTex;
			
			StructuredBuffer<RockStrataData> rockStrataDataCBuffer;

			//int _PixelsWidth;
			//int _PixelsHeight;
			//float4 _GridBounds;	

			float _MinAltitude;
			float _AvgAltitude;
			float _MaxAltitude;
			float _RemapOffset;
			float _StrataIndex;
			float _NumStrata;
						
		
			fixed4 frag (v2f i) : SV_Target
			{	
				float4 baseHeight = tex2D(_MainTex, i.uv);

				float normalizedAltitude = (baseHeight.x - _MinAltitude) / (_MaxAltitude - _MinAltitude);
				float strataValue = GetRemappedValue01(normalizedAltitude, _RemapOffset, _NumStrata);
				float pixelAltitude = baseHeight.x;

				float strataHardness = rockStrataDataCBuffer[_StrataIndex].hardness;

				float strataStartValue01 = GetRemappedValue01(_StrataIndex / (_NumStrata - 1), _RemapOffset, _NumStrata);
				float strataStartAltitude = strataStartValue01 * (_MaxAltitude - _MinAltitude) + _MinAltitude;

				float strataTopValue01 = GetRemappedValue01((_StrataIndex + 1) / (_NumStrata - 1), _RemapOffset, _NumStrata);
				float strataTopAltitude = strataTopValue01 * (_MaxAltitude - _MinAltitude) + _MinAltitude;

				float cliffMask = 0;
				
				float centerOfStrataAltitude = (strataTopAltitude - strataStartAltitude) * 0.5 + strataStartAltitude;
				float heightDeltaSoft = 0;
				if(pixelAltitude < centerOfStrataAltitude) {
					float midPointAltitude = (centerOfStrataAltitude - _MinAltitude) * 0.5;
					float distToMidAltitude = abs(midPointAltitude - pixelAltitude);
					//cliffMask = 1.0 - saturate(distToMidAltitude / (centerOfStrataAltitude - _MinAltitude) * 0.5);

					float distToStrataCenter = centerOfStrataAltitude - pixelAltitude;
					cliffMask = 1.0 - saturate(distToStrataCenter / ((centerOfStrataAltitude - _MinAltitude) * 0.5));

					heightDeltaSoft += distToStrataCenter * cliffMask;
				}
				else {
					float midPointAltitude = (_MaxAltitude - centerOfStrataAltitude) * 0.5;
					float distToMidAltitude = abs(midPointAltitude - pixelAltitude);
					//cliffMask = 1.0 - saturate(distToMidAltitude / (_MaxAltitude - centerOfStrataAltitude) * 0.5);

					float distToStrataCenter = pixelAltitude - centerOfStrataAltitude;
					cliffMask = 1.0 - saturate(distToStrataCenter / ((_MaxAltitude - centerOfStrataAltitude) * 0.5));
					
					heightDeltaSoft -= distToStrataCenter * cliffMask;
				}


				// HARD CLIFFS ONLY:::				
				//float4 baseHeight = tex2D(_MainTex, i.uv);

				//float normalizedAltitude = (baseHeight.x - _MinAltitude) / (_MaxAltitude - _MinAltitude);
				//float strataValue = GetRemappedValue01(normalizedAltitude, _RemapOffset, _NumStrata);
				//float pixelAltitude = baseHeight.x;

				//float strataHardness = rockStrataDataCBuffer[_StrataIndex].hardness;

				//float strataStartValue01 = GetRemappedValue01(_StrataIndex / (_NumStrata - 1), _RemapOffset, _NumStrata);
				//float strataStartAltitude = strataStartValue01 * (_MaxAltitude - _MinAltitude) + _MinAltitude;

				//float strataTopValue01 = GetRemappedValue01((_StrataIndex + 1) / (_NumStrata - 1), _RemapOffset, _NumStrata);
				//float strataTopAltitude = strataTopValue01 * (_MaxAltitude - _MinAltitude) + _MinAltitude;

				float cliffMaskHard = (pixelAltitude - strataStartAltitude) / (_MaxAltitude - strataStartAltitude);
				cliffMaskHard = 1.0 - saturate(cliffMaskHard);
				if(pixelAltitude < strataStartAltitude)
					cliffMaskHard = 0;

				float heightDeltaHard = (strataTopAltitude - strataStartAltitude) * cliffMaskHard * strataHardness;
				
				//baseHeight.x = cliffMask;
				//baseHeight.x += (strataTopAltitude - strataStartAltitude) * cliffMask * strataHardness;
				//return baseHeight;
				


				baseHeight.x += lerp(heightDeltaSoft, heightDeltaHard, strataHardness);

				return baseHeight;
				

				
			}



			ENDCG
		}
	}
}
