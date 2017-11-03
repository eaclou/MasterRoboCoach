Shader "Custom/DisplayTerrainSurfaceB" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_RemapTex ("Remap Tex", 2D) = "gray" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#include "UnityCG.cginc"
		#include "Assets/Shaders/Inc/NoiseShared.cginc"
		#include "Assets/Shaders/Inc/StrataRemap.cginc"

		#pragma only_renderers d3d11
		#pragma surface surf Standard fullforwardshadows
		#pragma vertex vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0


		sampler2D _MainTex;
		//sampler2D _RemapTex;

		struct v2f {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
			float4 color : COLOR;

			uint id : SV_VERTEXID;
			uint inst : SV_INSTANCEID;
		};

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float4 color : COLOR;
		};

		struct RockStrataData {
			float3 color;
			float hardness;
		};

		float _MinAltitude;
		float _MaxAltitude;
		float _RemapOffset;
		float _NumStrata;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float rand(float2 co){
			return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
		}

		#ifdef SHADER_API_D3D11
        StructuredBuffer<RockStrataData> rockStrataDataCBuffer;
        #endif
		

		void vert (inout v2f v, out Input o) {			
			UNITY_INITIALIZE_OUTPUT(Input,o);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Albedo = IN.worldPos;
			float3 strataColorNext;
			float3 strataColorPrev;
			float3 strataColor;
			float strataHardness; 
			
			float normalizedAltitude = (GetRemappedAltitude(IN.worldPos * float3(1/680 ,1 , 1/680), _MinAltitude, _MaxAltitude) - _MinAltitude) / (_MaxAltitude - _MinAltitude);
			//float3 normalizedPosition = float3(IN.worldPos.x / 680, normalizedAltitude, IN.worldPos.z / 680);
			//float strataValue = GetRemappedValue01(normalizedPosition, _RemapOffset, _NumStrata);
			//float strataValue = tex2D(_RemapTex, float2(normalizedAltitude, 0.5)).x;
			//float pixelAltitude = IN.worldPos.y;
			
			float strataIndex = floor(normalizedAltitude * (_NumStrata-1));  // 7 == SizeOfBuffer - 1
			// REALLY WEIRD stuff going on with this, need to set it up exactly like this otherwise it seems to choose 1 or the other compiler path
			#ifdef SHADER_API_D3D11	
			normalizedAltitude = 1;
            strataColor = rockStrataDataCBuffer[(int)clamp(strataIndex, 0, _NumStrata - 1)].color;
			strataColorNext = rockStrataDataCBuffer[(int)clamp(strataIndex + 1, 0, _NumStrata - 1)].color;
			strataColorPrev = rockStrataDataCBuffer[(int)clamp(strataIndex - 1, 0, _NumStrata - 1)].color;
			strataHardness = rockStrataDataCBuffer[(int)clamp(strataIndex, 0, _NumStrata - 1)].hardness;
			#endif

			

			o.Albedo = float3(1,1,1) * normalizedAltitude * strataColor * strataColor * strataColorNext; // REALLY WEIRD need to multiply by altitude to get strata Colors??????
			o.Albedo = lerp(o.Albedo, float3(0.9, 0.8, 0.7), 0.8);
			o.Albedo *= length(strataColor);// * strataHardness;
			//o.Albedo = lerp(o.Albedo, float3(0.9, 0.8, 0.7), 1);
			if(IN.color.y > 0) {
				o.Albedo = lerp(o.Albedo, float3(0.5, 0.5, 0.5) + (strataColor + strataColorNext + strataColorPrev) * 0.03, smoothstep(0.005, 0.09, IN.color.y));
			}
			if(IN.color.z > 0) {
				o.Albedo = lerp(o.Albedo, float3(1, 1, 1), smoothstep(0.01, 0.08, IN.color.z));
			}
			//o.Albedo = float3(0,0,0);
			//o.Albedo.b = IN.color.b;

			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
