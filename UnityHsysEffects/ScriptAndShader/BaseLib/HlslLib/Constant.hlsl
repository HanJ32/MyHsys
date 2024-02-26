#ifndef __CONSTANT_HLSL_
#define __CONSTANT_HLSL_


namespace Hsys
{
//数学常量
#define PI                  3.14159265359   //圆周率
#define EULER_MASCHERONI    0.57721566490   //欧拉-马斯切罗尼常数
#define EXP                 2.71828182845   //欧拉数 (e)
#define GOLDEN_RATIO        1.61803398875   //黄金比例
#define SQRT_TWO            1.41421356237   //根号2  sqrt(2);
//引申数学常量
#define LOG2_E              0.43429448190   //Log_2(e)
#define PI_TWO              6.28318530717   //二倍圆周率
#define PI_FOUR             12.5663706143   //四倍圆周率
#define PI_INV              0.31830988618   //圆周率倒数
#define PI_HALF             1.57079632679   //半圆周率
#define PI_HALF_INV         0.63661977236   //半圆周率倒数
    //=========================== 基础数学运算 ==============================
    //初值导数 
    float dfdx(in float df, float x)
    {
        float dx = 0.00001;
        return (df * (x + dx) - df * (x - dx)) / (2*dx);
    }
    
    //自动微分
    //学习 : https://arxiv.org/pdf/1502.05767.pdf
    
    
    
    // 数值积分
    //梯度法积分
    //f.x 是上界 f.y 是下界
    float fTdx(float2 f, float n)
    {
        float h = (f.y - f.x) / n;
        float s = pow(f.x, 2.0) + pow(f.y, 2.0);
        
        for (int index = 1; index < n; index+=1)
        {
            s += 2 * pow(f.y + index * h,2.0);
        }
        return s * h / 2;
    }
    
    //取区间随机值 
    float Range(float value, float down, float up)
    {
        return frac(sin(value + up) * down);
    }
    
    float Random1Dto1D(float value, float dest, float source)
    {
        return Range(value, dest, source);
    }
    
    float Random2DTo1D(float2 value, float dest, float2 source)
    {
        return frac(sin(dot(sin(value), source)) * dest);
    }
    
    float2 Random2DTo2D(float2 value)
    {
        return float2(Random2DTo1D(value, 14375.5964, float2(15.637, 76.243)), Random2DTo1D(value, 14684.6034, float2(45.366, 23.168)));
    }
    
    float Random3DTo1D(float3 value, float dest, float3 source)
    {
        return frac(sin(dot(sin(value), source)) * dest);
    }
    
    float2 Random3DTo2D(float3 value)
    {
        return float2(Random3DTo1D(value, 14375.5964, float3(15.637, 76.243, 37.168)), Random3DTo1D(value, 14684.6034, float3(45.366, 23.168, 65.918)));
    }
    
    float3 Random3DTo3D(float3 value)
    {
        return float3(Random3DTo1D(value, 14375.5964, float3(15.637, 76.243, 37.168)), Random3DTo1D(value, 14684.6034, float3(45.366, 23.168, 65.918)), Random3DTo1D(value, 17635.1739, float3(62.654, 88.467, 25.111)));
    }
    
    //=======================================================================
    //一些转换函数
    namespace Private_Hsys
    {
        inline float4 EncodeFloatRGBA(float v)
        {
            float4 enc = float4(1.0, 255.0, 65025.0, 16581375.0) * v;
            enc = frac(enc);
            enc -= enc.yzww * float(1.0 / 255.0);
            return enc;
        }
        inline float4 DecodeFloatRGBA(float4 rgba)
        {
            return dot(rgba, float4(1.0, 1.0 / 255.0, 1.0 / 65025.0, 1.0 / 16581375.0));
        }
        inline float2 EncodeFloatRG(float v)
        {
            float2 enc = v * float2(1.0, 255.0);
            enc = frac(enc);
            enc.x -= enc.y * float(1.0 / 255.0);
            return enc;
        }
        inline float DecodeFloatRG(float2 enc)
        {
            return dot(enc, float2(1.0, 1.0 / 255.0));
        }
    
    //unity 上的 pack unpack
        float4 pack(float dp)
        {
            const float4 bitshift = float4(1.0, 256.0, 256.0 * 256.0, pow(256.0, 3.0));
            const float4 bitmask = float4(1.0 / 256.0, 1.0 / 256.0, 1.0 / 256.0, 0.0);

            float4 rgbadp = frac(dp * bitshift);
            rgbadp -= rgbadp.gbaa * bitmask;
            return rgbadp;
        }
        float unpack(float rgbadp)
        {
            const float4 bitshift = float4(1.0, 1.0 / 256.0, 1.0 / (256.0 * 256.0), 1.0 / pow(256.0, 3.0));
            float dp = dot(rgbadp, bitshift);
            if (abs(dp)<EPS)
            {
                dp = 1.0;
            }
            return dp;
        }

        //https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
        
        
        //
        void uniformDiskSamples(const in float2 randomseed)
        {
            //float rn = Random2DTo1D(randomseed);
            //float sx = Random1Dto1D(rn);
        }

//=================================================================
//X-PostProcessing        
//#define SAMPLE_TEXTURE2D(texturename, samplername, coord2) texturename.Sampler(samplername, coord2)
    }
//物理常量
    
};
#endif