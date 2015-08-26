// texture we are rendering
sampler2D tex : register(S0);
Texture2D dist;

float2 pixel;
float phase;
float pi;

SamplerState TextureSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

// pixel shader function
float4 main(float2 uv : TEXCOORD, float4 color : COLOR0) : COLOR
{
	float4 c = tex2D(tex, uv);
	//stroke
	if (c.a == 0)
	{
		if ((uv.x + pixel.x < 1 && tex2D(tex, uv + float2(pixel.x, 0)).a != 0) ||
		    (uv.y + pixel.y < 1 && tex2D(tex, uv + float2(0, pixel.y)).a != 0) ||
			(uv.x - pixel.x > 0 && tex2D(tex, uv - float2(pixel.x, 0)).a != 0) ||
		    (uv.y - pixel.y > 0 && tex2D(tex, uv - float2(0, pixel.y)).a != 0))
				return float4(1, 1, 1, 1);
	}
	//morph
	//float4 overlay = tex2D(dist, float2(sin(uv.x + phase), cos(uv.y + phase)));
	//float4 overlay = dist.Sample(TextureSampler, uv);
	//overlay.r = sin(overlay.r * pi + phase) * 0.25f + 0.5f;
	//overlay.g = overlay.r;
	//overlay.b = overlay.r;
	return c * color;
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