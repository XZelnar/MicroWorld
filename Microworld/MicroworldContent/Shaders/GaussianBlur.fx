// texture we are rendering
sampler2D tex : register(S0);

float2 pixelSize;
int range;
float kernel[11];//MaxRange == 5

// pixel shader function
float4 horizontalPass(float2 uv : TEXCOORD) : COLOR
{
    float4 result = float4(0,0,0,0);
	//horizontal pass

	int r = range;
	if (r < 1)
		r = 1;
	if (r > 5)
		r = 5;
	int i = -r;
	int end = r;
	if (uv.x + i * pixelSize.x < 0)//goes left of screen. cut
		i = (int)(uv.x / pixelSize.x);
	if (uv.x + end * pixelSize.x > 1)//goes right of screen. cut
		end = (int)((1 - uv.x) / pixelSize.x);
	for (; i < end; i++)
	{
		result += tex2Dlod(tex, float4(uv.x + i * pixelSize.x, uv.y, 0, 0)) * kernel[i + r];
	}

    return result;
}

float4 verticalPass(float2 uv : TEXCOORD, float4 color : COLOR0) : COLOR
{
    float4 result = float4(0,0,0,0);
	//horizontal pass

	int r = range;
	if (r < 1)
		r = 1;
	if (r > 5)
		r = 5;
	int i = -r;
	int t = 0;//TODO rm
	for (; i < r; i++)
	{
		result += tex2Dlod(tex, float4(uv.x, uv.y + i * pixelSize.y, 0, 0)) * kernel[i + r];
	}

    return result * color;
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
        PixelShader = compile ps_3_0 horizontalPass();
        VertexShader = compile vs_3_0 SpriteVertexShader();
    }
    pass Pass2
    {
        PixelShader = compile ps_3_0 verticalPass();
        VertexShader = compile vs_3_0 SpriteVertexShader();
    }
}