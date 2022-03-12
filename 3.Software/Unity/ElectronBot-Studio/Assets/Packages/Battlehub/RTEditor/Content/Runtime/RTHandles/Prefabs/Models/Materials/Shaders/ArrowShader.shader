Shader "Battlehub/RTHandles/Models/ArrowShader"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		[Enum(Off,0,On,1)]_ZWrite("ZWrite", Float) = 1.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
	
		Pass
		{
			ZWrite[_ZWrite]
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color: COLOR;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 color: COLOR;
			};

			fixed4 _Color;


			inline float4 GammaToLinearSpace(float4 sRGB)
			{
				if (IsGammaSpace())
				{
					return sRGB;
				}
				return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);
			}

			vertexOutput vert(vertexInput input)
			{
				float3 worldNorm = UnityObjectToWorldNormal(input.normal);
				float3 viewNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
				vertexOutput output;
		
				output.pos = UnityObjectToClipPos(input.vertex);
				output.color = input.color * 1.5 * dot(viewNorm, float3(0, 0, 1));
				output.color =  GammaToLinearSpace(output.color);
				output.color.a = input.color.a;


				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return _Color * input.color;
			}
			ENDCG
		}
	}
}
