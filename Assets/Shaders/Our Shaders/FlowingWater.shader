/*  built off DistortionFlow shader 
	from which tutorial is from https://catlikecoding.com/unity/tutorials/flow/texture-distortion/ 
	The refraction is from  https://catlikecoding.com/unity/tutorials/flow/looking-through-water/ */

Shader "Custom/FlowingWater" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _FlowMap("Flow (RG, A noise)", 2D) = "black" {}
		[NoScaleOffset] _DerivHeightMap("Deriv (AG) Height (B)", 2D) = "black" {}
		_WaterFogColor("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity("Water Fog Density", Range(0, 2)) = 0.1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Tiling("Tiling", Float) = 1
		_Speed("Speed", Float) = 1
		_FlowStrength("Flow Strength", Float) = 1
		_HeightScale("Height Scale, Constant", Float) = 0.25
		_HeightScaleModulated("Height Scale, Modulated", Float) = 0.75
			//refraction time	
		_RefractionStrength("Refraction Strength", Range(0, 1)) = 0.25
	}
		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			GrabPass { "_WaterBackground" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard alpha finalcolor:ResetAlpha
			#pragma target 3.0

			sampler2D _MainTex, _FlowMap,_DerivHeightMap;

			sampler2D _CameraDepthTexture, _WaterBackground;
			
			float4 _CameraDepthTexture_TexelSize;

			float _Tiling, _Speed, _FlowStrength;
			//height stuff
			float _HeightScale, _HeightScaleModulated;
			// refraction time
			float _RefractionStrength;

			

			struct Input {
				float2 uv_MainTex;
				float4 screenPos;
			};

			half _Glossiness;
			
			fixed4 _Color;
			
			float3 _WaterFogColor;
			
			float _WaterFogDensity;
			
			float3 UnpackDerivativeHeight(float4 textureData) {
				float3 dh = textureData.agb;
				
				dh.xy = dh.xy * 2 - 1;
				
				return dh;
			}
			//disables default blending post calculation
			void ResetAlpha(Input IN, SurfaceOutputStandard o, inout fixed4 color) {
				color.a = 1;
			}
			float3 FlowUVW(float2 uv, float2 flowVector, float tiling, float time, bool flowB) {
				float phaseOffset = flowB ? 0.5 : 0;
				
				float progress = frac(time + phaseOffset);
				
				float3 uvw;
				
				uvw.xy = uv - flowVector * progress ;
				
				uvw.xy *= tiling;
				
				uvw.xy += phaseOffset;
				
				uvw.xy += (time - progress);
				
				uvw.z = 1 - abs(1 - 2 * progress);
				
				return uvw;
			}
			//used to fix minor issues around objects piercing water
			float2 AlignWithGrabTexel(float2 uv) {
				#if UNITY_UV_STARTS_AT_TOP
				if (_CameraDepthTexture_TexelSize.y < 0) {
					uv.y = 1 - uv.y;
				}
				#endif

				return (floor(uv * _CameraDepthTexture_TexelSize.zw) + 0.5) * abs(_CameraDepthTexture_TexelSize.xy);
			}

			
			float3 ColorBelowWater(float4 screenPos, float3 tangentSpaceNormal) {
				//uv is used for refraction, we set the offset to create the wiggle with the flow of the water
				// the flow is created by the tangentspacenormal
				float2 uvOffset = tangentSpaceNormal.xy * _RefractionStrength;
				
				uvOffset.y *= _CameraDepthTexture_TexelSize.z * abs(_CameraDepthTexture_TexelSize.y);
				//uv offset is made to offset the fact that the image can be wider than it is tall.
				float2 uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
				
				//used to ensure we only refract things in the water 
				#if UNITY_UV_STARTS_AT_TOP
				if (_CameraDepthTexture_TexelSize.y < 0) {
					uv.y = 1 - uv.y;
				}
				#endif

				float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));

				float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);

				float depthDifference = backgroundDepth - surfaceDepth;
				
				
				uvOffset *= saturate(depthDifference);
				
				uv = AlignWithGrabTexel((screenPos.xy + uvOffset) / screenPos.w);
				
				backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
				//because we're looking through a 2d plane the depth has to be calculated
				depthDifference = backgroundDepth - surfaceDepth;

				float3 backgroundColor = tex2D(_WaterBackground, uv).rgb;
				
				float fogFactor = exp2(-_WaterFogDensity * depthDifference);

				return lerp(_WaterFogColor, backgroundColor, fogFactor);
			}	
			//this creates the flow pattern using distortion maps, derivative maps and noise maps
			//speed, tiling, strength is all controllable
			void surf(Input IN, inout SurfaceOutputStandard o) {
				float3 flow = tex2D(_FlowMap, IN.uv_MainTex).rgb;
				
				flow.xy = flow.xy * 2 - 1;
				
				flow *= _FlowStrength;

				float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;
				
				float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
						
				float time = _Time.y * _Speed + noise;

				float3 uvwA = FlowUVW(IN.uv_MainTex, flow.xy, _Tiling, time, false);
				
				float3 uvwB = FlowUVW(IN.uv_MainTex, flow.xy, _Tiling, time, true);

				float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * finalHeightScale);
				
				float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * finalHeightScale);
				
				o.Normal = normalize(float3(-(dhA.xy + dhB.xy), 1));

				fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
				
				fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

				fixed4 c = (texA + texB) * _Color;

				o.Albedo = c.rgb;

				o.Smoothness = _Glossiness;
				
				o.Alpha = c.a;
				
				o.Emission = ColorBelowWater(IN.screenPos, o.Normal) * (1 - c.a);
			}



			ENDCG
		}

			
}