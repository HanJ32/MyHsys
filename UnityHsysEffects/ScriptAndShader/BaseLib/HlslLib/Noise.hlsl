#ifndef __NOISE_HLSL_
#define __NOISE_HLSL_

//来源:https://edu.uwa4d.com/lesson-detail/499/2434/0?isPreview=0

//晶格顶点随机值
float RandomLatticevertices(in float2 st)
{
    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
}

//Perline noise
float PerlineNoise(in float2 st)
{
    float2 i = floor(st);
    float2 f = frac(st);
    
    float a = RandomLatticevertices(i);
    float b = RandomLatticevertices(i + float2(1.0, 0.0));
    float c = RandomLatticevertices(i + float2(0.0, 1.0));
    float d = RandomLatticevertices(i + float2(1.0, 1.0));

    //tex: 
    //$$
    //  3f^{2} - 2f^{3}
    //$$
    float2 u = 3.0 * f * f - 2.0 * f * f * f;
    float interpolation = lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
    return interpolation;
}

//晶格顶点随机向量
float2 RandomLatticevertices_Vec2D(float2 st)
{
    st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
    return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
}

float PerlineNoise_InigoQuilez(in float2 st)
{
    float2 i = floor(st);
    float2 f = frac(st);
    
    float2 u = 3.0 * f * f - 2.0 * f * f * f;
    
    float result =
    {
        lerp(lerp(dot(RandomLatticevertices_Vec2D(i + float2(0.0, 0.0)), f - float2(0.0, 0.0)),
                dot(RandomLatticevertices_Vec2D(i + float2(1.0, 0.0)), f - float2(1.0, 0.0)), u.x),
            lerp(dot(RandomLatticevertices_Vec2D(i + float2(0.0, 1.0)), f - float2(0.0, 1.0)),
                dot(RandomLatticevertices_Vec2D(i + float2(1.0, 1.0)), f - float2(1.0, 1.0)), u.x), u.y)
    };
    return result;
}
//HLSL 内部本身提供 noise API 与上述的 RandomLatticevertices_Vec2D
float Turbulence_noise(float2 p)
{
    float f = 0.0;
    p = p * 7.0;
    
    f += 1.0000 * abs(noise(p));
    p = 2.0 * p;
    f += 0.5000 * abs(noise(p));
    p = 2.0 * p;
    f += 0.2500 * abs(noise(p));
    p = 2.0 * p;
    f += 0.1250 * abs(noise(p));
    p = 2.0 * p;
    f += 0.0625 * abs(noise(p));
    p = 2.0 * p;
    
    f = sin(f + p.x / 32.0);
    return f;
}
//====================================
//Stefan
float3 mod289(float3 x)
{
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}
float2 mod289(float2 x)
{
    return x - floor(x * (1.0 / 289.0)) * 289.0;
}
float3 permute(float3 x)
{
    return mod289(((x * 34.0) + 1.0) * x);
}

float StefanNoise(float2 v)
{
    //tex: 
    //$$
    // \frac{3 - \sqrt{3}}{6}   \\
    // 0.5(\sqrt{3} - 1)        \\
    // 2C_{x} - 1                \\
    // frac{1}{41}
    //$$
    const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);

    float2 i = floor(v + dot(v, C.yy));
    float2 x0 = v - i + dot(i, C.xx);
    
    float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
    
    float4 x12 = x0.xyxy + C.xxzz;
    x12.xy -= i1;
    
    i = mod289(i);
    float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
    
    float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
    m = m * m;
    m = m * m;
    float3 x = 2.0 * frac(p * C.www) - 1.0;
    float3 h = abs(x) - 0.5;
    float3 ox = floor(x + 0.5);
    float3 a0 = x - ox;
    
    m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
}
//===========================

//学习 : https://thebookofshaders.com/12/
float3 hash(float3 x)
{
    x = float3(dot(x, float3(127.1, 311.7, 74.7)),
			  dot(x, float3(269.5, 183.3, 246.1)),
			  dot(x, float3(113.5, 271.9, 124.6)));

    return frac(sin(x) * 43758.5453123);
}
//TODO:hlsl;
float3 voronoi(in float3 x)
{
    float3 p = floor(x);
    float3 f = fract(x);

    float id = 0.0;
    float2 res = float2(100.0);
    for (int k = -1; k <= 1; k++)
        for (int j = -1; j <= 1; j++)
            for (int i = -1; i <= 1; i++)
            {
                float3 b = float3(float(i), float(j), float(k));
                float3 r = float3(b) - f + hash(p + b);
                float d = dot(r, r);

                if (d < res.x)
                {
                    id = dot(p + b, float3(1.0, 57.0, 113.0));
                    res = float2(d, res.x);
                }
                else if (d < res.y)
                {
                    res.y = d;
                }
            }

    return float3(sqrt(res), abs(id));
}
#endif