#ifndef __FBM_HLSL_
#define __FBM_HLSL_

namespace Hsys
{
 #include "Noise.hlsl"
	//���β����˶� (FBM)
    //tex:
    //$$
    //  O_{t}���˶�(octaves)��ϸ�ڲ�� (��������)
    //  L_{a}: �������ԣ����θ��Ӷ� ���θ��Ӷ�Ϊ T_{c} = O_{t} \time L_{a}
    //  g �����θ��Ӷ�ǿ��Ȩ�� g \in [0,1] ���齫ֵǯ���� 0 �� 1֮��
    //  A : ��ֵ
    //  f : Ƶ��
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
	
	//���� FBM ��Ч��
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