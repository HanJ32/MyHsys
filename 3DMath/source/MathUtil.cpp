#include "MathUtil.h"
#include "Vector3.h"

//const Hsys::Vector3<float> Hsys:: kZeroVector{};

//The Angle is restricted to the range -Pi to Pi by adding an appropriate multiple of 2pi
auto Hsys::wrapPi(float theta) -> float
{
    theta += kPi;
    theta -= floor(theta * klOver2Pi) * k2Pi;
    theta -= kPi;
    return theta;
}

//Same as acos(x), but if x is out of range it returns the closest valid value, which is between 0 and pi
auto Hsys::safeAcos(float x) -> float
{
    //Cherk boundary
    if(x <= -1.0f)
    {
        return kPi;
    }
    if(x >= 1.0f)
    {
        return 0.0f;
    }
    return acos(x);
}