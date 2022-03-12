// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Battlehub/RTHandles/VertexColorClipUsingClipPlane" {
	Properties
	{
		_ZWrite("ZWrite", Float) = 0.0
		_ZTest("ZTest", Float) = 0.0
		_Cull("Cull", Float) = 0.0
	}
	SubShader
	{
		
		Tags{ "Queue" = "Geometry+5" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Pass
		{
			Cull[_Cull]
			ZTest Off
			ZWrite Off
		
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 

			struct vertexInput {
				float4 vertex : POSITION;
				float4 color: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float3 planeWorldNorm : TEXCOORD0;
				float3 planeWorldPos : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 color: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			inline float4 GammaToLinearSpace(float4 sRGB)
			{
				if (IsGammaSpace())
				{
					return sRGB;
				}
				return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
			}

			float distanceToPlane(float3 planePosition, float3 planeNormal, float3 pointInWorld)
			{
				float3 w = -(planePosition - pointInWorld);
				return (
					planeNormal.x * w.x +
					planeNormal.y * w.y +
					planeNormal.z * w.z
					) / sqrt(
						planeNormal.x * planeNormal.x +
						planeNormal.y * planeNormal.y +
						planeNormal.z * planeNormal.z
					);
			}

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				output.pos = UnityObjectToClipPos(input.vertex);

				output.planeWorldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
				output.planeWorldNorm = normalize(_WorldSpaceCameraPos - output.planeWorldPos);
				
				output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
			
				output.color = GammaToLinearSpace(input.color);
				output.color.a = input.color.a;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(input);
				clip(distanceToPlane(input.planeWorldPos, input.planeWorldNorm, input.worldPos));
				return  input.color;
			}	

			ENDCG
		}
	}
}