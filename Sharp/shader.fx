struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float3 normal: NORMAL;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float3 normal: NORMAL;
	float worldPos : TEXCOORD0;
};

cbuffer CBuffer : register (b0)
{
	matrix world;
	matrix viewProj;
	matrix worldIT;
};



PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(mul(input.pos , world), viewProj);
	output.normal = mul(float4(input.normal.x, input.normal.y, input.normal.z, 1.0f), worldIT).xyz;
	output.col = input.col;
	//output.worldPos = mul(input.pos, world);

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 materialAmbient = float4(0.5f, 0.5f, 0.5f, 1.0f);
	float4 materialDiffuse = float4(0.7, 0.7, 0.7, 1.0f);

	float3 lightDir = normalize(float3(0, -3.0, 5.0f));

	float4 lightAmbient = float4(0.3f, 0.3f, 0.3f, 1);
	float4 lightDiffuse = float4(0.7f, 0.7f, 0.7f, 1);

	float4 ambient = materialAmbient * lightAmbient;

	float3 n = normalize(input.normal);
	float3 l = - lightDir;
	float4 di = clamp(dot(n, l), 0.0f, 1.0f);

	float4 diffuse = di * materialDiffuse * lightDiffuse;

	return input.col * (ambient +diffuse);
	return input.col;
}