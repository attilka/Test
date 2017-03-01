struct VS_IN
{
	float4 pos : POSITION;
	float3 normal: NORMAL;
	float2 texcoord : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float3 normal: NORMAL;
	float2 texcoord: TEXCOORD;
	float3 worldPos : POSITION;
};

cbuffer CBufferPerFrame : register (b0)
{
	matrix viewProj;
	float3 eyePosition;
};

cbuffer CBufferPerObject : register (b1)
{
	matrix world;
	matrix worldIT;
};


PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(mul(input.pos , world), viewProj);
	output.normal = mul(float4(input.normal.x, input.normal.y, input.normal.z, 1.0f), worldIT).xyz;
	output.texcoord = input.texcoord;
	output.worldPos = mul(input.pos, world).xyz;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 materialAmbient = float4(0.5f, 0.5f, 0.5f, 1.0f);
	float4 materialDiffuse = float4(0.7, 0.7, 0.7, 1.0f);
	float4 materialSpecular = float4(1.0f, 1.0f, 1.0f, 1);

	float3 lightDir = normalize(float3(0, 3.0, -5.0f));

	float4 lightAmbient = float4(0.3f, 0.3f, 0.3f, 1);
	float4 lightDiffuse = float4(0.7f, 0.7f, 0.7f, 1);
	float4 lightSpecular = float4(1.0f, 1.0f, 1.0f, 1);

	float siPower = 100;

	float4 ambient = materialAmbient * lightAmbient;

	float3 normal = normalize(input.normal);
	float3 light = - lightDir;
	float di = clamp(dot(normal, light), 0.0f, 1.0f);

	float4 diffuse = mul(materialDiffuse * lightDiffuse, di);

	float4 specular = float4(0, 0, 0, 1);

	if (di > 0) {
		float3 reflection = reflect(lightDir, normal);
		float3 c = normalize(eyePosition - input.worldPos);
		float si = pow(clamp(dot(reflection, c), 0.0f, 1.0f), siPower);
		specular = mul(lightSpecular * materialSpecular,si);
	}

	return /* input.col */ (ambient + diffuse + specular);
}