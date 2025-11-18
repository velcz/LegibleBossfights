sampler2D uImage0 : register(s0);
float2 uImageSize0;
float4 OutlineColor = float4(1,1,1,1);
float OutlineThickness = 1.0;
float4 NPCShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(uImage0, coords);
    float4 result = tex * sampleColor;
    if (tex.a == 0)
    {
        float pxX = OutlineThickness / uImageSize0.x;
        float pxY = OutlineThickness / uImageSize0.y;
        if (tex2D(uImage0, coords + float2(pxX,0)).a > 0 ||
            tex2D(uImage0, coords - float2(pxX,0)).a > 0 ||
            tex2D(uImage0, coords + float2(0,pxY)).a > 0 ||
            tex2D(uImage0, coords - float2(0,pxY)).a > 0) {
            result = OutlineColor;
        }
    }
    return result;
}
technique Effect
{
    pass effectPass
    {
        PixelShader = compile ps_2_0 NPCShader();
    }
}
