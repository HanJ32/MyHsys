cmake_minimum_required(VERSION 3.5 FATAL_ERROR)
# set(CMAKE_CXX_COMPILER g++)

if(${CMAKE_COMPILER_IS_GNUCXX})
message(STATUS "========== BUILD CONFIG ========")
set(CMAKE_CXX_STANDARD 20)

message(STATUS "========== BUILD begin =========")
include_directories(${CMAKE_SOURCE_DIR}/include)
add_subdirectory(${CMAKE_SOURCE_DIR}/source)

message(STATUS "========== BUILD HsysMathat =========")
add_executable(HsysMathat main.cpp)
target_link_libraries(HsysMathat HsysMathatMathUtil)

message(STATUS "========== BUILD HsysMathat TEST =========")
add_subdirectory(${CMAKE_SOURCE_DIR}/test)

else()
message(WARNING "WARNING: Using GUNCXX(like g++ or clang++)")
endif()