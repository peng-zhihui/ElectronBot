Shader "Battlehub/RTHandles/RawImage"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Width("Width", Float) = 96
		_Height("Height", Float) = 96	
		_PivotAndAnchor("Pivot And Anchor", Vector) = (0.5, 0.5, 0.5, 0.5)
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent"  "Queue" = "Transparent"  }
		Blend One OneMinusSrcAlpha
		ZWrite Off
		ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			
            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Width;
			float _Height;
			float4 _PivotAndAnchor;
			
            v2f vert (appdata v)
            {
                v2f o;

				float4 vertex = v.vertex;
				
				if (_ProjectionParams.x < 0)
				{
					vertex.y = -vertex.y;
				}
				
				float2 pivot = -_PivotAndAnchor.xy + 0.5f;
				float2 anchor = _PivotAndAnchor.zw * 2 - 1;
				float2 size = 2 * float2((_ScreenParams.z - 1) * _Width, (_ScreenParams.w - 1) * _Height);

				vertex.xy += pivot + float2(1, 1);
				vertex.xy = vertex.xy * size + anchor - size;
								
				o.vertex = vertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
