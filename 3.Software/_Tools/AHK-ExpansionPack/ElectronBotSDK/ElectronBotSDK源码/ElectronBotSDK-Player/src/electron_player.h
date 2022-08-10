#ifndef ELECTRONBOTSDK_ELECTRONPLAYER_H
#define ELECTRONBOTSDK_ELECTRONPLAYER_H

#include <iostream>
#include <cstdio>
#include <thread>
#include "electron_low_level.h"

extern "C" {
__declspec(dllexport) void *AHK_New();
__declspec(dllexport) void AHK_Delete(void *ElectronPlayer);
__declspec(dllexport) bool AHK_Connect(void *ElectronPlayer);
__declspec(dllexport) bool AHK_Disconnect(void *ElectronPlayer);
__declspec(dllexport) void AHK_Play(void *p, char *filepath);
__declspec(dllexport) void AHK_Stop(void *ElectronPlayer);
__declspec(dllexport) void AHK_SetPlaySpeed(void *ElectronPlayer, float _ratio);
__declspec(dllexport) void* AHK_GetPose();
}

class __declspec(dllexport) ElectronPlayer
{
public:
    ElectronPlayer()
    {
        lowLevelHandle = new ElectronLowLevel();
    }

    ElectronPlayer(int _vid, int _pid) :
        USB_PID(_pid), USB_VID(_vid)
    {
        lowLevelHandle = new ElectronLowLevel(_vid, _pid);
    }

    ~ElectronPlayer()
    {
        delete (lowLevelHandle);
    }


    struct RobotPose_t
    {
        float j1;
        float j2;
        float j3;
        float j4;
        float j5;
        float j6;
    };
    RobotPose_t currentPose{0, 0, 0, 0, 0, 0};

    ElectronLowLevel* lowLevelHandle;

    bool Connect();
    bool Disconnect();
    void Play(const std::string &_filePath);
    void Play(const std::string &_filePath, float _speedRatio);
    void Stop();
    void SetPlaySpeed(float _ratio);
    void SetPose(const RobotPose_t &_pose);
    RobotPose_t GetPose();


    int USB_VID = 0x1001;
    int USB_PID = 0x8023;
    bool isConnected = false;


private:
    bool isPlaying = false;
    float playSpeedRatio = 1.0f;
    std::thread playTaskHandle;

    static void PlayTask(ElectronPlayer* _obj, const std::string &_filePath, float _speedRatio);
};


#endif