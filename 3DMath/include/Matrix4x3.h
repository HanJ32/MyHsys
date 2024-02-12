#pragma once
#ifndef _HSYS_MATRIX4X3_H_
#define _HSYS_MATRIX4X3_H_

#include <basemath.h>

namespace Hsys
{
template <typename T>
class Vector3;

template <typename T>
class EulerAngles;

template <typename T>
class Quaternion;

template <typename T>
class RotationMatrix;

template <typename T = float>
class Matrix4x3
{
public:
    using Type = T;

    //The fourth row matrix;
    Type _Ma4x3[M4][M3];

    auto setI() -> void;

    #pragma region "Have question"
    auto identity() noexcept -> void;
    #pragma endregion //"Have question"

    auto zeroTranslation() noexcept -> void;

    auto setTranslation(const Vector3<Type>& d) noexcept -> void;
    auto setupTranslation(const Vector3<Type>& d) noexcept -> void;

    auto setupLocalToParent(const Vector3<Type>& pos, const EulerAngles<Type>& orient) -> void;
    auto setupLocalToParent(const Vector3<Type>& pos, const RotationMatrix<Type>& orient) -> void;
    auto setupParentToLocal(const Vector3<Type>& pos, const EulerAngles<Type>& orient) -> void;
    auto setupParentToLocal(const Vector3<Type>& pos, const RotationMatrix<Type>& orient) -> void;

    //Construct the matrix of rotation about the coordinate axis
    auto setupRotate(int axis, Type theta) -> void;
    //Construct matrices that rotate about any axist
    auto setupRotate(const Vector3<Type>& axis, Type theta) -> void;
    //The rotation matrix is constructed and the angular displacement is given by the quaternion form.
    auto fromQuaternion(const Quaternion<Type>& q) -> void;
    //Construct matrices that scale along the coordinate axes
    auto setupScale(const Vector3<Type>& s) -> void;
    //Construct matrices that scale along any axis
    auto setupScaleAlongAxis(const Vector3<Type>& axis, float k) -> void;

    //Construct the shear matrix
    auto setupShear(int axis, Type s, Type t) -> void;

    auto setupProject(const Vector3<Type>& n) -> void;
    // Construct the reflection matrix along any plane
    auto setupReflect(int axis, Type k = 0.0f) -> void;

    auto setupReflect(const Vector3<Type>& n) -> void;

};

template <typename T>
auto operator*(const Vector3<T>& p, const Matrix4x3<T>& m) -> Vector3<T>;

template <typename T>
auto operator*(const Matrix4x3<T>& a, const Matrix4x3<T>& b) -> Matrix4x3<T>;

template <typename T>
auto operator*=(Vector3<T>& p, const Matrix4x3<T>& m) -> Vector3<T>&;

template <typename T>
auto operator*= (const Matrix4x3<T>& a, const Matrix4x3<T>& m) -> Matrix4x3<T>&;

template <typename T>
auto determinant(const Matrix4x3<T>& m) -> T;

template <typename T>
auto inverse(const Matrix4x3<T>& m) -> Matrix4x3<T>;

template <typename T>
auto getTranslation(const Matrix4x3<T>& m) -> Vector3<T>;

template <typename T>
auto getPositionFromParentToLocalMatrix(const Matrix4x3<T>& m) -> Vector3<T>;

template <typename T>
auto getPositionFromLocalToParentMatrix(const Matrix4x3<T>& m) -> Vector3<T>;


}

#endif