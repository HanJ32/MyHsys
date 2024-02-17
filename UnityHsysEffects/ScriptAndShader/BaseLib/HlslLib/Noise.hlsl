#ifndef __NOISE_HLSL_
#define __NOISE_HLSL_
//#include "Constant.hlsl"


//测试完毕 除了Voronoi函数 无法正确显示效果

//注: HLSL文件格式 VsTeXCommentsExtension 插件失效可以将 tex: 下的公式粘贴到支持 Markdown公式块 的软件中浏览

//encoding : UTF-8
namespace Hsys
{
#ifndef USE_HALF_DEAL
//在导入该文件时，可以适用定义 USE_NOISE_CONSTVAR 来开启定义的经验值或近似值 以提升性能
//但宏定义开启要用额外空间进行展开所以是空间换时间，但 USE_NOISE_CONSTVAR 宏都是值定义 并不会占用内存开销
#ifdef USE_NOISE_CONSTVAR
#define K 0.142857142857 // 1/7
#define Ko 0.428571428571 // 1/2-K/2
#define K2 0.020408163265306 // 1/(7*7)
#define Kz 0.166666666667 // 1/6
#define Kzo 0.416666666667 // 1/2-1/6*2
#define jitter 0.8 // smaller jitter gives less errors in F2
#endif //#ifdef USE_NOISE_CONSTVAR


//学习:https://edu.uwa4d.com/lesson-detail/499/2434/0?isPreview=0

//晶格顶点随机
    float RandomLatticevertices(in float2 st)
    {
        return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
    }

//Perline noise
    float PerlineNoise(float2 st, float size, float range)
    {
        st *= size;
        float2 i = floor(st);
        float2 f = frac(st);
    
        //i *= seed;
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
        return interpolation*range;
    }

//晶格顶点随机向量
    float2 RandomLatticevertices_Vec2D(float2 st)
    {
        st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
        return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
    }

    float PerlineNoise_InigoQuilez(float2 st, float size, float range)
    {
        st *= size;
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
        return result * range;
    }
//HLSL �ڲ������ṩ noise API �������� RandomLatticevertices_Vec2D
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
    float4 permute(float4 x)
    {
        return fmod((34.0 * x + 1.0) * x, 289.0);
    }
    

    //Unity shader \\换行有空格警告 注意 公式显示问题将 \\\_ 后的 \_ 删掉
    //tex: 
    //$$
    // \frac{3 - \sqrt{3}}{6}\\\_
    // 0.5(\sqrt{3} - 1)\\\_
    // 2C_{x} - 1\\\_
    // frac{1}{41}
    //$$
    //
    
    float StefanNoise(float2 v, float size, float range)
    {
        const float4 cxdx = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
        v = v * size;
        float2 i = floor(v + dot(v, cxdx.yy));
        float2 x0 = v - i + dot(i, cxdx.xx);
    
        float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
    
        float4 x12 = x0.xyxy + cxdx.xxzz;
        x12.xy -= i1;
    
        i = mod289(i);
        float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
    
        float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
        m = m * m;
        m = m * m;
        float3 x = 2.0 * frac(p * cxdx.www) - 1.0;
        float3 h = abs(x) - 0.5;
        float3 ox = floor(x + 0.5);
        float3 a0 = x - ox;
    
        m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
        float3 g;
        g.x = a0.x * x0.x + h.x * x0.y;
        g.yz = a0.yz * x12.xz + h.yz * x12.yw;
        return 130.0 * dot(m * range, g * range);
    }
//===========================

//学习 : https://thebookofshaders.com/12/
    float3 hash(float2 x)
    {
        x = float3(dot(x, float2(127.1, 311.7)),
			  dot(x, float2(269.5, 183.3)),
			  dot(x, float2(113.5, 271.9)));

        return float3(frac(sin(x) * 43758.5453123), 1.0);
    }
//TODO:hlsl;暂时不能用
    float Voronoi(float2 uv, float u, float v)
    {
    
        float k = 1.0 + 63.0 * pow(1.0 - v, 6.0);

        float2 p = floor(uv);
        float2 f = frac(uv);

        float2 res = float2(10.0,10.0);
        for (int j = -1; j <= 1; j++)
            for (int i = -1; i <= 1; i++)
            {
                float2 g = float2(i, j);
                float3 o = hash(p + g) * float3(u, u, 1.0);
                float2 d = g - f + o.xy;
                float w = pow(1.0 - smoothstep(0.0, 1.414, length(d)), k);
                res += float2(o.z * w, w);
            }

        return res.x / res.y;
    }
    
    float Voronoi_q(float2 uv, float size, float v)
    {
        //uv *= floor(uv);
        uv *= size;
        float2 p = floor(uv);
        float2 f = frac(uv);
        //v *= 1 / 5;
        v = v - 3;
        float k = 1.0 + 63.0 * pow(1.0 - v, 4.0);

        float va = 0.0;
        float wt = 0.0;
        for (int j = -2; j <= 2; j++)
        {
            for (int i = -2; i <= 2; i++)
            {
                float2 g = float2(float(i), float(j));
                float3 o = hash(p + g) * float3(1.0, 1.0, 1.0);
                float2 r = g - f + o.xy;
                float d = dot(r, r);
                float ww = pow(1.0 - smoothstep(0.0, 1.414, sqrt(d)), k);
                va += o.z * ww;
                wt += ww;
            }
        }

        return va / wt;
    }

    //基于 Voronoi 噪声生成
    float2 Cellular2x2x2(float3 P)
    {
        
#ifndef USE_NOISE_CONSTVAR //是否开启宏 USE_NOISE_CONSTVAR
        float K = 1 / 7; // 1/7
        float Ko = 1 / 2 - K / 2; // 1/2-K/2

        float K2 = 1 / (7 * 7); // 1/(7*7)
        float Kz = 1 / 6; // 1/6
        float Kzo = 1 / 2 - 1 / 6 * 2; // 1/2-1/6*2
        float3 jitter = 0.8;
#endif //USE_NOISE_CONSTVAR
    
        float3 Pi = fmod(floor(P), 289.0);
        float3 Pf = frac(P);
        float3 Pfx = Pf.x + float4(0.0, -1.0, 0.0, -1.0);
        float3 Pfy = Pf.y + float4(0.0, 0.0, -1.0, -1.0);
        float3 p = permute(Pi.x + float3(0.0, 1.0, 0.0));
        p = permute(p + Pi.y + float3(0.0, 0.0, 1.0));
        float3 p1 = permute(p + Pi.z); // z+0
        float3 p2 = permute(p + Pi.z + float3(1.0,1.0,1.0)); // z+1
        float3 ox1 = frac(p1 * K) - Ko;
        float3 oy1 = fmod(floor(p1 * K), 7.0) * K - Ko;
        float3 oz1 = floor(p1 * K2) * Kz - Kzo; // p1 < 289 guaranteed
        float3 ox2 = frac(p2 * K) - Ko;
        float3 oy2 = fmod(floor(p2 * K), 7.0) * K - Ko;
        float3 oz2 = floor(p2 * K2) * Kz - Kzo;
        float4 dx1 = float4(Pfx + jitter * ox1, 1.0);
        float4 dy1 = float4(Pfy + jitter * oy1,1.0);
        float4 dz1 = float4(Pf.z + jitter * oz1,1.0);
        float4 dx2 = float4(Pfx + jitter * ox2, 1.0);
        float4 dy2 = float4(Pfy + jitter * oy2, 1.0);
        float4 dz2 = float4(Pf.z - 1.0 + jitter * oz2,1.0);
        float4 d1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1; // z+0
        float4 d2 = dx2 * dx2 + dy2 * dy2 + dz2 * dz2; // z+1
        
	// Do it right and sort out both F1 and F2
        float4 d = min(d1, d2); // F1 is now in d
        d2 = max(d1, d2); // Make sure we keep all candidates for F2
        d.xy = (d.x < d.y) ? d.xy : d.yx; // Swap smallest to d.x
        d.xz = (d.x < d.z) ? d.xz : d.zx;
        d.xw = (d.x < d.w) ? d.xw : d.wx; // F1 is now in d.x
        d.yzw = min(d.yzw, d2.yzw); // F2 now not in d2.yzw
        d.y = min(d.y, d.z); // nor in d.z
        d.y = min(d.y, d.w); // nor in d.w
        d.y = min(d.y, d2.x); // F2 is now in d.y
        return sqrt(d.xy); // F1 and F2
    }
    
    float2 Cellular2x2(float2 uv, float strenge,float size)
    {
#ifndef USE_NOISE_CONSTVAR //是否开启宏 USE_NOISE_CONSTVAR
        float K = 1 / 7; // 1/7
        float K2 = 1 / (7 * 7); // 1/(7*7)
        float jitter = 0.8;
#endif //USE_NOISE_CONSTVAR
        
        uv *= size;
        float2 Pi = fmod(floor(uv), 289.0);
        float2 Pf = frac(uv);
        
        float4 Pfx = Pf.x + float4(-0.5, -1.5, -0.5, -1.5);
        float4 Pfy = Pf.y + float4(-0.5, -0.5, -1.5, -1.5);
        float4 p = permute(Pi.x + float4(0.0, 1.0, 0.0, 1.0));
        p = permute(p + Pi.y + float4(0.0, 0.0, 1.0, 1.0));
        float4 ox = fmod(p, 7.0) * K + K2;
        float4 oy = fmod(floor(p * K), 7.0) * K + K2;
        float4 dx = Pfx + jitter * ox;
        float4 dy = Pfy + jitter * oy;
        float4 d = dx * dx + dy * dy; // d11, d12, d21 and d22, squared
	// Do it right and find both F1 and F2
        d.xy = (d.x < d.y) ? d.xy : d.yx; // Swap if smaller
        d.xz = (d.x < d.z) ? d.xz : d.zx;
        d.xw = (d.x < d.w) ? d.xw : d.wx;
        d.y = min(d.y, d.z);
        d.y = min(d.y, d.w);
        return sqrt(d.xy) * strenge;
    }
    
    //基于 Cellular 的实现效果
    //unity shaderlab 使用示例
    //
    // fixed4 frag(appdate i) : SV_Target
    //{
    //   fixed n = Hsys::Cellular_Effect_Two(i.uv,_Float,_Float2,_Float3,_Float4);
    //   fixed4 col = float4(n, n, n, 1.0);
    //   return col;
    //}
    
    float Cellular_Effect_One(float2 uv, float strenge, float size, float updown, float cover)
    {
        float y = Cellular2x2(uv, strenge, size);
        float n = 1.0 - step(abs(sin(y)) - .1, updown + (uv.x - uv.y) * cover);
        return n;
    }
    
    //unity shaderlab 示例 形式同上 只不过多了个中线点参数
    float Cellular_Effect_Two(float2 uv, float strenge, float size, float updown, float cover, float2 center_point)
    {
        uv -= center_point;
        
        float y = Cellular2x2(uv, strenge, size);
        float n = step(abs(sin(dot(uv, uv) * 3.1415 * updown)), y * cover);
        return n;
    }
    //==================================================================
    //伪随机值 伪随机噪声
    float RandomNoise(float2 uv)
    {
        return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
    }
    float RandomNoise(float2 uv, float size)
    {
        uv *= size;
        float2 ipos = floor(uv); // get the integer coords
        return frac(sin(dot(ipos.xy, float2(12.9898, 78.233))) * 43758.5453123);
    }
    
    float2 RandomNoiseSeed(float2 uv, float seed = 1)
    {
        uv *= seed;
        uv = float2(dot(uv, float2(127.1, 311.7)),dot(uv, float2(269.5, 183.3)));
        return -1.0 + 2.0 * frac(sin(uv) * 43758.5453123);
    }
    
    float Noise(float2 uv)
    {
        float2 i = floor(uv);
        float2 f = frac(uv);

        float2 u = f * f * (3.0 - 2.0 * f);

        return lerp(lerp(dot(RandomNoiseSeed(i + float2(0.0, 0.0)), f - float2(0.0, 0.0)),
                        dot(RandomNoiseSeed(i + float2(1.0, 0.0)), f - float2(1.0, 0.0)), u.x),
                    lerp(dot(RandomNoiseSeed(i + float2(0.0, 1.0)), f - float2(0.0, 1.0)),
                        dot(RandomNoiseSeed(i + float2(1.0, 1.0)), f - float2(1.0, 1.0)), u.x), u.y);
    }
    
    //半精度好像没办法实现上述，精度缺失严重
#endif //ifndef USE_HALF_DEAL
#ifdef USE_HALF_DEAL
    float None(){return (0);}
    
#endif //ifdef USE_HALF_DEAL

}
#endif