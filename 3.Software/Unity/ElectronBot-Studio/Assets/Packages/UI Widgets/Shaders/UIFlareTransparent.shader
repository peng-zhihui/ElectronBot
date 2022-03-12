// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Flare in the sprite space with sprite transparency support.
// Flare support sprite texture transparency and displayed only on opaque parts.
// Should be used only _Flare* properties
// other properties should have the default value to be compatible with Unity UI.

Shader "Custom/New UI Widgets/UIFlareTransparent"
{
	Properties
	{
		// Sprite texture
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		// Tint
		_Color("Tint", Color) = (1,1,1,1)
		// flare color
		_FlareColor("Flare Color", Color) = (1,1,1,1)
		// flare size in fraction of texture size
		_FlareSize("Flare Size", Float) = 0.2
		// flare speed
		_FlareSpeed("Flare Speed", Float) = 0.2
		// flare delay in seconds
		_FlareDelay("Flare Delay", Float) = 0.0

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UIWidgets.cginc"

			#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
			#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				half4  mask : TEXCOORD2;
				float2 texcoord1     : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			float4 _MainTex_ST;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;

			float4 _FlareColor;
			float _FlareSize;
			float _FlareSpeed;
			float _FlareDelay;
			CBUFFER_END

			v2f vert(appdata_t v)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				float4 vPosition = UnityObjectToClipPos(v.vertex);
				OUT.worldPosition = v.vertex;
				OUT.vertex = vPosition;

				float2 pixelSize = vPosition.w;
				pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

				float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
				float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
				OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

				OUT.color = v.color * _Color;

				OUT.texcoord1 = v.texcoord1.xy;

				return OUT;
			}

			float flare_distance(float center, float size, float pos, float delay)
			{
				float half_size = size / 2.0;
				float left = center - half_size;
				float right = center + half_size;
				if ((left < 0) && (pos > (left + delay)))
				{
					pos -= delay;
				}
				else if ((right > 1.0) && (pos < (right - delay)))
				{
					pos += delay;
				}

				return abs(smoothstep(left, right, pos) - 0.5) * 2.0;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float delay = 1.0 + (_FlareDelay * _FlareSpeed);
				float flare_center = frac(_Time.y * (_FlareSpeed / delay)) * delay;
				float rate = flare_distance(flare_center, _FlareSize, IN.texcoord.x, delay);

				float4 color = IN.color * (UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, IN.texcoord) + _TextureSampleAdd);

				float3 flare_temp = lerp(color.rgb, _FlareColor.rgb, _FlareColor.a);
				float4 flare = float4(flare_temp.r, flare_temp.g, flare_temp.b, 1);

				#if defined(UIWIDGETS_COLORSPACE_GAMMA) || defined(UNITY_COLORSPACE_GAMMA)
				color.rgb = lerp(flare.rgb, color.rgb, rate);
				#else
				color.rgb = lerp(LinearToGammaSpace4(flare).rgb, color.rgb, rate);
				color = GammaToLinearSpace4(color);
				#endif

				#ifdef UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color.a *= m.x * m.y;
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
