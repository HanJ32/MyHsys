cmake_minimum_required(VERSION 3.5 FATAL_ERROR)
project(HsysMathat
VERSION 0.0.1 
LANGUAGES CXX)
message(STATUS "Build HsysMathat version --0.0.1")

set(include_source_cxx 
MathUtil.cpp
EulerAngles.cpp
Quaternion.cpp
RotationMatrix.cpp
Matrix4x3.cpp
)

add_library(HsysMathatMathUtil ${include_source_cxx})