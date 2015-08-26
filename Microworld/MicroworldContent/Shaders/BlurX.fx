// texture we are rendering
sampler2D tex : register(S0);

float PixelSize;
int Range;

// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c = 0; // will get max of each value for c
    float alpha = 0; // alpha will be average

    float2 myuv = uv;
	int r = Range;
	if (r < 1) r = 1;
	if (r > 5) r = 5;
	float4 sample;
    for(int i = -r; i <= r; i++)
    {
        myuv.x = uv.x + (i * PixelSize * 3.5);
		if (myuv.x >= 1 || myuv.x < 0) continue;
        sample = tex2D(tex, myuv);
        c = max(c, sample);
        alpha += sample.a;
    }

    c.a = saturate(pow(abs(alpha / 6), 0.4));
    return(c);
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