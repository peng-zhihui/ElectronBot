// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 
Shader "Hidden/UnityOutline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.01
    }
 
    SubShader
    {
        CGINCLUDE
        struct Input
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };
 
        struct Varying
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
 
        Varying vertex(Input input)
        {
            Varying output;
 
            output.position = UnityObjectToClipPos(input.position);
            output.uv = input.uv;
            return output;
        }
        ENDCG
       
        Tags { "RenderType"="Opaque" }
 

        // #0: things that are visible (pass depth). 1 in alpha, 1 in red (SM3.0)
        Pass
        {
            Blend One Zero
            ZTest LEqual
            Cull Off
            ZWrite Off
            // push towards camera a bit, so that coord mismatch due to dynamic batching is not affecting us
            Offset -0.02, 0
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
 
            float _ObjectId;
 
            #define DRAW_COLOR float4(_ObjectId, 1, 1, 1)
            #include "UnityOutline.cginc"
            ENDCG
        }
 
        // #1: all the things, including the ones that fail the depth test. Additive blend, 1 in green, 1 in alpha
        Pass
        {
            Blend One One
            BlendOp Max
            ZTest Always
            ZWrite Off
            Cull Off
            ColorMask GBA
            // push towards camera a bit, so that coord mismatch due to dynamic batching is not affecting us
            Offset -0.02, 0
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
 
            float _ObjectId;
 
            #define DRAW_COLOR float4(0, 0, 1, 1)
            #include "UnityOutline.cginc"
            ENDCG
        }
 
        // #2: separable blur pass, either horizontal or vertical
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
           
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment
            #pragma target 3.0
            #include "UnityCG.cginc"
 
            float2 _BlurDirection;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
 
            // 9-tap Gaussian kernel, that blurs green & blue channels,
            // keeps red & alpha intact.
            static const half4 kCurveWeights[9] = {
                half4(0,0.0204001988,0.0204001988,0),
                half4(0,0.0577929595,0.0577929595,0),
                half4(0,0.1215916882,0.1215916882,0),
                half4(0,0.1899858519,0.1899858519,0),
                half4(1,0.2204586031,0.2204586031,1),
                half4(0,0.1899858519,0.1899858519,0),
                half4(0,0.1215916882,0.1215916882,0),
                half4(0,0.0577929595,0.0577929595,0),
                half4(0,0.0204001988,0.0204001988,0)
            };
 
            half4 fragment(Varying i) : SV_Target
            {
                float2 step = _MainTex_TexelSize.xy * _BlurDirection;
                float2 uv = i.uv - step * 4;
                half4 col = 0;
                for (int tap = 0; tap < 9; ++tap)
                {
                    col += tex2D(_MainTex, uv) * kCurveWeights[tap];
                    uv += step;
                }
                return col;
            }
            ENDCG
        }
 
        // #3: Compare object ids
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
           
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment
            #pragma target 3.0
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
 
            // 8 tap search around the current pixel to
            // see if it borders with an object that has a
            // different object id
            static const half2 kOffsets[8] = {
                half2(-1,-1),
                half2(0,-1),
                half2(1,-1),
                half2(-1,0),
                half2(1,0),
                half2(-1,1),
                half2(0,1),
                half2(1,1)
            };
 
            half4 fragment(Varying i) : SV_Target
            {              
                float4 currentTexel = tex2D(_MainTex, i.uv);

				/*Following code will produce graphical issues if MSAA is enabled*/
				/*Section 1*/
                //if (currentTexel.r == 0)
                //    return currentTexel;
 
                //// if the current texel borders with a
                //// texel that has a differnt object id
                //// set the alpha to 0. This implies an
                //// edge.
                //for (int tap = 0; tap < 8; ++tap)
                //{
                //    float id = tex2D(_MainTex, i.uv + (kOffsets[tap] * _MainTex_TexelSize.xy)).r;
                //    if (id != 0 && id - currentTexel.r != 0)
                //    {
                //        currentTexel.a = 0;
                //    }
                //}
				
                return currentTexel;
            }
            ENDCG
        }

		// #4: final postprocessing pass
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
           
            CGPROGRAM
            #pragma vertex vertex
            #pragma fragment fragment
            #pragma target 3.0
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _OutlineColor;
 
            half4 fragment(Varying i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
               
                bool isSelected = col.a > 0.1;
                bool inFront = col.g > 0.0;
				bool backMask = col.r == 0.0;

				/*Comment out if Section 1 uncommented*/
				float alpha = saturate(col.b * 10);
                //float alpha = saturate(col.b * 1.5);
                if (isSelected)
                {
                    // UnityOutline color alpha controls how much tint the whole object gets
                    alpha = _OutlineColor.a;
                    if (any(i.uv - _MainTex_TexelSize.xy*2 < 0) || any(i.uv + _MainTex_TexelSize.xy*2 > 1))
                        alpha = 1;
                }

				alpha *= 0.99;
				
				/*Comment out if Section 1 uncommented*/
                if (!inFront)
                {
                    alpha *= 0.3;
                }

                if (backMask && isSelected)
                {
                    alpha = 0;
				}

                float4 UnityOutlineColor = float4(_OutlineColor.rgb,alpha);
                return UnityOutlineColor;
            }
            ENDCG
        }
    }
}