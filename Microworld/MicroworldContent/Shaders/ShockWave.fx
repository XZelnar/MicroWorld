// texture we are rendering
sampler2D tex : register(S0);

float2 center = float2(50, 50);
float thickness = 90;
float innerRadius = 200;

// pixel shader function
float4 main(float2 uv : TEXCOORD, float4 pixelPos: VPOS, float4 color : COLOR0) : COLOR
{
	float dist = sqrt(pow(pixelPos.x - center.x, 2) + pow(pixelPos.y - center.y, 2));
	if (dist < innerRadius || dist > innerRadius + thickness)
		return float4(0, 0, 0, 0);
	dist -= innerRadius;
	float t = thickness / 6;

	float tc;
	if (dist < t * 3)
		tc = dist / t / 3;
	else
		tc = 1 - (dist - t * 3) / t / 3;

	float4 res = float4(tc, tc, tc, tc);

	if (dist < t)
		res.a = dist / t / 2;
	else if (dist < 2 * t)
		res.a = 0.5f - (dist - t) / t / 2;
	else if (dist < 4 * t)
		res.a = 0;
	else
	{
		dist -= t * 4;
		if (dist < t)
			res.a = dist / t / 2;
		else
			res.a = 0.5f - (dist - t) / t / 2;
	}

	return res * color;
}

//================================================VERTEX
float4x4 MatrixTransform;

void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float2 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, MatrixTransform);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
        VertexShader = compile vs_3_0 SpriteVertexShader();
    }
}