sampler2D tex : register(S0);
float sin_time_0_X: register(c6);
float force : register(c1);

float4 main(float2 pos: TEXCOORD0, float2 img: TEXCOORD1) : COLOR
{
   float dst = sin((pos.y + sin_time_0_X)*64)/16;
   dst *= force;

   img.x += dst;
   if (img.x < 0) img.x += 1;
   if (img.x > 1) img.x -= 1;
   return tex2D(tex, img);
}

//================================================VERTEX
float4x4 MatrixTransform;
struct VS_OUTPUT {
   float4 Pos: POSITION;
   float2 pos: TEXCOORD0;
   float2 img: TEXCOORD1;
};
VS_OUTPUT SpriteVertexShader(float4 Pos : SV_Position)
{
	VS_OUTPUT Out;

	Pos = mul(Pos, MatrixTransform);
	Pos.xy = sign(Pos.xy);

	Out.Pos = float4(Pos.xy, 0, 1);
	Out.pos = Pos.xy;
	Out.img.x = 0.5 * (1 + Pos.x);
	Out.img.y = 0.5 * (1 - Pos.y);

	return Out;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 main();
        VertexShader = compile vs_3_0 SpriteVertexShader();
    }
}