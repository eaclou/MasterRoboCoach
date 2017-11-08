Shader "Instanced/IndirectInstanceRockReliefArenaShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_BaseColorPrimary ("_BaseColorPrimary", Color) = (1,1,1,1)
		_BaseColorSecondary ("_BaseColorSecondary", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

        sampler2D _MainTex;
		float4 _BaseColorPrimary;
		float4 _BaseColorSecondary;

        struct Input {
            float2 uv_MainTex;
			float3 color;
        };
				
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float4x4> matricesCBuffer;
		StructuredBuffer<float4x4> invMatricesCBuffer;
    #endif

        void rotate2D(inout float2 v, float r)
        {
            float s, c;
            sincos(r, s, c);
            v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

		float3 qtransform(float4 q, float3 v){ 
			return v + 2.0*cross(cross(v, q.xyz ) + q.w*v, q.xyz);
		}

		float rand(in float2 uv)
		{
			float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
			return abs(noise.x + noise.y) * 0.5;
		}

        void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            //TransformData data = instancedRocksCBuffer[unity_InstanceID];

			//unity_ObjectToWorld._11_21_31_41 = float4(1, 0, 0, 0);
			//unity_ObjectToWorld._12_22_32_42 = float4(0, 1, 0, 0);
			//unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1, 0);
			//unity_ObjectToWorld._14_24_34_44 = float4(matricesCBuffer[unity_InstanceID]._14_24_34_44.xyz, 1); //float4(matricesCBuffer[unity_InstanceID]._11_21_31_41.x, matricesCBuffer[unity_InstanceID]._11_21_31_41.y, matricesCBuffer[unity_InstanceID]._11_21_31_41.w, 1);		
			
			/*float rx = data.rotation.x;
			float ry = data.rotation.y;
			float rz = data.rotation.z;
			float4x4 rotationMatrix = float4x4(  // ZXY:
				cos(rz)*cos(ry)-sin(rz)*cos(rx)*cos(ry)-sin(rz)*sin(rx)*sin(ry), -sin(rz)*cos(rx), cos(rz)*sin(ry)-sin(rz)*cos(rx)*sin(ry)+sin(rz)*sin(rx)*cos(ry), 0,
				sin(rz)*cos(ry)-cos(rx)*sin(ry), cos(rz)*cos(rx), sin(rz)*sin(ry)-cos(rz)*sin(rx)*cos(ry), 0,
				-cos(rx)*sin(ry), sin(rx), cos(rx)*cos(ry), 0,
				0, 0, 0, 1);*/
			//unity_ObjectToWorld = mul(unity_ObjectToWorld, rotationMatrix);  // apply rotation

			unity_ObjectToWorld = matricesCBuffer[unity_InstanceID];
            unity_WorldToObject = invMatricesCBuffer[unity_InstanceID];
        #endif
        }

        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o) {
			float4 col = 1.0f;
			float randColorLerp = 0;
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			//col.gb = (float)(unity_InstanceID % 256) / 255.0f;
			col = matricesCBuffer[unity_InstanceID]._11_21_31_41;	
			randColorLerp = rand(col.xz);
			//col = col.xxx1;
#else
			//col.gb = float4(0, 0, 1, 1);
			col = float4(0, 0, 1, 1);
#endif

			//float3 hue = float3(rand(col.xy), rand(col.yz), rand(col.zw));
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);			
            o.Albedo = c.rgb * lerp(_BaseColorPrimary, _BaseColorSecondary, randColorLerp); //float3(c.r * hue.x, c.y * hue.y, c.z * hue.z);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}