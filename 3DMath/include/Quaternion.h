#pragma once
#ifndef _HSYS_QUATERNION_H_
#define _HSYS_QUATERNION_H_

namespace Hsys
{
template <class T>
class Vector3;
template <class T>
class EulerAngles;

template <class T = float>
class Quaternion
{
public:
    using Type = T;
    Type _w,_x,_y,_z;
    Quaternion() = default;
    ~Quaternion() = default;

    auto identity() -> void;
    auto setToRotateAboutX(Type theta) noexcept -> void;
    auto setToRotateAboutY(Type theta) noexcept -> void;
    auto setToRotateAboutZ(Type theta) noexcept -> void;
    auto setToRotateAboutAxis(const Vector3<Type>& axis, Type theta) noexcept -> void;

    //The quaternion that performs the rotation of the object the inertia is constructed, 
    //and the azimuth parameters are given in the form of Euler angles.
    auto setToRotateObjectToInertial(const EulerAngles<Type>& orientation) -> void;
    auto setToRotateInertialToObject(const EulerAngles<Type>& orientation) -> void;

    //multiplication cross
    auto operator *(const Quaternion<Type>& a) const -> Quaternion<Type>;
    auto operator *=(const Quaternion<Type>& a) -> Quaternion<Type>&;
    
    auto normalize() noexcept -> void;

    auto getRotationAngle() const -> Type;
    auto getRotationAxis() const -> Vector3<Type>;
};

template <typename T>
extern const Quaternion<T> kQuaternionIdentity{};
} // namespace Hsys

#endif //_HSYS_QUATERNION_H_