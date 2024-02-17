#ifndef __GRAYSCALE_HLSL_
#define __GRAYSCALE_HLSL_

//注: HLSL文件格式 VsTeXCommentsExtension 插件失效可以将 tex: 下的公式粘贴到支持 Markdown公式块 的软件中浏览
//#include "Constant.hlsl"
namespace Hsys
{
// Adapted from the author: Inigo Quiles
    //来自作者 Inigo Quiles 的实现函数
    float ExpStep(float x, float n)
    {
        return exp(-5.0 * pow(x, n));
    }

    float Impulse(float s, float k, float x)
    {
        float h = k * x;
        return h * exp(s - h);
    }

    float CubicPulse(float c, float w, float x)
    {
        x = abs(x - c) / -w;
        return 1.0 - x * x * (3.0 - 2.0 * x);
    }

    float Parabola(float x, float k)
    {
        return pow(4.0 * x * (1.0 - x), k);
    }

    float Pcurve(float x, float a, float b)
    {
        return pow(a + b, a + b) / (pow(a, a) * pow(b, b)) * pow(x, a) * pow(1.0 - x, b);
    }
    
    
    //缓动函数
    float EaseInElastic(float x, float step, float strength)
    {
        return -pow(2., strength * 10. * x - 5.) * sin((x * step * 5. - 10.75) * 2.52291579641);
    }
    
    //移动圆环 UI //不太好用
    float MovingRing(float2 d, float midR, float thickR, float time)
    {
        float r = length(d);
        float theta = -atan(d);

        return frac(.5 * (1. + theta / 3.14) - time) * smoothstep(2., 0., abs(r - midR) - thickR);
    }
};

#endif