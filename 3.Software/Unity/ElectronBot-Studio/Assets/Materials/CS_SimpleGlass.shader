Shader "Ciconia Studio/CS_Glass/Builtin/SimpleGlass"
{
	Properties
	{
		[Space(10)][Header(Main Properties)][Space(15)]_Color("Color", Color) = (0,0,0,0)
		_Saturation("Saturation", Float) = 0
		_Brightness("Brightness", Range( 1 , 8)) = 1
		[Space(15)]_Metallic("Metallic", Range( 0 , 2)) = 0.2
		_Glossiness("Smoothness", Range( 0 , 1)) = 0.5
		[Space(35)]_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 0.3
		_Refraction("Refraction", Range( 0 , 2)) = 1.1
		[Space(45)][Header(Self Illumination)][Space(15)]_Intensity("Intensity", Range( 1 , 10)) = 1
		[Space(45)][Header(Reflection Properties) ][Space(15)]_ColorCubemap("Color ", Color) = (1,1,1,1)
		[HDR]_CubeMap("Cube Map", CUBE) = "black" {}
		_ReflectionIntensity("Reflection Intensity", Float) = 1
		_BlurReflection("Blur", Range( 0 , 8)) = 0
		[Space(15)]_ColorFresnel1("Color Fresnel", Color) = (1,1,1,0)
		[ToggleOff(_USECUBEMAP_OFF)] _UseCubemap("Use Cubemap", Float) = 1
		_FresnelStrength("Fresnel Strength", Range( 0 , 8)) = 0
		_PowerFresnel("Power", Float) = 0.5
		[Space(45)][Header(Transparency Properties)][Space(15)]_Opacity("Opacity", Range( 0 , 1)) = 1
		[Space(10)][Toggle]_FalloffOpacity("Falloff Opacity", Float) = 0
		[Toggle]_Invert("Invert", Float) = 0
		[Space(10)]_FalloffOpacityIntensity("Falloff Intensity", Range( 0 , 1)) = 1
		_PowerFalloffOpacity("Power", Float) = 1
		[Space(45)][Header(Fade Properties)][Space(15)]_Fade("Fade", Range( 0 , 1)) = 0
		[Space(10)][Toggle]_FalloffFade("Falloff", Float) = 0
		[Toggle]_InvertFresnelFade("Invert", Float) = 0
		[Space(10)]_GradientFade("Falloff Intensity", Range( 0 , 1)) = 1
		_PowerFalloffFade("Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Glass"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ "_ScreenGrab0" }
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USECUBEMAP_OFF
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldRefl;
			float4 screenPos;
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _Brightness;
		uniform float4 _Color;
		uniform float _Saturation;
		uniform float4 _ColorFresnel1;
		uniform float _PowerFresnel;
		uniform float _FresnelStrength;
		uniform samplerCUBE _CubeMap;
		uniform float _BlurReflection;
		uniform float _ReflectionIntensity;
		uniform float4 _ColorCubemap;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _ScreenGrab0 )
		uniform float _Refraction;
		uniform float _FalloffOpacity;
		uniform float _Intensity;
		uniform float _Opacity;
		uniform float _Invert;
		uniform float _FalloffOpacityIntensity;
		uniform float _PowerFalloffOpacity;
		uniform float _Metallic;
		uniform float _Glossiness;
		uniform float _FalloffFade;
		uniform float _Fade;
		uniform float _InvertFresnelFade;
		uniform float _GradientFade;
		uniform float _PowerFalloffFade;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 tex2DNode2 = UnpackScaleNormal( tex2D( _BumpMap, uv0_BumpMap ), _BumpScale );
			float3 Normal101 = tex2DNode2;
			o.Normal = Normal101;
			float clampResult239 = clamp( _Saturation , -1.0 , 100.0 );
			float3 desaturateInitialColor211 = _Color.rgb;
			float desaturateDot211 = dot( desaturateInitialColor211, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar211 = lerp( desaturateInitialColor211, desaturateDot211.xxx, -clampResult239 );
			float4 temp_output_303_0 = CalculateContrast(_Brightness,float4( desaturateVar211 , 0.0 ));
			float4 AlbedoAmbient117 = temp_output_303_0;
			o.Albedo = AlbedoAmbient117.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV163 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode163 = ( -0.05 + 1.0 * pow( 1.0 - fresnelNdotV163, _PowerFresnel ) );
			float4 temp_output_465_0 = ( _ColorFresnel1 * fresnelNode163 * _FresnelStrength );
			float3 NormalmapXYZ170 = tex2DNode2;
			float4 texCUBENode6 = texCUBElod( _CubeMap, float4( WorldReflectionVector( i , NormalmapXYZ170 ), _BlurReflection) );
			#ifdef _USECUBEMAP_OFF
				float4 staticSwitch461 = temp_output_465_0;
			#else
				float4 staticSwitch461 = ( temp_output_465_0 * texCUBENode6 );
			#endif
			float4 clampResult468 = clamp( staticSwitch461 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 clampResult530 = clamp( ( texCUBENode6 * ( texCUBENode6.a * _ReflectionIntensity ) * _ColorCubemap ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Cubmap179 = ( clampResult468 + clampResult530 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float4 screenColor381 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_ScreenGrab0,( (ase_grabScreenPosNorm).xyzw + float4( (( ( Normal101 + mul( float4( ase_normWorldNormal , 0.0 ), UNITY_MATRIX_V ).xyz ) * (-1.0 + (_Refraction - 0.0) * (1.0 - -1.0) / (2.0 - 0.0)) )).xyz , 0.0 ) ).xy);
			float4 GrabSreenRefraction385 = screenColor381;
			float lerpResult483 = lerp( 0.0 , _Intensity , _Opacity);
			float lerpResult68 = lerp( -3.0 , 0.0 , _FalloffOpacityIntensity);
			float fresnelNdotV25 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode25 = ( lerpResult68 + _PowerFalloffOpacity * pow( 1.0 - fresnelNdotV25, (( 1.0 + -lerpResult483 ) + (1.0 - 0.0) * (lerpResult483 - ( 1.0 + -lerpResult483 )) / (1.0 - 0.0)) ) );
			float clampResult45 = clamp( (( _Invert )?( ( 1.0 - fresnelNode25 ) ):( fresnelNode25 )) , 0.0 , 1.0 );
			float Opacity87 = (( _FalloffOpacity )?( clampResult45 ):( ( 1.0 - lerpResult483 ) ));
			o.Emission = ( Cubmap179 + ( GrabSreenRefraction385 * ( 1.0 - Opacity87 ) ) ).rgb;
			float Metallic110 = _Metallic;
			o.Metallic = Metallic110;
			float Roughness111 = _Glossiness;
			o.Smoothness = Roughness111;
			float lerpResult513 = lerp( -3.0 , 0.0 , _GradientFade);
			float fresnelNdotV515 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode515 = ( lerpResult513 + _PowerFalloffFade * pow( 1.0 - fresnelNdotV515, (( 1.0 + -( 1.0 - _Fade ) ) + (1.0 - 0.0) * (( 1.0 - _Fade ) - ( 1.0 + -( 1.0 - _Fade ) )) / (1.0 - 0.0)) ) );
			float clampResult518 = clamp( (( _InvertFresnelFade )?( ( 1.0 - fresnelNode515 ) ):( fresnelNode515 )) , 0.0 , 1.0 );
			float Fade450 = (( _FalloffFade )?( clampResult518 ):( ( 1.0 - _Fade ) ));
			o.Alpha = Fade450;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}