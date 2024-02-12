#include <assert.h>
#include <math.h>

#include "Quaternion.h"
#include "MathUtil.h"
#include "Vector3.h"
#include "EulerAngles.h"

namespace Hsys
{

template <typename T>
const Quaternion<T> kQuaternionIdentity = {1.0f,0.0f,0.0f,0.0f};

template <typename T>
auto Quaternion<T>::identity() -> void
{
    this->_w = (Type)1.0;
    this->_x = (Type)0.0;
    this->_y = (Type)0.0;
    this->_z = (Type)0.0;
}

template <typename T>
auto Quaternion<T>::setToRotateAboutX(Type theta) noexcept -> void
{
    Type theta2 = theta * (Type)0.5;
    this->_w = cos(theta2);
    this->_x = sin(theta2);
    this->_y = 0.0f;
    this->_z = 0.0f;
}

template <typename T>
auto Quaternion<T>::setToRotateAboutY(Type theta) noexcept -> void
{
    Type theta2 = theta * (Type)0.5;
    this->_w = cos(theta2);
    this->_x = 0.0f;
    this->_y = sin(theta2);
    this->_z = 0.0f;
}

template <typename T>
auto Quaternion<T>::setToRotateAboutZ(Type theta) noexcept -> void
{
    Type theta2 = theta * (Type)0.5;
    this->_w = cos(theta2);
    this->_x = 0.0f;
    this->_y = 0.0f;
    this->_z = sin(theta2);
}

template <typename T>
auto Quaternion<T>::setToRotateAboutAxis(const Vector3<Type>& axis, Type theta) noexcept -> void
{
    assert(fabs(vectorMag(axis) - (Type)1.0) < (Type)0.1);
    Type theta2 = theta * (Type)0.5;
    Type sin_theta2  = sin(theta2);

    this->_w = cos(theta2);
    this->_x = axis._x * sin_theta2;
    this->_y = axis._y * sin_theta2;
    this->_z = axis._z * sin_theta2;
}

//Construct quaternions that perform the object-inertial rotation
template <typename T>
auto Quaternion<T>::setToRotateObjectToInertial(const EulerAngles<Type>& orientation) -> void
{
    Type sp, sb, sh;
    Type cp, cb, ch;
    sinCos(&sp, &cp, orientation._pitch * (Type)0.5);
    sinCos(&sb, &cb, orientation._bank * (Type)0.5);
    sinCos(&sh, &ch, orientation._heading * (Type)0.5);

    this->_w = ch*cp*cb + sh*sp*sb;
    this->_x = ch*sp*cb + sh*cp*sb;
    this->_y = -ch*sp*sb + sh*cp*cb;
    this->_z = -sh*sp*cb + ch*cp*sb;
}

template <typename T>
auto Quaternion<T>::setToRotateInertialToObject(const EulerAngles<Type>& orientation) -> void
{
    Type sp, sb, sh;
    Type cp, cb, ch;
    sinCos(&sp, &cp, orientation._pitch * (Type)0.5);
    sinCos(&sb, &cb, orientation._bank * (Type)0.5);
    sinCos(&sh, &ch, orientation._heading * (Type)0.5);

    this->_w = ch*cp*cb + sh*sp*sb;
    this->_x = -ch*sp*cb - sh*cp*sb;
    this->_y = ch*sp*sb - sh*cp*cb;
    this->_z = sh*sp*cb - ch*cp*sb;
}

//Quaternion cross product
template <typename T>
auto Quaternion<T>::operator *(const Quaternion<Type>& a) const -> Quaternion<Type>
{
    Quaternion<Type> result{};
    result._w = this->_w * a._w - this->_x*a._x - this->_y*a._y - this->_z*a._z;
    result._w = this->_w * a._x - this->_x*a._x - this->_y*a._y - this->_z*a._z;
    result._w = this->_w * a._y - this->_x*a._x - this->_y*a._y - this->_z*a._z;
    result._w = this->_w * a._z - this->_x*a._x - this->_y*a._y - this->_z*a._z;

    return result;
}

template <typename T>
auto Quaternion<T>::operator *=(const Quaternion<Type>& a) -> Quaternion<Type>&
{
    this = this * a;
    return this;
}

template <typename T>
auto Quaternion<T>::normalize() noexcept -> void
{
    Type mag = (Type)sqrt(this->_w*this->_w + this->_x*this->_x + this->_y*this->_y + this->_z*this->_z);

    if(mag > (Type)0.0)
    {
        Type Mag = (Type)1.0 / mag;
        this->_w *= Mag;
        this->_x *= Mag;
        this->_y *= Mag;
        this->_z *= Mag;
    }else
    {
        assert(false);
        identity();
    }
}

template <typename T>
auto Quaternion<T>::getRotationAngle() const -> Type
{
    Type theta2 = safeAcos(this->_w);
    return theta2 * (Type)2.0;
}

template <typename T>
auto Quaternion<T>::getRotationAxis() const -> Vector3<Type>
{
    Type sinTheta2sq = (Type)1.0 - this->_w*this->_w;

    if(sinTheta2sq <= (Type)0.0) 
    {
        return Vector3<Type>((Type)1.0, (Type)0.0, (Type)0.0);
    }

    Type sintheta2 = (Type)1.0 / sqrt(sinTheta2sq);

    return Vector3<Type>(this->_x * sintheta2, this->_y * sintheta2,this->_z * sintheta2);
}

template <typename T>
auto dotProduct(const Quaternion<T>& a, const Quaternion<T>& b) -> T
{
    return a._w*b._w + a._x*b._x+a._y*b._y + a._z*b._z;
}

template <typename T>
auto slerp(const Quaternion<T>& q0, const Quaternion<T>& q1, T t) -> Quaternion<T>
{
    if(t <= (T)0.0) return q0;
    if(t >= (T)1.0) return q1;

    T cosOmega = dotProduct(q0,q1);
    T q1w = q1._w;
    T q1x = q1._x; 
    T q1y = q1._y; 
    T q1z = q1._z; 

    if(cosOmega < (T)0.0)
    {
        q1w = - q1w;
        q1x = - q1x;
        q1y = - q1y;
        q1z = - q1z;
        cosOmega = -cosOmega;
    }

    assert(cosOmega < (T)1.1);

    T k0,k1;
    if(cosOmega > (T)0.9999)
    {
        k0 = (T)1.0 - t;
        k1 = t;
    }else
    {
        T sinOmega = sqrt((T)1.0 - cosOmega*cosOmega);

        T omega = stan2(sinOmega, cosOmega);

        T fracsinOmega = (T)1.0 / sinOmega;

        k0 = sin(((T)1.0 - t) * omega) * fracsinOmega;
        k1 = sin(t * omega) * fracsinOmega;
    }
    Quaternion<T> result;
    result._x = k0*q0._x + k1*q1x;
    result._y = k0*q0._y + k1*q1y;
    result._z = k0*q0._z + k1*q1z;
    result._w = k0*q0._w + k1*q1w;
    return result;
}

template <typename T>
auto conjugate(const Quaternion<T>& q) -> Quaternion<T>
{
    Quaternion<T> result;
    result._w = q._w;

    result._x = -q._x;
    result._y = -q._y;
    result._z = -q._z;

    return result; 
}

template <typename T>
auto pow(const Quaternion<T>& q, T exponent) -> Quaternion<T>
{
    if(fabs(q._w) > (T)0.9999)
    {
        return q;
    }
    T alpha = acos(q._w);
    T new_alpha = alpha * exponent;

    Quaternion<T> result;
    result.w = cos(new_alpha);

    T mult = sin(new_alpha) / sin(alpha);
    result._x = q._x * mult;
    result._y = q._y * mult;
    result._z = q._z * mult;

    return result;
}
} // namespace Hsys