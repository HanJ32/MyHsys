#include <Vector3.h>
#include <MathUtil.h>
#include <Quaternion.h>
#include <EulerAngles.h>
#include <RotationMatrix.h>
//#include <Matrix4x3.h>

namespace Hsys {

auto identity() noexcept -> void;
template <typename T>
auto RotationMatrix<T>::identity() noexcept -> void
{
    unsigned char row{0},col{0};
    for(auto index : I_3x3<Type>)
    {
        for(auto index2 : I_3x3<Type>[row])
        {
            this->_Ma3x3[row][col] = index2;
            ++col;
        }
        ++row; 
    }
}

template <typename T>
auto RotationMatrix<T>::setup(const EulerAngles<Type>& orientation) noexcept -> void
{
    float sh,ch,sp,cp,sb,cb;
    sinCos(sh, ch, orientation._heading);
    sinCos(sp, cp, orientation._pitch);
    sinCos(sb, cb, orientation._bank);

    this->_Ma3x3[0][0] = ch*cb + sh*sp*sb;
    this->_Ma3x3[0][1] = -ch*sb + sh*sp*cb;
    this->_Ma3x3[0][2] = sh*cp;

    this->_Ma3x3[1][0] = sb*cp;
    this->_Ma3x3[1][1] = cb*cp;
    this->_Ma3x3[1][2] = -sp;

    this->_Ma3x3[2][0] = -sh*cb + ch*sp*sb;
    this->_Ma3x3[2][1] = sb*sh + ch*sp*cb;
    this->_Ma3x3[2][2] = ch*cp;
}

template <typename T>
auto RotationMatrix<T>::fromObjectToInertialQuaternion(const Quaternion<Type>& q) throw() -> void
{
    this->_Ma3x3[0][0] = (Type)1.0 - (Type)2.0 * (q._y*q._y + q._z*q._z);
    this->_Ma3x3[0][1] = (Type)2.0 * (q._x*q._y + q._w*q._z);
    this->_Ma3x3[0][2] = (Type)2.0 * (q._x*q._z - q._w*q._y);

    this->_Ma3x3[1][0] = (Type)2.0 * (q._x*q._y - q._w*q._z);
    this->_Ma3x3[1][1] = (Type)1.0 - (Type)2.0 * (q._x*q._x + q._z*q._z);
    this->_Ma3x3[1][2] = (Type)2.0 * (q._y*q._z + q._w*q._x);

    this->_Ma3x3[2][0] = (Type)2.0 * (q._x*q._z + q._w*q._y);
    this->_Ma3x3[2][1] = (Type)2.0 * (q._y*q._z - q._w*q._x);
    this->_Ma3x3[2][2] = (Type)1.0 - (Type)2.0 * (q._x*q._x + q._y* q._y);
}

template <typename T>
auto RotationMatrix<T>::inertialToObject(const Vector3<Type>& v) const -> Vector3<Type>
{
    Type si1[3];
    for(unsigned char col = 0; col < 3; col+=1)
    {
        si1[col] = this->_Ma3x3[0][col]*v._x + this->_Ma3x3[1][col]*v._y + this->_Ma3x3[2][col]*v._z;
    }
    return Vector3<Type>(si1[0], si1[1], si1[2]);
}

template <typename T>
auto RotationMatrix<T>::objectToInertial(const Vector3<Type>& v) const -> Vector3<Type>
{
    Type si1[3];
    for(unsigned char row = 0; row < 3; row+=1)
    {
        si1[row] = this->_Ma3x3[row][0]*v._x + this->_Ma3x3[row][1]*v._y + this->_Ma3x3[row][2]*v._z;
    }
    return Vector3<Type>(si1[0], si1[1], si1[2]);
}

}; //Hsys namespace