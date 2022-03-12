Shader "Battlehub/RTHandles/BoxSelectionShader"
{
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Pass
	{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _SelectionColor)
			#define _SelectionColor_arr Props
			UNITY_INSTANCING_BUFFER_END(Props)
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return UNITY_ACCESS_INSTANCED_PROP(_SelectionColor_arr, _SelectionColor);
			}
		ENDCG
		}
	}


}