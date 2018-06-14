// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//  Copyright(c) 2016, Michal Skalsky
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification,
//  are permitted provided that the following conditions are met:
//
//  1. Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//
//  2. Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//
//  3. Neither the name of the copyright holder nor the names of its contributors
//     may be used to endorse or promote products derived from this software without
//     specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY
//  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
//  OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT
//  SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
//  OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
//  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
//  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//  EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


Shader "Skybox/AtmosphericScattering"
{
	Properties{
		_SkyColor("Sky Color", Color) = (0.197,0.197,0.197,1)
		_HorizonColor("Horizon Color", Color) = (0.4980392,0.4980392,0.4980392,1)
		_HorizonLevel("Horizon Level", Range(0, 1)) = 0.1
		_MHST("Min Height Stars Threshold", Range(0, 10)) = 1.5 // At what level of darkness may stars be seen
		_HSDT("Height Stars Drop Threshold", Range(0, 10)) = 0.08 // At what level of darkness may stars be seen
		_StarIntensityLevel("Star intensity Level", Range(0, 1)) = 0.3
		_SunAttenuation("Sun Attenuation", Range(0, 1)) = 0.2
	}

	SubShader
	{
		Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off ZWrite Off

		Pass
		{
			CGPROGRAM
			//#pragma shader_feature ATMOSPHERE_REFERENCE
			#pragma shader_feature RENDER_SUN
			#pragma shader_feature HIGH_QUALITY
			
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 5.0
			
			#include "UnityCG.cginc"
			#include "AtmosphericScattering.cginc"

			float3 _CameraPos;
			uniform float4 _SkyColor;
			uniform float4 _HorizonColor;
			uniform float _HorizonLevel;
			uniform vector _StarPos;
			uniform float _MHST;
			uniform float _HSDT;
			uniform float _SunAttenuation;
			uniform float _StarIntensityLevel;
			uniform float _CurrentSunIntensity;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4	pos		: SV_POSITION;
				float3	vertex	: TEXCOORD0;
				float4 posWorld : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = v.vertex;
				return o;
			}

			// Convert from one range to another
			float map(float val, float old1, float old2, float new1, float new2)
			{
				return new1 + (val - old1) * (new2 - new1) / (old2 - old1);
			}

			//---------NOISE GENERATION------------
			//Noise generation based on a simple hash, to ensure that if a given point on the dome
			//(after taking into account the rotation of the sky) is a star, it remains a star all night long
			float Hash(float n) {
				return frac((1.0 + sin(n)) * 415.92653);
			}

			float Noise3d(float3 x) {
				float xhash = Hash(round(400 * x.x) * 37.0);
				float yhash = Hash(round(400 * x.y) * 57.0);
				float zhash = Hash(round(400 * x.z) * 67.0);
				return frac(xhash + yhash + zhash);
			}

			// Get brightness (0,1)
			float GetBright(float3 color) {
				return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
#ifdef ATMOSPHERE_REFERENCE
				float3 rayStart = _CameraPos;
				float3 rayDir = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex));

				float3 lightDir = -_WorldSpaceLightPos0.xyz;

				float3 planetCenter = float3(0, -_PlanetRadius, 0);

				float2 intersection = RaySphereIntersection(rayStart, rayDir, planetCenter, _PlanetRadius + _AtmosphereHeight);		
				float rayLength = intersection.y;

				intersection = RaySphereIntersection(rayStart, rayDir, planetCenter, _PlanetRadius);
				if (intersection.x > 0)
					rayLength = min(rayLength, intersection.x);

				float4 extinction;
				float4 inscattering = IntegrateInscattering(rayStart, rayDir, rayLength, planetCenter, 1, lightDir, 16, extinction);
				return float4(inscattering.xyz, 1);
#else
				float3 rayStart = _CameraPos;
				float3 rayDir = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex));

				float3 lightDir = -_WorldSpaceLightPos0.xyz;

				float3 planetCenter = float3(0, -_PlanetRadius, 0);

				float4 scatterR = 0;
				float4 scatterM = 0;

				float height = length(rayStart - planetCenter) - _PlanetRadius;

				float3 normal = normalize(rayStart - planetCenter);

				float viewZenith = dot(normal, rayDir);
				float sunZenith = dot(normal, -lightDir);

				float3 coords = float3(height / _AtmosphereHeight, viewZenith * 0.5 + 0.5, sunZenith * 0.5 + 0.5);

				coords.x = pow(height / _AtmosphereHeight, 0.5);
				float ch = -(sqrt(height * (2 * _PlanetRadius + height)) / (_PlanetRadius + height));

				if (viewZenith > ch)
				{
					coords.y = 0.5 * pow((viewZenith - ch) / (1 - ch), 0.2) + 0.5;
				}
				else
				{
					coords.y = 0.5 * pow((ch - viewZenith) / (ch + 1), 0.2);
				}
				coords.z = 0.5 * ((atan(max(sunZenith, -0.1975) * tan(1.26*1.1)) / 1.1) + (1 - 0.26));

				scatterR = tex3D(_SkyboxLUT, coords);			

#ifdef HIGH_QUALITY
				scatterM.x = scatterR.w;
				scatterM.yz = tex3D(_SkyboxLUT2, coords).xy;
#else
				scatterM.xyz = scatterR.xyz * ((scatterR.w) / (scatterR.x));// *(_ScatteringR.x / _ScatteringM.x) * (_ScatteringM / _ScatteringR);
#endif

				float3 m = scatterM;
				//scatterR = 0;
				// phase function
				ApplyPhaseFunctionElek(scatterR.xyz, scatterM.xyz, dot(rayDir, -lightDir.xyz));

				// Mix between incoming light & sky color
				float3 lightInscatter = (scatterR * _ScatteringR + scatterM * _ScatteringM) * _IncomingLight.xyz; //(.7 * _IncomingLight.xyz + .3 *_SkyColor.xyz);
#ifdef RENDER_SUN
				lightInscatter += RenderSun(m, dot(rayDir, -lightDir.xyz)) * _SunIntensity;
#endif
				float3 finalColor = max(0, lightInscatter);

				/******************* Calculate stars ******************************/
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				
				// If sun is too intense -> no stars
				if (_CurrentSunIntensity < _MHST) {
					// Get brightness of current pixel. 
					float skyBrightness = GetBright(finalColor);

					float starsThreshold = 0.991; // More stars will appear the lower the threshold
					float twinkleThreshold = 0.0087; // The a star that passes the sum of twinkle threshold and star threshold will twinkle
					float minShine = 0.006;

					// We generate a random value between 0 and 1
					float starIntensity = Noise3d(normalize(i.posWorld.xyz));

					float starIntensInLargeRange = map(starIntensity, starsThreshold, 1, 0, 1);

					//float distFromSun = clamp(sunZenith, i.posWorld.xyz), 0.000001, 1);
					//float attenuationI = clamp(_CurrentSunIntensity / (_SunAttenuation * distFromSun), 0, 1);
					//float heightI = map(sqrt(sunHeightAboveHorizon / _HSDT), 0, sqrt(_MHST / _HSDT), 0, 1);

					// Check if the current star intensity is high enough to be visible at all relative to the sun Intensity
					float revealRatio = saturate(starIntensInLargeRange - (0.91 * _CurrentSunIntensity + /*0.5 * attenuationI*/ +5 * skyBrightness));

					if (revealRatio >= 0) {
						revealRatio = min(0.6, revealRatio); // Floor level (so it won't take them so long to pop up

						// And we apply a threshold to keep only the brightest areas
						if (starIntensity - twinkleThreshold >= starsThreshold) {
							// Twinkly star!
							float minIntensity = starsThreshold + twinkleThreshold;
							float intensityRange = (starIntensity - minIntensity) * (1.0 / minIntensity);  // Range of 0..1
							float extraIntensity = lerp(0.0080, minShine, intensityRange); // How much to add to star intensity

							// Extra intensity wave change as a factor of time (twinkle effect)
							float currExtraInt = lerp(extraIntensity, minShine, abs(sin(map(intensityRange, 0, 1, 4.5, 50) *_Time[1])));

							// Light brighter by intensity range. Change light ferquencies between stars
							starIntensity = revealRatio * (pow((starIntensity - starsThreshold + currExtraInt) / (1.0 - starsThreshold), 5) *
								(-skyBrightness + _StarIntensityLevel));
							finalColor += float3(starIntensity, starIntensity, starIntensity);
						}
						else if (starIntensity >= starsThreshold) {
							// We compute the star intensity
							starIntensity = revealRatio * (pow((starIntensity - starsThreshold + minShine) / (1.0 - starsThreshold), 5) *
										(-skyBrightness + _StarIntensityLevel));
							finalColor += float3(starIntensity, starIntensity, starIntensity);
						}
					}
				}

				return float4(finalColor, 1);
#endif
			}
			ENDCG
		}
	}
}
