// texture we are rendering
sampler2D tex : register(S0);

float2 halfpixel;
float random[] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//l=32
float4 color = float4(1, 0.25f, 0.25f, 1);
float4 transparent = float4(0, 0, 0, 0);
bool horizontal;

// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 col = color;
	int tx, ty, t;
	int xi, yi;

	if (horizontal)
	{
		xi = (int)(uv.x * 64) % 32;
		yi = (int)(uv.y * 32);
	}
	else
	{
		xi = (int)(uv.x * 32);
		yi = (int)(uv.y * 64) % 32;
	}
	tx = random[xi];
	ty = random[yi];
	if (horizontal)
	{
		if ((uv.y < 0.1 || uv.y > 0.9) && tx < 32)
			return transparent;
	}
	else
	{
		if ((uv.x < 0.1 || uv.x > 0.9) && ty < 32)
			return transparent;
	}

	t = tx + ty;
	if (t > 480)
		col.r -= 0.2f;
	else if (t > 448)
		col.g -= 0.2f;
	else if (t > 416)
		col.b -= 0.2f;
	else if (t > 384)
		col.g += 0.2f;
	else if (t > 352)
		col.b += 0.2f;

	return col;
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