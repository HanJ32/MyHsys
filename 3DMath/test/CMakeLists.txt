cmake_minimum_required(VERSION 3.5 FATAL_ERROR)
project(HsysMathat
VERSION 0.0.1 
LANGUAGES CXX)
message(STATUS "Build HsysMathat version ===TEST=== --0.0.1")


find_package(GTest REQUIRED)
if(GTest)
include(GoogleTest)
include_directories(${GTEST_INCLUDE_DIRS})
add_executable(_3DMATH_TEST testRotationMatrix.hpp)

target_link_libraries(test_my_class
    ${GTEST_BOTH_LIBRARIES} pthread
)

gtest_discover_tests(_3DMATH_TEST)

else()
include(FetchContent)
FetchContent_Declare(
    googletest
    URL https://github.com/google/googletest/archive/609281088cfefc76f9d0ce82e1ff6c30cc3591e5.zip
)
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

target_link_libraries(test_my_class
    ${GTEST_BOTH_LIBRARIES} pthread
)

gtest_discover_tests(_3DMATH_TEST)
endif()

include_directories(${CMAKE_SOURCE_DIR}/source)

enable_testing()

set(_3DMATH_TEST_SRC


)


# set{include_test_cxx
# testRotationMatrix.hpp
# }

# add_library(HsysMathatMathUtilTest ${include_test_cxx})