sampler2D tex : register(S0);
float2 pixel;
float4 FillerColor = float4(0.34, 0.44, 0.8, 1);
float4 BorderColor = float4(1, 1, 1, 1);
float3 BlurColor = float3(0.3, 0.56, 0.97);

// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 t = tex2D(tex, uv);

	if (tex2D(tex, uv).a != 0)
			return FillerColor;

	if (tex2D(tex, uv + float2(pixel.x, 0)).a != 0 ||
		tex2D(tex, uv + float2(-pixel.x, 0)).a != 0 ||
		tex2D(tex, uv + float2(0, pixel.y)).a != 0 ||
		tex2D(tex, uv + float2(0, -pixel.y)).a != 0)
			return BorderColor;

    float4 c = 0;
	float r = 11;
	float rr = r * r;
	float rr2 = rr / 8;
    for (int x = -r; x < r; x += 2)
	{
		for (int y = -r; y < r; y += 2)
		{
			c += tex2D(tex, uv + float2(pixel.x * x, pixel.y * y)) * (1 - (x + y) / rr) / rr2;
		}
	}

	return c * float4(BlurColor, 1);
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