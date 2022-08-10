#ifndef ELECTRONBOTSDK_ELECTRONLOWLEVEL_H
#define ELECTRONBOTSDK_ELECTRONLOWLEVEL_H

#include <iostream>
#include <cstdio>
#include <thread>
#include <opencv2/opencv.hpp>

extern "C" {
__declspec(dllexport) void *AHK_New();
__declspec(dllexport) void AHK_Delete(void *electronLowLevel);
__declspec(dllexport) bool AHK_Connect(void *electronLowLevel);
__declspec(dllexport) bool AHK_Disconnect(void *electronLowLevel);
__declspec(dllexport) bool AHK_Sync(void *electronLowLevel);
__declspec(dllexport) void AHK_SetImageSrc_MatData(void *electronLowLevel, void* image_data);
__declspec(dllexport) void AHK_SetImageSrc_Path(void *electronLowLevel, void *filepath);
__declspec(dllexport) void AHK_SetExtraData(void *electronLowLevel, uint8_t* _data, uint32_t _len);
__declspec(dllexport) uint8_t* AHK_GetExtraData(void *electronLowLevel, uint8_t* _data);
__declspec(dllexport) void AHK_GetJointAngles(void *electronLowLevel, float* _jointAngles);
__declspec(dllexport) void AHK_SetJointAngles(void *electronLowLevel, float _j1, float _j2, float _j3, float _j4, float _j5, float _j6, bool _enable);
}

class __declspec(dllexport) ElectronLowLevel
{
public:
    ElectronLowLevel()
    = default;

    ElectronLowLevel(int _vid, int _pid) :
        USB_PID(_pid), USB_VID(_vid)
    {}

    bool Connect();
    bool Disconnect();
    bool Sync();
    void SetImageSrc(const cv::Mat &_mat);
    void SetImageSrc(const std::string &_filePath);
    void SetImageSrc_MatData(void* image_data);
    void SetExtraData(uint8_t* _data, uint32_t _len = 32);
    void SetJointAngles(float _j1, float _j2, float _j3, float _j4, float _j5, float _j6,
                        bool _enable = false);
    void GetJointAngles(float* _jointAngles);
    uint8_t* GetExtraData(uint8_t* _data = nullptr);

    int USB_VID = 0x1001;
    int USB_PID = 0x8023;
    bool isConnected = false;
    uint32_t timeStamp = 0;


private:
    uint8_t pingPongWriteIndex = 0;
    uint8_t usbBuffer200[200]{};
    uint8_t frameBufferTx[2][240 * 240 * 3]{};
    uint8_t extraDataBufferTx[2][32]{};
    uint8_t extraDataBufferRx[32]{};
    std::thread syncTaskHandle;

    static bool ReceivePacket(uint8_t* _buffer, uint32_t _packetCount, uint32_t _packetSize);
    static bool TransmitPacket(uint8_t* _buffer, uint32_t _packetCount, uint32_t _packetSize);
    static void SyncTask(ElectronLowLevel* _obj);
};


#endif