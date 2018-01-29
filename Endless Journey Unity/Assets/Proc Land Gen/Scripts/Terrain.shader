Shader "Custom/Terrain" {
	Properties {
		// Sagie additions, to make it look more dreamy
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_WaterAlpha ("WAlpha", Range(0,1)) = 0.7
	}
	SubShader {
		// TODO Verify that this is not too expensive
		Tags {"RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float epsilon = 1E-4;

		int layerCount;
		half3 baseColours[maxLayerCount];
		half baseStartHeights[maxLayerCount];
		half baseBlends[maxLayerCount];
		half baseColourStrength[maxLayerCount];

		half minHeight;
		half maxHeight;

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

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);

			// layerCount >= 2 && baseStartHeights[1] >= heightPercent
			half isWater = step(2.0, layerCount) * step(heightPercent, baseStartHeights[1]);

			// If water - set alpha, otherwise set opaque
			o.Alpha = isWater * _WaterAlpha + (1-isWater);

			for (int i = 0; i < layerCount; i ++) {
				half drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);
				half3 baseColour = baseColours[i] * baseColourStrength[i];
				
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
