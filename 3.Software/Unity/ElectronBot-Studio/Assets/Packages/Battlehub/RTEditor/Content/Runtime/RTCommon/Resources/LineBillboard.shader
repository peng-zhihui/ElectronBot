Shader "Battlehub/RTCommon/LineBillboard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Scale("Scale", Range(0, 20)) = 1
		_HandleZTest("_HandleZTest", Int) = 8
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent"  "RenderType" = "Transparent" "DisableBatching" = "True" "ForceNoShadowCasting" = "True" "IgnoreProjector" = "True" }

		Blend SrcAlpha OneMinusSrcAlpha
		Lighting Off
		ZTest[_HandleZTest]
		ZWrite On
		Cull Off
		Offset -1, -1

		Pass
		{
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			#define ORTHO (1 - UNITY_MATRIX_P[3][3])

			const float ORTHO_CAM_OFFSET = .0001;
			float _Scale;

			float4 ClipToScreen(float4 v)
			{
				v.xy /= v.w;
				v.xy = v.xy * .5 + .5;
				v.xy *= _ScreenParams.xy;
				return v;
			}

			float4 ScreenToClip(float4 v)
			{
				v.z -= ORTHO_CAM_OFFSET * ORTHO;
				v.xy /= _ScreenParams.xy;
				v.xy = (v.xy - .5) / .5;
				v.xy *= v.w;
				return v;
			}

			inline float4 GammaToLinearSpace(float4 sRGB)
			{
				if (IsGammaSpace())
				{
					return sRGB;
				}
				return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
			}

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.pos = float4(UnityObjectToViewPos(v.vertex.xyz), 1);
				o.pos *= .99;
				o.pos = mul(UNITY_MATRIX_P, o.pos);
				
				o.pos = ClipToScreen(o.pos);
				o.color = GammaToLinearSpace(v.color);
				return o;
			}

			[maxvertexcount(4)]
			void geo(line v2f p[2], inout TriangleStream<v2f> triStream)
			{
				float2 perp = normalize(float2(-(p[1].pos.y - p[0].pos.y), p[1].pos.x - p[0].pos.x)) * _Scale;

				v2f geo_out;
				UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], geo_out);
				UNITY_TRANSFER_INSTANCE_ID(p[0], geo_out);

				geo_out.pos = ScreenToClip(float4(p[1].pos.x + perp.x, p[1].pos.y + perp.y, p[1].pos.z, p[1].pos.w));
				geo_out.color = p[1].color;
				triStream.Append(geo_out);

				geo_out.pos = ScreenToClip(float4(p[1].pos.x - perp.x, p[1].pos.y - perp.y, p[1].pos.z, p[1].pos.w));
				geo_out.color = p[1].color;
				triStream.Append(geo_out);

				geo_out.pos = ScreenToClip(float4(p[0].pos.x + perp.x, p[0].pos.y + perp.y, p[0].pos.z, p[0].pos.w));
				geo_out.color = p[0].color;
				triStream.Append(geo_out);

				geo_out.pos = ScreenToClip(float4(p[0].pos.x - perp.x, p[0].pos.y - perp.y, p[0].pos.z, p[0].pos.w));
				geo_out.color = p[0].color;
				triStream.Append(geo_out);
			}

			fixed4 frag(v2f i) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(i); 
				return i.color * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
			}

			ENDCG
		}
	}
}
