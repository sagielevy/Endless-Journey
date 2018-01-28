Shader "Custom/Terrain" {
	Properties {
		//testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1
		
		// Sagie additions, to make it look more dreamy
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_WaterAlpha ("WAlpha", Range(0,1)) = 0.7
	}
	SubShader {
		// TODO Verify that this is not too expensive
		Tags {"RenderType"="Transparent" } // { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade //vertex:vert (might be more efficient)

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4;

		int layerCount;
		float3 baseColours[maxLayerCount];
		float baseStartHeights[maxLayerCount];
		float baseBlends[maxLayerCount];
		float baseColourStrength[maxLayerCount];
		//float baseTextureScales[maxLayerCount];

		float minHeight;
		float maxHeight;

		//sampler2D testTexture;
		float testScale;

		half _Glossiness;
		half _Metallic;
		half _WaterAlpha;

		UNITY_DECLARE_TEX2DARRAY(baseTextures);

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		 //struct Input {
         //    float2 uv_MainTex;
         //    float3 vertexColor; // Vertex color stored here by vert() method
         //};

         //void vert (inout appdata_full v, out Input o)
         //{
         //    UNITY_INITIALIZE_OUTPUT(Input,o);
         //    o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
         //}

//		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
//			float3 scaledWorldPos = worldPos / scale;
//			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
//			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
//			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
//			return xProjection + yProjection + zProjection;
//		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			o.Alpha = 1;

			// Change alpha value for water (must have at least two layers!)
			if (layerCount > 2 && baseStartHeights[1] > heightPercent) {
				o.Alpha = _WaterAlpha;
			}

			for (int i = 0; i < layerCount; i ++) {
				float drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

				float3 baseColour = baseColours[i] * baseColourStrength[i];
				
				// Sagie: Where we're going, we ain't gonna need no textures no moe
				//float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1-baseColourStrength[i]);

				//o.Albedo = o.Albedo * (1-drawStrength) + (baseColour+textureColour) * drawStrength;
				o.Albedo = o.Albedo * (1-drawStrength) + baseColour * drawStrength;
			}

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}


		ENDCG
	}
	FallBack "Diffuse"
}
