// texture we are rendering
sampler2D tex : register(S0);

float2 pixel;

// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c = tex2D(tex, uv);
	if (c.a == 0)
		return float4(0, 0, 0, 0);
	if (tex2D(tex, uv + float2(pixel.x, 0)).a == 0 ||
		tex2D(tex, uv - float2(pixel.x, 0)).a == 0 ||
		tex2D(tex, uv + float2(0, pixel.y)).a == 0 ||
		tex2D(tex, uv - float2(0, pixel.y)).a == 0)
		return float4(0.7f, 0.7f, 0.9f, 1);
	return c;
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