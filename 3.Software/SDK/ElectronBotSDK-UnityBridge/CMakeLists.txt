cmake_minimum_required(VERSION 3.20)
project(ElectronBotSDK-UnityBridge)

set(CMAKE_CXX_STANDARD 14)
set(CMAKE_C_STANDARD 11)

include_directories(
        src
        lib
        D:\\Libraries\\Cpp\\OpenCV_lib\\include
        D:\\Libraries\\Cpp\\Boost_lib\\include
        D:\\Libraries\\Cpp\\Boost_lib
)

link_directories(
        lib
        D:\\Libraries\\Cpp\\OpenCV_lib\\x64\\vc16\\lib
        D:\\Libraries\\Cpp\\Boost_lib\\lib64-msvc-14.1
)


add_library(${PROJECT_NAME} SHARED
        src/electron_sdk_unity_bridge.cpp)

target_link_libraries(${PROJECT_NAME}
        ElectronBotSDK-LowLevel;
        opencv_world348d.lib
        )
