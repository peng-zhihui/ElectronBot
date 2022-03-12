#ifndef CPPTOUNITY_LIBRARY_H
#define CPPTOUNITY_LIBRARY_H

#include <Windows.h>
#include <sdkddkver.h>
#include <iostream>
#include "electron_low_level.h"

using namespace std;

#define DLL_API extern "C" _declspec(dllexport)

DLL_API
void Native_OnKeyFrameChange(const char* _filePath);

DLL_API
float* Native_OnFixUpdate(unsigned char* _imgDataEmoji, unsigned char* _imgDataCamera,
                          int _width, int _height, float* _setJoints, bool _enable);

DLL_API
void Native_OnInit();

DLL_API
void Native_OnExit();

#endif
