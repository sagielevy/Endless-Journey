Shader "Custom/Terrain" {
	Properties {
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

		float minHeight;
		float maxHeight;

		half _Glossiness;
		half _Metallic;
		half _WaterAlpha;

		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		 // This is to use vertex coloring instead!
		 //struct Input {
         //    float2 uv_MainTex;
         //    float3 vertexColor; // Vertex color stored here by vert() method
         //};
         //void vert (inout appdata_full v, out Input o)
         //{
         //    UNITY_INITIALIZE_OUTPUT(Input,o);
         //    o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
         //}

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
