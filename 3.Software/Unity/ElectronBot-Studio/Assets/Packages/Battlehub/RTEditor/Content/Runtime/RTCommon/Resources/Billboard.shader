Shader "Battlehub/RTCommon/Billboard"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture Image", 2D) = "white" {}
		_Cutoff("Cutoff", Float) = 0.01
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZTest LEqual
		ZWrite On

		Tags{ "Queue" = "Transparent+20" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert  
			#pragma fragment frag 
			#pragma multi_compile_instancing

			// User-specified uniforms            
			uniform sampler2D _MainTex;
			fixed4 _Color;
			float _Cutoff;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 tex : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				float scaleX = length(mul(unity_ObjectToWorld, float4(1.0, 0.0, 0.0, 0.0)));
				float scaleY = length(mul(unity_ObjectToWorld, float4(0.0, 1.0, 0.0, 0.0)));

				float4 mv = float4(UnityObjectToViewPos(float3(0.0, 0.0, 0.0)), 1.0);
				output.pos = mul(UNITY_MATRIX_P, mv
					- float4(input.vertex.x * scaleX, input.vertex.y * scaleY, 0.0, 0.0));
				output.tex = input.tex;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(input);
				float4 color = _Color * tex2D(_MainTex, float2(input.tex.xy));
				clip(color.a - _Cutoff);
				return color;
			}

			ENDCG
		}
	}
}


