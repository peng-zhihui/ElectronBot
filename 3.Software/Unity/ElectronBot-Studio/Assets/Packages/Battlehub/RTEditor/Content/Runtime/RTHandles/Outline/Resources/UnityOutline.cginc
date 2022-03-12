
#ifndef DRAW_COLOR
#define DRAW_COLOR 1
#endif
 
#include "UnityCG.cginc"
 
sampler2D _MainTex;
float4 _MainTex_ST;
float _DoClip;
fixed _Cutoff;
 
struct appdata_t
{
    float4 vertex   : POSITION;
    float2 texcoord : TEXCOORD0;
};
 
struct v2f
{
    float4 vertex        : SV_POSITION;
    float2 texcoord      : TEXCOORD0;
};
 
v2f vert (appdata_t IN)
{
    v2f OUT;
	OUT.vertex = UnityObjectToClipPos(IN.vertex);
    OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
    return OUT;
}
 
fixed4 frag (v2f IN) : SV_Target
{
    if (_DoClip)
    {
        fixed4 col = tex2D( _MainTex, IN.texcoord);
        clip(col.a - _Cutoff);
    }
    return DRAW_COLOR;
}
 