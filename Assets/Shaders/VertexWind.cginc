sampler2D _WindTex;
float4 _WindParams;
float _WindStrength;

float3x3 xRotation3dRadians(float rad) 
{
	float s = sin(rad);
	float c = cos(rad);
	return float3x3(
	    1, 0, 0,
	    0, c, s,
	    0, -s, c);
}

float3 WindOffset(float3 posOS)
{
	float3 worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

	float wind = tex2Dlod(_WindTex, float4(worldPos.xz * _WindParams.xy + frac(_WindParams.zw * _Time.y), 0, 0));
	wind = (wind * 2 - 1) * _WindStrength;
	// return posOS + wind;
	return mul(xRotation3dRadians(wind), posOS);
}