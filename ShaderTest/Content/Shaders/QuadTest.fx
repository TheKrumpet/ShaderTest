struct VertexPositionTexture
{
    float4 Position : POSITION;
    float2 TexCoords : TEXCOORD0;
};

struct VertexPositionTextureOutput
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD0;
};

struct VertexPositionColor
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};

struct VertexPositionColorOutput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
};

VertexPositionTextureOutput VShaderGradient(VertexPositionTexture input)
{
    VertexPositionTextureOutput output;

    output.Position = input.Position;
    output.TexCoords = input.TexCoords;
    
    return output;
}

float4 PShaderGradient(VertexPositionTextureOutput input) : SV_Target0
{
    return float4(input.TexCoords.xy, 0.0f, 1.0f);
}

VertexPositionColorOutput VShaderVertexColor(VertexPositionColor input)
{
    VertexPositionColorOutput output;
    
    output.Position = input.Position;
    output.Color = input.Color;
    
    return output;
}

float4 PShaderVertexColor(VertexPositionColorOutput input) : SV_Target0
{
    return input.Color;
}

technique DrawGradient
{
    pass p0
    {
        VertexShader = compile vs_6_0 VShaderGradient();
        PixelShader = compile ps_6_0 PShaderGradient();
    }
}

technique DrawVertexColor
{
    pass p0
    {
        VertexShader = compile vs_6_0 VShaderVertexColor();
        PixelShader = compile ps_6_0 PShaderVertexColor();
    }
}