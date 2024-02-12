#include <math.h>

#include <EulerAngles.h>
#include <Quaternion.h>
#include <MathUtil.h>
#include <Matrix4x3.h>
#include <RotationMatrix.h>

namespace Hsys
{
    template <typename T>
    auto EulerAngles<T>::canonize() -> void
    {
        this->_pitch = wrapPi(this->_pitch);

        if(this->_pitch < -kPiOver2)
        {
            this->_pitch = -kPi - this->_pitch;
            this->_heading += kPi;
            this->_bank += kPi;
        }else if(this->_pitch > kPiOver2)
        {
            this->_pitch = kPi - this->_pitch;
            this->_heading += kPi;
            this->bank += kPi;
        }

        if(fabs(this->_pitch > kPiOver2))
        {
            this->_heading += this->_bank;
            this->_bank = (Type)0.0;
        }else
        {
            this->_bank = wrapPi(this->_bank);
        }

        this->_heading = wrapPi(this->_heading);
    }

    template <typename T>
    auto EulerAngles<T>::fromObjectToInertialQuaternion(const Quaternion<T>& q) throw() -> void
    {
        Type sp = (Type)-2.0 * (q._y*q._z - q._w*q._x);

        if(fabs(sp) > (Type)0.9999)
        {
            this->_pitch = kPiOver2 * sp;
            this->_heading = atan2(-q._x*q._z + q._w*q._y, (Type)0.5 - q._y*q._y - q._z*q._z);
            this->_bank = (Type)0.0;
        }else
        {
            this->_pitch = asin(sp);
            this->_heading = atan2(q._x*q._z + q._w*q._y, (Type)0.5 - q._x*q._x - q._y*q._y);
            this->_bank = atan2(q._x*q._y + q._w*q._z, (Type)0.5 - q._x*q._x - q._z*q._z);
        }
    }

    template <typename T>
    auto EulerAngles<T>::fromInertialToObjectQuaternion(const Quaternion<T>& q) throw() -> void
    {
        Type sp = (Type)-2.0 * (q._y*q._z + q._w*q._x);

        if(fabs(sp) > (Type)0.9999)
        {
            this->_pitch = kPiOver2 * sp;
            this->_heading = atan2(-q._x*q._z - q._w*q._y, (Type)0.5 - q._y*q._y - q._z*q._z);
            this->_bank = (Type)0.0;
        }else
        {
            this->_pitch = asin(sp);
            this->_heading = atan2(q._x*q._z - q._w*q._y, (Type)0.5 - q._x*q._x - q._y*q._y);
            this->_bank = atan2(q._x*q._y - q._w*q._z, (Type)0.5 - q._x*q._x - q._z*q._z);
        }
    }

    template <typename T>
    auto EulerAngles<T>::fromObjectToWorldMatrix(const Matrix4x3<Type>& m) throw() -> void
    {
        Type sp = -m._Ma4x3[2][1];

        if(fabs(sp) > (Type)9.99999)
        {
            this->_pitch = kPiOver2 * sp;
            this->_heading = atan2(-m._Ma4x3[1][2],m._Ma4x3[0][0]);
            this->_bank = (Type)0.0;
        }else
        {
            this->_heading = atan2(m._Ma4x3[2][0], m._Ma4x3[2][2]);
            this->_pitch = asin(sp);
            this->_bank = atan2(m._Ma4x3[0][1],m._Ma4x3[1][1]);
        }
    }

    template <typename T>
    auto EulerAngles<T>::fromWorldToObjectMatrix(const Matrix4x3<Type>& m) throw() -> void
    {
        Type sp = -m._Ma4x3[1][2];

        if(fabs(sp) > (Type)9.99999)
        {
            this->_pitch = kPiOver2 * sp;
            this->_heading = atan2(-m._Ma4x3[2][0], m._Ma4x3[0][0]);
            this->_bank = (Type)0.0;
        }else
        {
            this->_heading = atan2(m._Ma4x3[0][2], m._Ma4x3[2][2]);
            this->_pitch = asin(sp);
            this->_bank = atan2(m._Ma4x3[1][0], m._Ma4x3[1][1]);
        }
    }

    template <typename T>
    auto EulerAngles<T>::fromRotationMatrix(const RotationMatrix<Type>& m) throw() -> void
    {
        Type sp = -m._Ma3x3[1][2];

        if(fabs(sp) > (Type)9.99999)
        {
            this->_pitch = kPiOver2 * sp;
            this->_heading = atan2(-m._Ma3x3[2][0], m._Ma3x3p[0][0]);
            this->_bank = (Type)0.0;
        }else
        {
            this->_heading = atan2(m._Ma3x3[0][2], m._Ma3x3[2][2]);
            this->_pitch = asin(sp);
            this->_bank = atan2(m._Ma3x3[1][0],m._Ma3x3[1][1]);
        }
    }
}