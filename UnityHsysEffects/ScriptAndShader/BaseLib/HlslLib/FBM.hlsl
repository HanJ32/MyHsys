#ifndef __FBM_HLSL_
#define __FBM_HLSL_

namespace Hsys
{
 #include "Noise.hlsl"
	//分形布朗运动 (FBM)
    //tex:
    //$$
    //  O_{t}：八度(octaves)，细节层次 (迭代次数)
    //  L_{a}: 不均匀性，地形复杂度 地形复杂度为 T_{c} = O_{t} \time L_{a}
    //  g ：地形复杂度强度权重 g \in [0,1] 建议将值钳制在 0 到 1之间
    //  A : 幅值
    //  f : 频率
    // 
    //$$
    //V ==> (O_{t}, L_{t}, A, f)
    //Adapted From : https://iquilezles.org/articles/fbm/
	
	float FBM(in float4 V, in float H)
	{
		float G = exp2(-H);
		float f = V.w;
		float A = V.z;
        
		float t = 0.0;
		for (int i = 0; i < V.x; i += 1)
		{
			t += A * noise(f * V.y);
			f *= 2.0;
			A *= G;
		}
		return t;
	}
	
	//基于 FBM 的效果
    void FBM_Clouds()
    {
		
    }
	
    namespace Private_Hsys
    {
		void interpolate()
		for(
        int y = 0;y < 256; y+=step)
    }
    

}
#endif