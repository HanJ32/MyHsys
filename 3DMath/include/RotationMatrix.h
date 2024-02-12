#pragma once
#ifndef _HSYS_ROTATTIONMATRIX_H_
#define _HSYS_ROTATTIONMATRIX_H_
#include <basemath.h>


namespace Hsys
{
template <typename T>
class Vector3;
template <typename T>
class EulerAngles;
template <typename T>
class Quaternion;

template <class T = float>
class RotationMatrix
{
public:
    using Type = T;

    // Type m11, m12, m13;
    // Type m21, m22, m23;
    // Type m31, m32, m33;
    Type _Ma3x3[M3][M3];

    auto identity() noexcept -> void;

    auto setup(const EulerAngles<Type>& orientation) noexcept -> void;

    auto fromObjectToInertialQuaternion(const Quaternion<Type>& q) throw() -> void;

    auto inertialToObject(const Vector3<Type>& v) const -> Vector3<Type>;
    auto objectToInertial(const Vector3<Type>& v) const -> Vector3<Type>;
};

} //Hsys namespace

#endif //_HSYS_ROTATTIONMATRIX_H_