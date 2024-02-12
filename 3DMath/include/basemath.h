#pragma once
#ifndef _HSYS_BASEMATH_H_
#define _HSYS_BASEMATH_H_

namespace Hsys
{
    
constexpr unsigned char M2 = 2;
constexpr unsigned char M3 = 3;
constexpr unsigned char M4 = 4;

template <typename T = float>
constexpr T I_2x2[M2][M2] = {{1,0},
                            {0,1}};
template <typename T = float>
constexpr T I_3x3[M3][M3] = {{1,0,0},
                            {0,1,0},
                            {0,0,1}};
template <typename T = float>
constexpr T I_4x4[M4][M4] = {{1,0,0,0},
                            {0,1,0,0},
                            {0,0,1,0},
                            {0,0,0,1}};

} // namespace Hsys
#endif //_HSYS_BASEMATH_H_
