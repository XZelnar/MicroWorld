// texture we are rendering
sampler2D tex : register(S0);

float cr = 0.5882353f;
float cg = 0.6117647f;
float cb = 0.709803939f;

float dr = 0.8235294f;
float dg = 0.7764706f;
float db = 0.5803922f;

float state = 1;
float2 pixel1 = 0;
float2 pixel2 = 0;


// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
    uv += pixel1;
    float4 c = tex2D(tex, uv);
	float t;
	if (c.a == 0)
	{
		t = tex2D(tex, float2(uv.x+pixel1.x,uv.y)).a;
		if (t != 0)
				return float4(state,state,state,state * t);
		t = tex2D(tex, float2(uv.x-pixel1.x,uv.y)).a;
		if (t != 0)
				return float4(state,state,state,state * t);
		t = tex2D(tex, float2(uv.x,uv.y+pixel1.y)).a;
		if (t != 0)
				return float4(state,state,state,state * t);
		t = tex2D(tex, float2(uv.x,uv.y-pixel1.y)).a;
		if (t != 0)
				return float4(state,state,state,state * t);
		//if (
		//	tex2D(tex, float2(uv.x+pixel1.x,uv.y)).a != 0 ||
		//	tex2D(tex, float2(uv.x-pixel1.x,uv.y)).a != 0 ||
		//	tex2D(tex, float2(uv.x,uv.y+pixel1.y)).a != 0 ||
		//	tex2D(tex, float2(uv.x,uv.y-pixel1.y)).a != 0)
		//		return float4(state,state,state,state);
		
		return c;
	}

	c.r = c.r - (c.r - cr) * 2 * state;
	c.g = c.g - (c.g - cg) * 2 * state;
	c.b = c.b - (c.b - cb) * 2 * state;

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