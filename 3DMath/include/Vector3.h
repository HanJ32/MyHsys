#pragma once
#ifndef _HSYS_VECTOR3_H_
#define _HSYS_VECTOR3_H_

namespace Hsys
{

template<class T = float>
class Vector3
{

public:
    using Type = T;
    Type _x,_y,_z;

    Vector3() = default;
    Vector3(const Vector3& a): _x(a._x),_y(a._y),_z(a._z){}
    Vector3(Type x,Type y, Type z):_x(x),_y(y),_z(z){}
    auto operator = (const Vector3& a) -> Vector3& 
    {
        this->_x = a._x;
        this->_y = a._y;
        this->_z = a._z;
        return this;
    }

    auto operator == (const Vector3& a) const -> bool
    {
        return this->_x == a._x && this->_y == a._y && this->_z == a._z;
    }
    auto operator != (const Vector3& a) const -> bool
    {
        return this->_x != a._x || this->_y != a._y || this->_z != a._z;
    }

    auto zero() -> void
    {
        this->_x = 0.0f;
        this->_y = 0.0f;
        this->_z = 0.0f;
    }
    
    auto operator - () const -> Vector3
    {
        return Vector3(-this->_x,-this->_y,-this->_z);
    }

    auto operator - (const Vector3& a) const -> Vector3
    {
        return Vector3(this->_x - a._x, this->_y - a._y, this->_z - a._z);
    }

    auto operator + (const Vector3& a) const -> Vector3
    {
        return Vector3(this->_x + a._x, this->_y + a._y, this->_z + a._z);
    }

    auto operator * (Type a) const -> Vector3
    {
        return Vector3(this->_x*a, this->_y*a,this->_z*a);
    }

    auto operator / (Type a) const -> Vector3
    {
        T fracA = ((Type)1.0) / a;
        return Vector3(this->_x*fracA, this.y *fracA, this->_z*fracA);
    }

    auto operator +=(const Vector3 &a) -> Vector3&
    {
        this->_x += a._x;
        this->_y += a._y;
        this->_z += a._z;
        return this;
    }

    auto operator -=(const Vector3 &a) -> Vector3&
    {
        this->_x -= a._x;
        this->_y -= a._y;
        this->_z -= a._z;
        return this;
    }

    auto operator *=(Type a) -> Vector3&
    {
        this->_x *= a;
        this->_y *= a;
        this->_z *= a;
        return this;
    }
    auto operator /=(Type a) -> Vector3&
    {
        T fracA = ((Type)1.0) / a;
        this->_x *= fracA;
        this->_y *= fracA;
        this->_z *= fracA;
        return this;
    }

    auto normalize() throw() -> void
    {
        Type magSq = (_x*_x) + (_y*_y) + (_z*_z);
        if(magSq > (Type)0.0)
        {
            Type fracMag = (Type)1.0 / sqrt(magSq);
            this->_x *= fracMag;
            this->_y *= fracMag;
            this->_z *= fracMag;
        }
    }

    auto operator *(const Vector3& a) const -> float
    {
        return this->_x*a._x + this->_y*a._y+this->_z*a._z;
    }
};

template <typename T = float>
auto vectorMag(const Vector3<T>& a) -> T
{
    return sqrt(a._x*a._x + a._y*a._y + a._z*a._z);
}

template <typename T = float>
auto crossProduct(const Vector3<T>& a, const Vector3<T>& b) -> Vector3<T>
{
    return Vector3<T>(
        a._y*b._z - a._z*b._y,
        a._z*b._x - a._x*b._z,
        a._x*b._y - a._y*b._x
    );
}
template <typename T = float>
auto operator * (T k, const Vector3<T> &v) -> Vector3<T>
{
    return Vector3<T>(k*v._x, k*v._y, k*v._z);
}
template <typename T = float>
auto distance(const Vector3<T>& a, const Vector3<T>& b) -> T
{
    T dx = a._x - b._x;
    T dy = a._y - b._y;
    T dz = a._z - b._z;
    return sqrt(dx*dx + dy*dy + dz*dz);
}
template <typename T>
extern const Vector3<T> kZeroVector{};

}; // namespace Hsys

#endif //_HSYS_VECTOR3_H_