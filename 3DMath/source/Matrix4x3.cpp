#include <MathUtil.h>
#include <Vector3.h>
#include <Matrix4x3.h>
#include <EulerAngles.h>
#include <assert.h>
#include <math.h>
#include <RotationMatrix.h>

namespace Hsys
{
template <typename T>
auto Matrix4x3<T>::setI() -> void
{
    unsigned char row{0},col{0};
    for(auto index : I_3x3<Type>)
    {
        for(auto index2 : I_3x3<Type>[row])
        {
            this->_Ma4x3[row][col] = index2;
            ++col;
        }
        ++row; 
    }
}

template <typename T>
auto Matrix4x3<T>::identity() noexcept -> void
{
    for(unsigned char index = 0; index < 3; index+=1)
    {
        this->_Ma4x3[0][index] = I_3x3<Type>[0][index];
        this->_Ma4x3[1][index] = I_3x3<Type>[1][index];
        this->_Ma4x3[2][index] = I_3x3<Type>[2][index];
        this->_Ma4x3[3][index] = I_4x4<Type>[2][index];
    }
}

template <typename T>
auto Matrix4x3<T>::zeroTranslation() noexcept -> void
{
    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}

template <typename T>
auto Matrix4x3<T>::setTranslation(const Vector3<Type>& d) noexcept -> void
{
    this->_Ma4x3[3][0] = d._x;
    this->_Ma4x3[3][1] = d._y;
    this->_Ma4x3[3][2] = d._z;
}

template <typename T>
auto Matrix4x3<T>::setupTranslation(const Vector3<Type>& d) noexcept -> void
{
    this->setI();

    this->_Ma4x3[3][0] = d._x;
    this->_Ma4x3[3][1] = d._y;
    this->_Ma4x3[3][1] = d._z;
}


// template <typename T>
// auto Matrix4x3<T>::setupTranslation(const Vector3<Type>& d) noexcept -> void
// {

// }

template <typename T>
auto Matrix4x3<T>::setupLocalToParent(const Vector3<Type>& pos, const EulerAngles<Type>& orient) -> void
{
    RotationMatrix<Type> orientMatrix;
    orientMatrix.setup(orient);

    setupLocalToParent(pos, orientMatrix);
}

template <typename T>
auto Matrix4x3<T>::setupLocalToParent(const Vector3<Type>& pos, const RotationMatrix<Type>& orient) -> void
{
    for(unsigned char index = 0; index < 3; index+=1)
    {
        this->_Ma4x3[index][0] = orient._Ma3x3[index][0];
        this->_Ma4x3[index][1] = orient._Ma3x3[index][1];
        this->_Ma4x3[index][2] = orient._Ma3x3[index][2];
    }
    this->_Ma4x3[3][0] = pos._x;
    this->_Ma4x3[3][1] = pos._y;
    this->_Ma4x3[3][2] = pos._z;
}


template <typename T>
auto Matrix4x3<T>::setupParentToLocal(const Vector3<Type>& pos, const EulerAngles<Type>& orient) -> void
{
    RotationMatrix<Type> orientMatrix;
    orientMatrix.setup(orient);
    setupParentToLocal(pos, orientMatrix);
}

template <typename T>
auto Matrix4x3<T>::setupParentToLocal(const Vector3<Type>& pos, const RotationMatrix<Type>& orient) -> void
{
    for(unsigned char index = 0; index < 3; index+=1)
    {
        this->_Ma4x3[0][index] = orient._Ma3x3[0][index];
        this->_Ma4x3[1][index] = orient._Ma3x3[1][index];
        this->_Ma4x3[2][index] = orient._Ma3x3[2][index];
    }

    this->_Ma4x3[3][0] = -(pos._x*this->_Ma4x3[0][0] + pos._y*this->_Ma4x3[1][0] + pos._z*this->_Ma4x3[2][0]);
    this->_Ma4x3[3][1] = -(pos._x*this->_Ma4x3[0][1] + pos._y*this->_Ma4x3[1][1] + pos._z*this->_Ma4x3[2][1]);
    this->_Ma4x3[3][2] = -(pos._x*this->_Ma4x3[0][2] + pos._y*this->_Ma4x3[1][2] + pos._z*this->_Ma4x3[2][2]);
}


template <typename T>
auto Matrix4x3<T>::setupRotate(int axis, Type theta) -> void
{
    Type sinvalue,cosvalue;
    sinCos(sinvalue,cosvalue, theta);

    switch (axis)
    {
    case 1:// Around X rotation
        this->_Ma4x3[0][0] = (Type)1.0;
        this->_Ma4x3[0][1] = this->_Ma4x3[0][2] = (Type)0.0;
        this->_Ma4x3[1][0] = this->_Ma4x3[2][0] = (Type)0.0;
        this->_Ma4x3[1][1] = this->_Ma4x3[2][2] = cosvalue;
        this->_Ma4x3[1][2] = sinvalue;
        this->_Ma4x3[2][1] = -sinvalue;
        break;
    case 2:// Around Y rotation
        this->_Ma4x3[1][1] = (Type)1.0;
        this->_Ma4x3[0][1] = this->_Ma4x3[1][0] = (Type)0.0;
        this->_Ma4x3[1][2] = this->_Ma4x3[2][1] = (Type)0.0;
        this->_Ma4x3[0][0] = this->_Ma4x3[2][2] = cosvalue;
        this->_Ma4x3[2][0] = sinvalue;
        this->_Ma4x3[0][2] = -sinvalue;
        break;
    case 3:// Around Z rotation
        this->_Ma4x3[2][2] = (Type)1.0;
        this->_Ma4x3[0][2] = this->_Ma4x3[2][0] = (Type)0.0;
        this->_Ma4x3[1][2] = this->_Ma4x3[2][1] = (Type)0.0;
        this->_Ma4x3[0][0] = this->_Ma4x3[1][1] = cosvalue;
        this->_Ma4x3[0][1] = sinvalue;
        this->_Ma4x3[1][0] = -sinvalue;
        break;
    default:
        assert(false);
        break;
    }
    this->_Ma4x3[3][0] = (Type)0.0;
    this->_Ma4x3[3][1] = (Type)0.0;
    this->_Ma4x3[3][2] = (Type)0.0;
}
template <typename T>
auto Matrix4x3<T>::setupRotate(const Vector3<Type>& axis, Type theta) -> void
{
    assert(fabs(axis*axis - (Type)1.0) < (Type)0.01);

    Type sinvalue, cosvalue;
    sinCos(sinvalue, cosvalue,theta);

    Type Onemoun = 1.0f - cosvalue;
    Type x = Onemoun * axis._x;
    Type y = Onemoun * axis._y;
    Type z = Onemoun * axis._z;

    this->_Ma4x3[0][0] = x*axis._x + cosvalue;
    this->_Ma4x3[0][1] = y*axis._y + axis._z*sinvalue;
    this->_Ma4x3[0][2] = z*axis._z - axis._y*sinvalue;
    this->_Ma4x3[1][0] = x*axis._x - axis._z*sinvalue;
    this->_Ma4x3[1][1] = y*axis._y + cosvalue;
    this->_Ma4x3[1][2] = z*axis._z + axis._x*sinvalue;
    this->_Ma4x3[2][0] = z*axis._x + axis._y*sinvalue;
    this->_Ma4x3[2][1] = z*axis._y - axis._x*sinvalue;
    this->_Ma4x3[2][2] = z*axis._z + cosvalue;

    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}
template <typename T>
auto Matrix4x3<T>::fromQuaternion(const Quaternion<Type>& q) -> void
{
    Type ww = (Type)2.0 * q._w;
    Type xx = (Type)2.0 * q._x;
    Type yy = (Type)2.0 * q._y;
    Type zz = (Type)2.0 * q._z;

    this->_Ma4x3[0][0] = (Type)1.0 - yy*q._y - zz*q._z;
    this->_Ma4x3[0][1] = xx*q._y + ww*q._z;
    this->_Ma4x3[0][2] = xx*q._z - ww*q._x;

    this->_Ma4x3[1][0] = xx*q._y - ww*q._z;
    this->_Ma4x3[1][1] = (Type)1.0 - xx*q._x - zz*q._z;
    this->_Ma4x3[1][2] = yy*q._z + ww*q._z;

    this->_Ma4x3[2][0] = xx*q._z + ww*q._y;
    this->_Ma4x3[2][1] = yy*q._z - ww*q._x;
    this->_Ma4x3[2][2] = (Type)1.0 - xx*q._x - yy*q._y;


    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}
template <typename T>
auto Matrix4x3<T>::setupScale(const Vector3<Type>& s) -> void
{
    this->_Ma4x3[0][1] = (Type)0.0;
    this->_Ma4x3[0][2] = (Type)0.0;

    this->_Ma4x3[1][2] = (Type)0.0;
    this->_Ma4x3[2][1] = (Type)0.0;

    this->_Ma4x3[1][0] = (Type)0.0;
    this->_Ma4x3[2][1] = (Type)0.0;

    this->_Ma4x3[0][0] = s._x;
    this->_Ma4x3[1][1] = s._y;
    this->_Ma4x3[2][2] = s._z;

    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}
template <typename T>
auto Matrix4x3<T>::setupScaleAlongAxis(const Vector3<Type>& axis, float k) -> void
{
    assert(fabs(axis * axis - (Type)1.0) < (Type)0.01);

    Type subone = k - 1.0;
    Type x = subone * axis._x;
    Type y = subone * axis._y;
    Type z = subone * axis._z;

    this->_Ma4x3[0][0] = x * axis._x + (Type)1.0;
    this->_Ma4x3[1][1] = y * axis._y + (Type)1.0;
    this->_Ma4x3[2][1] = z * axis._z + (Type)1.0;

    this->_Ma4x3[0][1] = this->_Ma4x3[1][0] = x*axis._y;
    this->_Ma4x3[0][2] = this->_Ma4x3[2][0] = x*axis._z;
    this->_Ma4x3[1][2] = this->_Ma4x3[2][1] = y*axis._z;

    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}

template <typename T>
auto Matrix4x3<T>::setupShear(int axis, Type s, Type t) -> void
{
    switch (axis)
    {
    case 1:
        this->setI();
        this->_Ma4x3[0][1] = s;
        this->_Ma4x3[0][2] = t;
        break;

    case 2:
        this->setI();
        this->_Ma4x3[1][0] = s;
        this->_Ma4x3[1][2] = t;
        break;

    case 3:
        this->setI();
        this->_Ma4x3[2][0] = s;
        this->_Ma4x3[2][1] = t;
        break;
    default:
    assert(false);
        break;
    
    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
    }
}

template <typename T>
auto Matrix4x3<T>::setupProject(const Vector3<Type>& n) -> void
{
    assert(fabs(n*n - (Type)1.0) < (Type)0.01);

    this->_Ma4x3[0][0] = (Type)1.0 - n._x*n._x;
    this->_Ma4x3[1][1] = (Type)1.0 - n._y*n._y;
    this->_Ma4x3[2][2] = (Type)1.0 - n._z*n._z;
    
    this->_Ma4x3[0][1] = this->_Ma4x3[1][0] = - n._x*n._y;
    this->_Ma4x3[0][2] = this->_Ma4x3[2][0] = - n._x*n._z;
    this->_Ma4x3[1][2] = this->_Ma4x3[2][1] = - n._y*n._z;

    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}
template <typename T>
auto Matrix4x3<T>::setupReflect(int axis, Type k) -> void
{
    switch (axis)
    {
    case 1:
        this->setI();
        this->_Ma4x3[0][0] = -1.0f;

        this->_Ma4x3[3][0] = 2.0f * k;
        this->_Ma4x3[3][1] = 0.0f;
        this->_Ma4x3[3][2] = 0.0f;
        break;

    case 2:
        this->setI();
        this->_Ma4x3[1][1] = -1.0f;

        this->_Ma4x3[3][0] = 0.0f;
        this->_Ma4x3[3][1] = 2.0f * k;
        this->_Ma4x3[3][2] = 0.0f;
        break;
    
    case 3:
        this->setI();
        this->_Ma4x3[2][2] = -1.0f;

        this->_Ma4x3[3][0] = 0.0f;
        this->_Ma4x3[3][1] = 0.0f;
        this->_Ma4x3[3][2] = 2.0f * k;
    default:
        assert(false);
        break;
    }
}

template <typename T>
auto Matrix4x3<T>::setupReflect(const Vector3<Type>& n) -> void
{
    assert(fabs(n*n - (Type)1.0) < (Type)1.0);

    Type x = -(Type)2.0 * n._x;
    Type y = -(Type)2.0 * n._y;
    Type z = -(Type)2.0 * n._z;

    this->_Ma4x3[0][0] = (Type)1.0 + x*n._x;
    this->_Ma4x3[1][1] = (Type)1.0 + y*n._y;
    this->_Ma4x3[2][1] = (Type)1.0 + z*n._z;

    this->_Ma4x3[0][1] = this->_Ma4x3[1][0] = x*n._y;
    this->_Ma4x3[0][2] = this->_Ma4x3[2][0] = x*n._z;
    this->_Ma4x3[1][2] = this->_Ma4x3[2][1] = y*n._z;

    this->_Ma4x3[3][0] = this->_Ma4x3[3][1] = this->_Ma4x3[3][2] = (Type)0.0;
}

template<typename T>
auto operator *(const Vector3<T>& p, const Matrix4x3<T>& m) -> Vector3<T>
{
    T fon[3];
    for(unsigned char index = 0; index < 3; index+=1)
    {
        fon[index] = p._x*m._Ma4x3[0][index] + p._y*m._Ma4x3[1][index] + p._z*m._Ma4x3[2][index] + m._Ma4x3[3][index];
    }
    return Vector3<T>(fon[0],fon[1],fon[2]);
}

template <typename T>
auto operator *=(Vector3<T>& p, const Matrix4x3<T>& m) -> Vector3<T>&
{
    p = p * m;
    return p;
}


template <typename T>
auto operator *(const Matrix4x3<T>& a, const Matrix4x3<T>& b) -> Matrix4x3<T>
{
    Matrix4x3<T> result;

    for(unsigned char index = 0; index < 3; index+=1)
    {
        for(unsigned char index2 = 0; index2 < 3; index2 +=1)
        {
            result._Ma4x3[index][index2] = a._Ma4x3[index][0]*b._Ma4x3[0][index2] + a._Ma4x3[index][1]*b[1][index2] + a._Ma4x3[index][2]*b[2][index2];
        }
        result._Ma4x3[3][index] = a._Ma4x3[3][0] * b._Ma4x3[0][index] + a._Ma4x3[3][1]*b._Ma4x3[1][index] + a._Ma4x3[3][2]*b._Ma4x3[2][index] + b._Ma4x3[3][index];
    }
    return result;
}

template <typename T>
auto operator *=(Matrix4x3<T> &a, const Matrix4x3<T>& b) -> Matrix4x3<T>&
{
    a = a * b;
    return a;
}

template <typename T>
auto determinant(const Matrix4x3<T>& m) -> T
{
    T result = m._Ma4x3[0][0] * (m._Ma4x3[1][1]*m._Ma4x3[2][2] - m._Ma4x3[1][2]*m._Ma4x3[2][1]) 
                + m._Ma4x3[0][1] * (m._Ma4x3[1][2]*m._Ma4x3[2][0] - m._Ma4x3[1][0]*m._Ma4x3[2][2])
                + m._Ma4x3[1][3] * (m._Ma4x3[1][0]*m._Ma4x3[2][1] - m._Ma4x3[1][1]*m._Ma4x3[2][0]);
    return result;
}

template <typename T>
auto inverse(const Matrix4x3<T>& m) -> Matrix4x3<T>
{
    T det = determinant(m);

    assert(fabs(det) > 0.000001f);

    T fracdet = (T)1.0 / det;

    Matrix4x3<T> result;
    result._Ma4x3[0][0] = (m._Ma4x3[1][1]*m._Ma4x3[2][2] - m._Ma4x3[1][2]*m._Ma4x3[2][1]) * fracdet;
    result._Ma4x3[0][1] = (m._Ma4x3[0][2]*m._Ma4x3[2][1] - m._Ma4x3[0][1]*m._Ma4x3[2][2]) * fracdet;
    result._Ma4x3[0][2] = (m._Ma4x3[0][1]*m._Ma4x3[1][2] - m._Ma4x3[0][2]*m._Ma4x3[1][1]) * fracdet;

    result._Ma4x3[1][0] = (m._Ma4x3[1][2]*m._Ma4x3[2][0] - m._Ma4x3[1][0]*m._Ma4x3[2][2]) * fracdet;
    result._Ma4x3[1][1] = (m._Ma4x3[0][0]*m._Ma4x3[2][2] - m._Ma4x3[0][2]*m._Ma4x3[2][1]) * fracdet;
    result._Ma4x3[1][2] = (m._Ma4x3[0][2]*m._Ma4x3[1][0] - m._Ma4x3[0][0]*m._Ma4x3[1][2]) * fracdet;
    
    result._Ma4x3[2][0] = (m._Ma4x3[1][0]*m._Ma4x3[2][1] - m._Ma4x3[1][1]*m._Ma4x3[2][0]) * fracdet;
    result._Ma4x3[2][1] = (m._Ma4x3[0][1]*m._Ma4x3[2][0] - m._Ma4x3[0][0]*m._Ma4x3[2][1]) * fracdet;
    result._Ma4x3[2][2] = (m._Ma4x3[0][0]*m._Ma4x3[1][1] - m._Ma4x3[0][1]*m._Ma4x3[1][0]) * fracdet;

    for(unsigned char index = 0; index < 3; index+=1)
    {
        result._Ma4x3[3][index] = -(m._Ma4x3[3][0]*result._Ma4x3[0][index] + m._Ma4x3[3][1]*result._Ma4x3[1][index] + m._Ma4x3[3][2]*result._Ma4x3[3][index]);
    }

    return result;
}

template <typename T>
auto getTranslation(const Matrix4x3<T>& m) -> Vector3<T>
{
    return Vector3<T>(m._Ma4x3[3][0], m._Ma4x3[3][1], m._Ma4x3[3][2]);
}

template <typename T>
auto getPositionFromParentToLocalMatrix(const Matrix4x3<T>& m)
{
    T fon[3];
    for(unsigned char index = 0; index < 3; index+=1)
    {
        fon[index] = -(m._Ma4x3[3][0]*m._Ma4x3[index][0] + m._Ma4x3[3][1]*m._Ma4x3[index][1] + m._Ma4x3[3][2]*m._Ma4x3p[index][2]);
    }
    return Vector3<T>(fon[0],fon[1],fon[2]);
}

template <typename T>
auto getPositionFromLocalToParentMatrix(const Matrix4x3<T>& m) -> Vector3<T>
{
    return Vector3<T>(m._Ma4x3[3][0], m._Ma4x3[3][1], m._Ma4x3[3][2]);
}
}