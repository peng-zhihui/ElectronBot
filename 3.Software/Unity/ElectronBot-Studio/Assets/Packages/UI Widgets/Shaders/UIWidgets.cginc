#ifndef UIWIDGETS_INCLUDED
#define UIWIDGETS_INCLUDED

// Convert from linear colorspace to gamma.
// linRGB - color in the linear colorspace.
inline half4 LinearToGammaSpace4(half4 linRGB)
{
	linRGB = max(linRGB, half4(0.h, 0.h, 0.h, 0.h));
	// An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

	// Exact version, useful for debugging.
	//return half4(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b), LinearToGammaSpaceExact(linRGB.a));
}

// Convert from gamma colorspace to linear.
// sRGB - color in the gamma colorspace.
inline half4 GammaToLinearSpace4(half4 sRGB)
{
	// Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);

	// Precise version, useful for debugging.
	//return half4(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b), GammaToLinearSpaceExact(sRGB.a));
}

// Convert hue to base rgb info.
// H - H parameter from the HSV color.
inline float4 Hue(float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float4(R,G,B,1));
}

// Get color of the specified point of the HSV circle.
// pos - point, relative to circle center.
// value - V parameter from HSV.
// quality - circle edges quality.
inline float4 CircleHSV(in float2 pos, in float value, in float quality)
{
	float pi2 = 6.28318530718;

	float saturation = sqrt(pos.x * pos.x * 4.0 + pos.y * pos.y * 4.0);
	float alpha = 1.0 - smoothstep(1.0 - quality, 1.0 + quality, dot(pos, pos) * 4.0);

	float hue = atan2(pos.x, pos.y) / pi2;
	if (hue < 0)
	{
		hue += 1;
	}

	return saturate(float4(hue, saturation, value, alpha));
}

// Convert hsv color to rgb.
// HSV - HSV color.
inline float4 HSVtoRGB(in float4 HSV)
{
	float4 result = ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
	result.a = HSV.a;
	return result;
}
#endif