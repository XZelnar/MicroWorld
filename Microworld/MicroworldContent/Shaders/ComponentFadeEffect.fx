// texture we are rendering
sampler2D tex : register(S0);
float Opacity;
bool Drawtex;
float2 halfpixel;

// pixel shader function
float4 main(float2 uv : TEXCOORD, float4 color : COLOR0) : COLOR
{
	float4 b = tex2D(tex, uv);
	float4 a;
	if (b.a > 0.2)
	{
		if (Drawtex)
		{
			a = b + float4(1,1,1,1) * Opacity;
		}
		else
		{
			a = float4(1,1,1,1) * Opacity;
		}
		a *= color;
	}
	else
	{
		return float4(0,0,0,0);
	}
	return a;
}

//================================================VERTEX
float4x4 MatrixTransform;

void SpriteVertexShader(inout float4 color    : COLOR0,
                        inout float2 texCoord : TEXCOORD0,
                        inout float4 position : SV_Position)
{
    position = mul(position, MatrixTransform);
	//color=color*Opacity;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
        VertexShader = compile vs_3_0 SpriteVertexShader();
    }
}