// texture we are rendering
sampler2D tex : register(S0);

float power = 0;
float2 halfpixel;
float brightnessDistortion = 0;
float sinTime = 0;
float sinDistForce = 0;
float noiseForce = 0;
float inversionForce = 0;

// pixel shader function
float4 main(float2 uv : TEXCOORD) : COLOR
{
	//=======================================================sine dist
	float dist = sin((uv.y * 4 + sinTime) * 64) / 16;
	dist *= sinDistForce;
	uv.x += dist;

    float4 c = tex2D(tex, uv);

	//=======================================================stereo distortion
	float4 res = float4(
		tex2D(tex, uv - float2(power, 0)).r, 
		c.g, 
		tex2D(tex, uv + float2(power, 0)).b, 
		c.a);

	//=======================================================brightness distortion
	res.x += brightnessDistortion;
	res.y += brightnessDistortion;
	res.z += brightnessDistortion;

	//=======================================================noise
	//TODO
	
	//=======================================================inversion
	float4 t = res + ((float4(1, 1, 1, 1) - res) - res) * inversionForce;
	t.a = 1;
	res = t;
	
	//=======================================================uncolored tiles
	//TODO

	return res;
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