#pragma once
#ifndef _HSYS_EULERANGLES_H_
#define _HSYS_EULERANGLES_H_


namespace Hsys
{
template<class T>
class Quaternion;

template<class T>
class Matrix4x3;

template<class T>
class RotationMatrix;

template<class T = float>
class EulerAngles
{
public:
    using Type = T;
    Type _heading;
    Type _pitch;
    Type _bank;


    EulerAngles(/* args */) = default;
    EulerAngles(Type heading, Type pitch, Type bank)
    :_heading(heading), _pitch(pitch),_bank(bank) {}

    //zero setting
    auto identity() noexcept -> void
    {
        this->_pitch = (Type)0.0;
        this->_bank = (Type)0.0;
        this->_heading = (Type)0.0;
    }

    //Transform to "restricted set" Euler Angle
    auto canonize() -> void;

    //From quaternion to Euler Angle
    //The matrices are assumed to be orthogonal
    auto fromObjectToInertialQuaternion(const Quaternion<Type>& q) throw() -> void;
    auto fromInertialToObjectQuaternion(const Quaternion<Type>& q) throw() -> void;
    
    //From matrix to Euler Angle
    auto fromObjectToWorldMatrix(const Matrix4x3<Type>& m) throw() -> void;
    auto fromWorldToObjectMatrix(const Matrix4x3<Type>& m) throw() -> void;

    //From matrix to Euler Angle
    auto fromRotationMatrix(const RotationMatrix<Type>& m) throw() -> void;
};

template <typename T = float>
extern const EulerAngles<T> kEulerAnglesIdentity{};

//Quaternion point multiplication
template <typename T = float>
extern T dotProduct(const Quaternion<T>&a, const Quaternion<T>& b);

//Spherical interpolation
template <typename T = float>
extern Quaternion<T> slerp(const Quaternion<T>& p, const Quaternion<T>& q, T thera);

//Quaternion conjugation
template <typename T = float>
extern Quaternion<T> conjugate(const Quaternion<T>& q);

//Quaternion powers(Quaternion Fourier transform)
template <typename T = float>
extern Quaternion<T> pow(const Quaternion<T>& q, T exponent);
} // namespace Hsys

#endif //_HSYS_EULERANGLES_H_