#pragma once
#ifndef _Hsys_MATHUTIL_H_
#define _Hsys_MATHUTIL_H_
#include <math.h>

namespace Hsys
{
    


constexpr float kPi = 3.14159265f;
constexpr float k2Pi = kPi * 2.0f;
constexpr float kPiOver2 = kPi / 2.0f;
constexpr float klOverPi = 1.0f / kPi;
constexpr float klOver2Pi = 1.0f / k2Pi;

// Limit the Angle to the range -Pi to Pi by adding an appropriate multiple of 2Pi
extern float wrapPi(float theta);
// “Safe” inverse trigonometric funtion
extern float safeAcos(float x);

//Calculate the sin and cos of the Angle
constexpr auto sinCos(float &returnSin, float &returnCosm, float &theta) -> void
{
    returnSin = sin(theta);
    returnCosm = cos(theta);
}



} // namespace Hsys

#endif //_Hsys_MATHUTIL_H_
