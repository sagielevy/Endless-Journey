// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:2,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:0,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:3554,x:32480,y:32959,varname:node_3554,prsc:2|emission-2737-OUT;n:type:ShaderForge.SFN_Color,id:8306,x:32089,y:32870,ptovrint:False,ptlb:Sky Color,ptin:_SkyColor,varname:node_8306,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.197,c2:0.197,c3:0.197,c4:1;n:type:ShaderForge.SFN_ViewVector,id:2265,x:31588,y:33214,varname:node_2265,prsc:2;n:type:ShaderForge.SFN_Dot,id:7606,x:31759,y:33214,varname:node_7606,prsc:2,dt:1|A-2265-OUT,B-3211-OUT;n:type:ShaderForge.SFN_Vector3,id:3211,x:31588,y:33351,varname:node_3211,prsc:2,v1:0,v2:-1,v3:0;n:type:ShaderForge.SFN_Color,id:3839,x:32089,y:33053,ptovrint:False,ptlb:Horizon Color,ptin:_HorizonColor,varname:_GroundColor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4980392,c2:0.4980392,c3:0.4980392,c4:1;n:type:ShaderForge.SFN_Power,id:4050,x:32089,y:33214,varname:node_4050,prsc:2|VAL-6125-OUT,EXP-7609-OUT;n:type:ShaderForge.SFN_Vector1,id:7609,x:31929,y:33357,varname:node_7609,prsc:2,v1:8;n:type:ShaderForge.SFN_OneMinus,id:6125,x:31929,y:33214,varname:node_6125,prsc:2|IN-7606-OUT;n:type:ShaderForge.SFN_Lerp,id:2737,x:32295,y:33053,cmnt:Sky,varname:node_2737,prsc:2|A-8306-RGB,B-3839-RGB,T-4050-OUT;proporder:8306-3839;pass:END;sub:END;*/

Shader "CubedParadox/Simple Gradient Sky" {
    Properties {
        _SkyColor ("Sky Color", Color) = (0.197,0.197,0.197,1)
        _HorizonColor ("Horizon Color", Color) = (0.4980392,0.4980392,0.4980392,1)
		_HorizonLevel ("Horizon Level", Range(0, 1)) = 0.1
		_BrightThreshold ("Brightness Threshold", Range(0, 1)) = 0.3 // At what level of darkness may light be seen
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Background"
            "RenderType"="Opaque"
            "PreviewType"="Skybox"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma target 3.0

            uniform float4 _SkyColor;
            uniform float4 _HorizonColor;
			uniform float _HorizonLevel;
			uniform vector _StarPos;
			uniform float _BrightThreshold;

            struct VertexInput {
                float4 vertex : POSITION;
            };
            
			struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

			// Convert from one range to another
			float map(float s, float a1, float a2, float b1, float b2)
			{
				return b1 + (s - a1)*(b2 - b1) / (a2 - a1);
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
			float GetBright(float4 color) {
				return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
			}

            float4 frag(VertexOutput i) : COLOR {                
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				
				// Emissive:
                float3 finalColor = lerp(_SkyColor.rgb, _HorizonColor.rgb, pow((1.0 - max(0, dot(viewDirection, float3(0,-1, _HorizonLevel)))), 7.0));

				float brightness = GetBright(_SkyColor);
				

				// Stars
				if (brightness < _BrightThreshold) {
					float starsThreshold = 0.99; // More stars will appear the lower the threshold
					float twinkleThreshold = 0.0097; // The a star that passes the sum of twinkle threshold and star threshold will twinkle
					float minShine = 0.006;

					// We generate a random value between 0 and 1
					float star_intensity = Noise3d(normalize(i.posWorld.xyz));

					// And we apply a threshold to keep only the brightest areas
					if (star_intensity - twinkleThreshold >= starsThreshold) {
						// Twinkly star!
						float minIntensity = starsThreshold + twinkleThreshold;
						float intensityRange = (star_intensity - minIntensity) * (1.0 / minIntensity);  // Range of 0..1
						float extraIntensity = lerp(0.0070, minShine, intensityRange); // How much to add to star intensity

						// Extra intensity wave change as a factor of time (twinkle effect)
						float currExtraInt = lerp(extraIntensity, minShine, abs(sin(map(intensityRange, 0, 1, 4.5, 50) *_Time[1])));

						// Light brighter by intensity range. Change light ferquencies between stars
						star_intensity = pow((star_intensity - starsThreshold + currExtraInt) / (1.0 - starsThreshold), 5) *
							(-brightness + _BrightThreshold);
						finalColor += float3(star_intensity, star_intensity, star_intensity);
					}
					else if (star_intensity >= starsThreshold) {
						// We compute the star intensity
						star_intensity = pow((star_intensity - starsThreshold + minShine) / (1.0 - starsThreshold), 5) * (-brightness + _BrightThreshold);
						finalColor += float3(star_intensity, star_intensity, star_intensity);
					}
				}

                return fixed4(finalColor,1);
				
            }
            ENDCG
        }
    }
}
