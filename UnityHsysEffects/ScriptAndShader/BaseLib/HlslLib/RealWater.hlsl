#ifndef __REAL_WATER_HLSL_
#define __REAL_WATER_HLSL_

namespace Hsys
{

// 学习 : https://blog.csdn.net/poem_qianmo/article/details/103447558?ops_request_misc=%257B%2522request%255Fid%2522%253A%2522170821929016800188579564%2522%252C%2522scm%2522%253A%252220140713.130102334..%2522%257D&request_id=170821929016800188579564&biz_id=0&utm_medium=distribute.pc_search_result.none-task-blog-2~all~sobaiduend~default-1-103447558-null-null.142^v99^pc_search_result_base4&utm_term=%E5%88%86%E5%BD%A2%E5%B8%83%E6%9C%97%E8%BF%90%E5%8A%A8%E4%BB%A3%E7%A0%81&spm=1018.2226.3001.4187

//波动粒子（Wave Particle）

// 学习 : https://www.researchgate.net/publication/326729789_Water_surface_wavelets

    
    
//=========================================================================
//浅水方程
//学习 :https://cn.comsol.com/model/the-shallow-water-equations-202
  
    // 参数 : x 包含 求解值 和 初值, ak 包含 参数 a 、k1
    //float Shallow_Waterequation(float2 x, float2 ak)
    //{
    //    float Zf = ak.x * exp(-((x.x - x.y) * (x.x - x.y))) + ak.y * x; //海床轮廓
    //    float dZfdx = ddx(Zf);
    //    
    //
    //}
    namespace Private_Hsys
    {
        float Height_Pro(float x)
        {
            return 0.1 + 0.1 * exp(-64 * (x - 0.25) * (x - 0.25));

        }
    }
}
#endif