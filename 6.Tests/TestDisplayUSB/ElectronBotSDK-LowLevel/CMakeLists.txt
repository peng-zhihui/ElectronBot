cmake_minimum_required(VERSION 3.21)
project(ElectronBotSDK-LowLevel)

set(CMAKE_CXX_STANDARD 14)

include_directories(
        src
        lib
        D:\\Libraries\\Cpp\\OpenCV_lib\\include
        D:\\Libraries\\Cpp\\Boost_lib\\include
)

file(GLOB_RECURSE SOURCES
        src/electron_low_level.cpp
        )

link_directories(
        lib
        D:\\Libraries\\Cpp\\OpenCV_lib\\x64\\vc16\\lib
        D:\\Libraries\\Cpp\\Boost_lib\\lib64-msvc-14.1
)

add_library(${PROJECT_NAME} SHARED
        "src/electron_low_level.cpp"
        )

add_executable(Sample
        "src/sample.cpp"
        )

target_link_libraries(${PROJECT_NAME}
        USBInterface
        opencv_core348d.lib
        opencv_imgcodecs348d.lib
        opencv_imgproc348d.lib
        opencv_videoio348d.lib
        )

target_link_libraries(Sample
        ${PROJECT_NAME}
        opencv_core348d.lib
        opencv_imgcodecs348d.lib
        opencv_imgproc348d.lib
        opencv_videoio348d.lib
        )