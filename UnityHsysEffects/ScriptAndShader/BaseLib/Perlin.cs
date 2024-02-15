﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsys
{
    namespace MathAndAlgorithm_Hsys
    {
        //如果你想学习 里边的内容 将他导入到你的工程中 在 VS2015以上版本 下载插件 ==> VsTeXCommentsExtension
        //以显示在内部的数学注释

        //如果是 其他集成工具 没有关系，将注释导入 支持 Markdowm公式 的文件中

        public class Perlin
        {
            // 学习观看 ==> https://www.cnblogs.com/leoin2012/p/7218033.html
            // https://mrl.cs.nyu.edu/~perlin/paper445.pdf


            public int repeat;

            public Perlin(int repeat = -1)
            {
                this.repeat = repeat;
            }

            //提供幅频参数去调噪声特征
            //tex:
            //$$
            //  o_t = 0     \\
            //  o_f = 1     \\
            //  o_a = 1     \\
            //  o_{max} = 1, o_{max} \in [0,1]    \\
            //  O_{p} \Longleftarrow \{x,y,z,o,p\};\\
            //  \{ \\
            //  o_t = o_t + p(xo_f,yo_f,zo_f)o_{a}\\
            //  o_{(n)max} = o_{(n-1)max} + p_{(n-1)}o_a\\
            //  o_{(n)f} = 2o_(n)f\\
            //  \}
            //$$
            public double OctavePerlin(double x, double y, double z, int octaves, double persistence)
            {
                double total = 0;
                double frequency = 1;
                double amplitude = 1;
                double maxValue = 0;            // Used for normalizing result to 0.0 - 1.0
                for (int i = 0; i < octaves; i++)
                {
                    total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;

                    maxValue += amplitude;

                    amplitude *= persistence;
                    frequency *= 2;
                }

                return total / maxValue;
            }

            //准备随机的数据集，不重复的输入 0-255 间的任意数
            //tex:
            //$$
            // P_{r} = \{\Psi\}
            //$$
            private static readonly int[] permutation = { 151,160,137,91,90,15,					// Hash lookup table as defined by Ken Perlin.  This is a randomly
		        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,	
		        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
            };

            private static readonly int[] p;                                                    // Doubled permutation to avoid overflow

            //重复填充一次上述数组
            //tex:
            //$$
            //  P_{\tau} = \{ P_r, P_r \}
            //$$
            static Perlin()
            {
                p = new int[512];
                for (int x = 0; x < 512; x++)
                {
                    p[x] = permutation[x % 256];
                }
            }
            
            private double perlin(double x, double y, double z)
            {

                if (repeat > 0)
                {                                   // If we have any repeat on, change the coordinates to their "local" repetitions
                    x = x % repeat;
                    y = y % repeat;
                    z = z % repeat;
                }

                int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
                int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
                int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
                double xf = x - (int)x;                             // We also fade the location to smooth the result.
                double yf = y - (int)y; 

                double zf = z - (int)z;
                double u = Fade(xf);
                double v = Fade(yf);
                double w = Fade(zf);

                int aaa, aba, aab, abb, baa, bba, bab, bbb;
                aaa = p[p[p[xi] + yi] + zi];
                aba = p[p[p[xi] + inc(yi)] + zi];
                aab = p[p[p[xi] + yi] + inc(zi)];
                abb = p[p[p[xi] + inc(yi)] + inc(zi)];
                baa = p[p[p[inc(xi)] + yi] + zi];
                bba = p[p[p[inc(xi)] + inc(yi)] + zi];
                bab = p[p[p[inc(xi)] + yi] + inc(zi)];
                bbb = p[p[p[inc(xi)] + inc(yi)] + inc(zi)];

                double x1, x2, y1, y2;
                x1 = Lerp(Grad(aaa, xf, yf, zf),                // The Gradient function calculates the dot product between a pseudorandom
                            Grad(baa, xf - 1, yf, zf),              // Gradient vector and the vector from the input coordinate to the 8
                            u);                                     // surrounding points in its unit cube.
                x2 = Lerp(Grad(aba, xf, yf - 1, zf),                // This is all then lerped together as a sort of weighted average based on the faded (u,v,w)
                            Grad(bba, xf - 1, yf - 1, zf),              // values we made earlier.
                              u);
                y1 = Lerp(x1, x2, v);

                x1 = Lerp(Grad(aab, xf, yf, zf - 1),
                            Grad(bab, xf - 1, yf, zf - 1),
                            u);
                x2 = Lerp(Grad(abb, xf, yf - 1, zf - 1),
                              Grad(bbb, xf - 1, yf - 1, zf - 1),
                              u);
                y2 = Lerp(x1, x2, v);

                return (Lerp(y1, y2, w) + 1) / 2;                       // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
            }

            public int inc(int num)
            {
                ++num;
                if (repeat > 0) num %= repeat;

                return num;
            }

            //原算法中采用 位反转技巧来近似拟合梯度向量 以及 顶点位置
/*            public static double Grad(int hash, double x, double y, double z)
            {
                int h = hash & 15;                                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
                double u = h < 8 *//* 0b1000 *//* ? x : y;              // If the most significant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

                double v;                                           // In Ken Perlin's original implementation this was another conditional operator (?:).  I
                                                                    // expanded it for readability.

                if (h < 4 *//* 0b0100 *//*)                             // If the first and second significant bits are 0 set v = y
                    v = y;
                else if (h == 12 *//* 0b1100 *//* || h == 14 *//* 0b1110*//*)// If the first and second significant bits are 1 set v = x
                    v = x;
                else                                                // If the first and second significant bits are not equal (0/1, 1/0) set v = z
                    v = z;

                return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
            }*/

            public static double Grad(int hash, double x, double y, double z)
            {
                //从中挑选以下向量为梯度向量
                //(1,1,0),(-1,1,0),(1,-1,0),(-1,-1,0), (1,0,1),(-1,0,1),(1,0,-1),(-1,0,-1), (0,1,1),(0,-1,1),(0,1,-1),(0,-1,-1)
                //tex:
                //$$
                //  A_{\alpha} = {(1,1,0),(-1,1,0),(1,-1,0),(-1,-1,0), (1,0,1),(-1,0,1),(1,0,-1),(-1,0,-1), (0,1,1),(0,-1,1),(0,1,-1),(0,-1,-1)}\\
                //  grad(x^{\rightharpoonup}), x^{\rightharpoonup} \subset A_{\alpha}
                //$$
                switch (hash & 0x0F)
                {
                    case 0x00: return x + y;
                    case 0x01: return -x + y;
                    case 0x02: return x - y;
                    case 0x03: return -x - y;
                    case 0x04: return x + z;
                    case 0x05: return -x + z;
                    case 0x06: return x - z;
                    case 0x07: return -x - z;
                    case 0x08: return y + z;
                    case 0x09: return -y + z;
                    case 0x0A: return y - z;
                    case 0x0B: return -y - z;
                    case 0x0C: return y + x;
                    case 0x0D: return -y + z;
                    case 0x0E: return y - x;
                    case 0x0F: return -y - z;
                    default: return 0; // never happens
                }
                
            }

            //插值
            public static double Lerp(double a, double b, double x)
            {
                return a + x * (b - a);
            }

            //缓动
            //tex:
            //$$
            //  F = 6t^{5} - 15t^{4} + 10t^{3}
            //$$
            public static double Fade(double t)
            {
                // Fade function as defined by Ken Perlin.  This eases coordinate values
                // so that they will "ease" towards integral values.  This ends up smoothing
                // the final output.
                return t * t * t * (t * (t * 6 - 15) + 10);         
            }
        }

        public class KenPerlin
        {
            //学习: https://redirect.cs.umbc.edu/~olano/s2002c36/ch02.pdf
            int i, j, k;
            int[] A = { 0, 0, 0 };
            double u, v, w;
            double noise(double x, double y, double z)
            {
                double s = (x + y + z) / 3;
                i = (int)Math.Floor(x+s);
                j = (int)Math.Floor(y+s);
                k = (int)Math.Floor(z+s);

                s = (i + j + k) / 6;
                u = (x - i + s);
                v = (y - j + s);
                w = (z - k + s);

                A[0] = A[1] = A[2] = 0;
                int hi, lo;

                //下式等同于
                //int hi = u >= w ? u >= v ? 0 : 1 : v >= w ? 1 : 2;
                //int lo = u< w ? u< v ? 0 : 1 : v< w ? 1 : 2;
                if (u >= w)
                {
                    if (u >= v)
                    {
                        hi = 0;
                    }
                    else { hi = 1; }

                    if(v < w)
                    {
                        lo = 1;
                    }else { lo = 0; }
                }
                else
                {
                    if (v >= w)
                    {
                        hi = 1;
                    }
                    else { hi = 0; }

                    if(u < v) 
                    { 
                        lo = 0; 
                    }
                    else { lo = 1; }
                }


                return K(hi) + K(3 - hi - lo) + K(lo) + K(0);
            }

            private double K(int a)
            {
                double s = (A[0] + A[1] + A[2]) / 6;
                double x = u - A[0] + s, y = v - A[1] + s, z = w - A[2] + s;
                double t = 0.6 - x*x - y*y - z*z;

                int h = shuffle(i + A[0], j + A[1], k + A[2]);
                A[a] += 1;
                if(t < 0) { return 0; }

                int b5 = h >> 5 & 1;
                int b4 = h >> 4 & 1;
                int b3 = h >> 3 & 1;
                int b2 = h >> 2 & 1;
                int b = h & 3;

                //double p = b==1?x:b==2?y:z, q = b==1?y:b==2?z:x, r = b==1?z:b==2?x:y;
                double p,q,r;
                if(b==1)
                {
                    p = x;
                    q = y;
                    r = z;
                }else
                {
                    if (b == 2)
                    {
                        p = y;
                        q = z;
                        r = x;
                    }else
                    {
                        p = z; q = x; r = y;
                    }
                }

                p = (b5 == b3 ? -p : p);
                q = (b5 == b4 ? -q : q); 
                r = (b5 != (b4 ^ b3) ? -r : r);

                t *= t;

                return 8 * t * t * (p + (b == 0 ? q + r : b2 == 0 ? q : r));
            }

            private int shuffle(int i, int j, int k)
            {
                int result = 0;
                result += b(i, j, k, 0);
                result += b(j, k, i, 1);
                result += b(k, i, j, 2);
                result += b(i, j, k, 3);
                result += b(j, k, i, 4);
                result += b(k, i, j, 5);
                result += b(i, j, k, 6);
                result += b(j, k, i, 7);
                return result;
            }

            private int b(int i, int j, int k, int B)
            {
                return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
            }
            private int b(int N, int B) 
            { 
                return N >> B & 1; 
            }

            private readonly int[] T = { 0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a };

        }


        namespace Private_MathConst
        {
            public class MathConst
            {
                public const int B = 0x0100;
                public const int BM = B - 0x01;

                public const int N = 0x1000;
                public const int NM = 0x1000 - 0x01;
                public const int NP = 12;

                //public static float p[B + B + 2]

                public static double s_curve(double x)
                {
                    return ((3.0 - 2.0 * x) * x * x);
                }

                public static double lerp(double x, double a,  double b)
                {
                    return (a + x * (b - a));
                }

                //TODO:打个标记
                public static double setup(double t)
                {
                    //sdouble t = Vector2[]
                    return 0;
                }


            }
        }
    }


}
