﻿// Toony Colors Pro+Mobile 2
// (c) 2014-2020 Jean Moreno

Shader "Toony Colors Pro 2/User/Triplanar"
{
	Properties
	{
		[TCP2HeaderHelp(Base)]
		_BaseColor ("Color", Color) = (1,1,1,1)
		[TCP2ColorNoAlpha] _HColor ("Highlight Color", Color) = (0.75,0.75,0.75,1)
		[TCP2ColorNoAlpha] _SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
		[TCP2Separator]

		[TCP2Header(Ramp Shading)]
		
		[TCP2Gradient] _Ramp			("Ramp Texture (RGB)", 2D) = "gray" {}
		[TCP2Separator]
		
		[TCP2HeaderHelp(Specular)]
		[TCP2ColorNoAlpha] _SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_SpecularSmoothness ("Smoothness", Float) = 0.2
		[TCP2Separator]
		
		[TCP2HeaderHelp(Triplanar Mapping)]
		[NoScaleOffset] _TriGround ("Ground", 2D) = "white" {}
		[NoScaleOffset] _TriSide ("Walls", 2D) = "white" {}
		[TCP2Vector4Floats(Contrast X,Contrast Y,Contrast Z,Smoothing,1,16,1,16,1,16,0.05,1)] _TriplanarBlendStrength ("Triplanar Parameters", Vector) = (2,8,2,0.5)
		[TCP2Separator]
		
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType"="Opaque"
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		
		// Built-in renderer (CG) to SRP (HLSL) bindings
		#define UnityObjectToClipPos TransformObjectToHClip
		#define _WorldSpaceLightPos0 _MainLightPosition
		
		// Uniforms
		CBUFFER_START(UnityPerMaterial)
		
		// Shader Properties
		sampler2D _TriGround;
		sampler2D _TriSide;
		fixed4 _BaseColor;
		float _SpecularSmoothness;
		fixed4 _SpecularColor;
		fixed4 _SColor;
		fixed4 _HColor;
		sampler2D _Ramp;
		float4 _TriplanarBlendStrength;
		CBUFFER_END

		ENDHLSL
		Pass
		{
			Name "Main"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material keywords
			//#pragma shader_feature _ALPHATEST_ON
			#pragma multi_compile _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			// -------------------------------------

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 tangent      : TANGENT;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float2 pack0 : TEXCOORD3; /* pack0.xy = texcoord0 */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Texture Coordinates
				output.pack0.xy = input.texcoord0.xy;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// normal
				output.normal = NormalizeNormalPerVertex(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				// Shader Properties Sampling
				float4 __mainColor = ( _BaseColor.rgba );
				float __specularSmoothness = ( _SpecularSmoothness );
				float3 __specularColor = ( _SpecularColor.rgb );
				float3 __shadowColor = ( _SColor.rgb );
				float3 __highlightColor = ( _HColor.rgb );
				float __ambientIntensity = ( 1.0 );

				// main texture
				half3 albedo = half3(1,1,1);
				half alpha = 1;
				half3 emission = half3(0,0,0);
				half4 albedoAlpha = half4(albedo, alpha);
				
				//Triplanar Texture Blending
				half2 uv_ground = positionWS.xz;
				half2 uv_sideX = positionWS.zy;
				half2 uv_sideZ = positionWS.xy;
				float3 triplanarNormal = normalWS;
				
				half3 objPositionInWorld = unity_ObjectToWorld._m03_m13_m23;
				uv_ground.xy -= objPositionInWorld.xz;
				uv_sideX.xy -= objPositionInWorld.zy;
				uv_sideZ.xy -= objPositionInWorld.xy;
							
				//ground
				half4 triplanar = ( tex2D(_TriGround, uv_ground).rgba );
				
				//walls
				fixed4 tex_sideX = ( tex2D(_TriSide, uv_sideX).rgba );
				fixed4 tex_sideZ = ( tex2D(_TriSide, uv_sideZ).rgba );
				
				//blending
				half3 blendWeights = pow(abs(triplanarNormal), _TriplanarBlendStrength.xyz / _TriplanarBlendStrength.w);
				blendWeights = blendWeights / (blendWeights.x + abs(blendWeights.y) + blendWeights.z);
				triplanar = tex_sideX * blendWeights.x + triplanar * blendWeights.y + tex_sideZ * blendWeights.z;
				albedoAlpha *= triplanar;
				albedo = albedoAlpha.rgb;
				alpha = albedoAlpha.a;
				
				albedo *= __mainColor.rgb;

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord = input.shadowCoord;
			#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
			#else
				float4 shadowCoord = float4(0, 0, 0, 0);
			#endif
				Light mainLight = GetMainLight(shadowCoord);

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;
				half atten = mainLight.shadowAttenuation;

				half ndl = max(0, dot(normalWS, lightDir));
				half3 ramp;
				half2 rampUv = ndl.xx * 0.5 + 0.5;
				ramp = tex2D(_Ramp, rampUv).rgb;

				// apply attenuation
				ramp *= atten;

				half3 color = half3(0,0,0);
				half3 accumulatedRamp = ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
				half3 accumulatedColors = ramp * lightColor.rgb;

				//Blinn-Phong Specular
				half3 h = normalize(lightDir + viewDirWS);
				float ndh = max(0, dot (normalWS, h));
				float spec = pow(ndh, __specularSmoothness * 128.0);
				spec *= ndl;
				spec *= atten;
				
				//Apply specular
				color.rgb += spec * lightColor.rgb * __specularColor;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				int additionalLightsCount = GetAdditionalLightsCount();
				for (int i = 0; i < additionalLightsCount; ++i)
				{
					Light light = GetAdditionalLight(i, positionWS);
					half atten = light.shadowAttenuation * light.distanceAttenuation;
					half3 lightDir = light.direction;
					half3 lightColor = light.color.rgb;

					half ndl = max(0, dot(normalWS, lightDir));
					half3 ramp;
					sampler2D rampTexture = _Ramp;
					half2 rampUv = ndl;
					ramp = tex2D(rampTexture, rampUv).rgb;

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

					//Blinn-Phong Specular
					half3 h = normalize(lightDir + viewDirWS);
					float ndh = max(0, dot (normalWS, h));
					float spec = pow(ndh, __specularSmoothness * 128.0);
					spec *= ndl;
					spec *= atten;
					
					//Apply specular
					color.rgb += spec * lightColor.rgb * __specularColor;
				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				half3 shadowColor = (1 - accumulatedRamp.rgb) * __shadowColor;
				accumulatedRamp = accumulatedColors.rgb * __highlightColor + shadowColor;
				color += albedo * accumulatedRamp;

				// ambient or lightmap
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
				half occlusion = 1;
				half3 indirectDiffuse = bakedGI;
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;
				color += indirectDiffuse;

				color += emission;

				return half4(color, alpha);
			}
			ENDHLSL
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE
		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;

			struct Attributes
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float3 pack0 : TEXCOORD0; /* pack0.xyz = positionWS */
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normal);

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

			#if UNITY_REVERSED_Z
				positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#else
				positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
			#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output;
				UNITY_SETUP_INSTANCE_ID(input);
				#if defined(DEPTH_ONLY_PASS)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal);
				output.normal = NormalizeNormalPerVertex(vertexNormalInput.normalWS);
				output.pack0.xyz = vertexInput.positionWS;

				#if defined(DEPTH_ONLY_PASS)
					output.positionCS = TransformObjectToHClip(input.vertex.xyz);
				#elif defined(SHADOW_CASTER_PASS)
					output.positionCS = GetShadowPositionHClip(input);
				#else
					output.positionCS = float4(0,0,0,0);
				#endif

				return output;
			}

			half4 ShadowDepthPassFragment(Varyings input) : SV_TARGET
			{
				#if defined(DEPTH_ONLY_PASS)
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				#endif

				float3 positionWS = input.pack0.xyz;
				float3 normalWS = NormalizeNormalPerPixel(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);
				half3 albedo = half3(1,1,1);
				half alpha = 1;
				half3 emission = half3(0,0,0);
				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// -------------------------------------
			// Material Keywords
			// #pragma shader_feature _ALPHATEST_ON
			// #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			ENDHLSL
		}

		// Depth prepass
		// UsePass "Universal Render Pipeline/Lit/DepthOnly"

	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2022.3.2f1";ver:"2.4.3";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","ENABLE_SRP_BATCHER","DISABLE_SHADOW_RECEIVING","DISABLE_SHADOW_CASTING","TEXTURE_RAMP","TRIPLANAR","SPEC_LEGACY","SPECULAR","TRIPLANAR_OBJ_POS_OFFSET","TEMPLATE_LWRP"];flags:list[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[];customTextures:list[]) */
/* TCP_HASH f58d8d695f4ca9d75a3ce9c20e201857 */
