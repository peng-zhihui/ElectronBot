#include "electron_low_level.h"
#include "USBInterface.h"
#include "opencv2/imgproc/types_c.h"

//改用opencv_world455.dll发布版本做依赖，使用 Release - x64 编译
void *AHK_New(){
    return new ElectronLowLevel();
}

void AHK_Delete(void *electronLowLevel){
    delete static_cast<ElectronLowLevel *>(electronLowLevel);
}

bool AHK_Connect(void *electronLowLevel){
    return static_cast<ElectronLowLevel *>(electronLowLevel)->Connect();
}

bool AHK_Disconnect(void *electronLowLevel){
    return static_cast<ElectronLowLevel *>(electronLowLevel)->Disconnect();
}

bool AHK_Sync(void *electronLowLevel){
    return static_cast<ElectronLowLevel *>(electronLowLevel)->Sync();
}

//void ElectronLowLevel::SetImageSrc(const cv::Mat &_mat) 方便适配AHK，新增了一个修改函数
void AHK_SetImageSrc_MatData(void *electronLowLevel, void* image_data){
    static_cast<ElectronLowLevel *>(electronLowLevel)->SetImageSrc_MatData(image_data);
}

//void ElectronLowLevel::SetImageSrc(const string &_filePath)
void AHK_SetImageSrc_Path(void *electronLowLevel, void *filepath){
    static_cast<ElectronLowLevel *>(electronLowLevel)->SetImageSrc(string((char*)filepath));
}

//void ElectronLowLevel::SetExtraData(uint8_t* _data, uint32_t _len)
void AHK_SetExtraData(void *electronLowLevel, uint8_t* _data, uint32_t _len){
    static_cast<ElectronLowLevel *>(electronLowLevel)->SetExtraData(_data, _len);
}

//uint8_t* ElectronLowLevel::GetExtraData(uint8_t* _data)
uint8_t* AHK_GetExtraData(void *electronLowLevel, uint8_t* _data){
    return static_cast<ElectronLowLevel *>(electronLowLevel)->GetExtraData(_data);
}

//void ElectronLowLevel::GetJointAngles(float* _jointAngles)
void AHK_GetJointAngles(void *electronLowLevel, float* _jointAngles){
   static_cast<ElectronLowLevel *>(electronLowLevel)->GetJointAngles(_jointAngles);
}

//void ElectronLowLevel::SetJointAngles(float _j1, float _j2, float _j3, float _j4, float _j5, float _j6, bool _enable)
void AHK_SetJointAngles(void *electronLowLevel, float _j1, float _j2, float _j3, float _j4, float _j5, float _j6, bool _enable){
    static_cast<ElectronLowLevel *>(electronLowLevel)->SetJointAngles(_j1, _j2, _j3, _j4, _j5, _j6, _enable);
}

//新增函数只接收已裁剪并转换好颜色的MatData
void ElectronLowLevel::SetImageSrc_MatData(void* image_data)
{
    std::memcpy(frameBufferTx[pingPongWriteIndex], image_data, 240 * 240 * 3);
}


bool ElectronLowLevel::Sync()
{
    if (isConnected)
    {
        if (syncTaskHandle.joinable())
            syncTaskHandle.join();

        syncTaskHandle = std::thread(SyncTask, this);

        timeStamp++;
        return true;
    }

    return false;
}


void ElectronLowLevel::SetImageSrc(const cv::Mat &_mat)
{
    cv::Mat temp;
    resize(_mat, temp, cv::Size(240, 240));
    cvtColor(temp, temp, CV_BGRA2RGB);
    std::memcpy(frameBufferTx[pingPongWriteIndex], temp.data, 240 * 240 * 3);
}


void ElectronLowLevel::SetImageSrc(const string &_filePath)
{
    cv::Mat temp = cv::imread(_filePath);
    resize(temp, temp, cv::Size(240, 240));
    cvtColor(temp, temp, CV_BGRA2RGB);
    std::memcpy(frameBufferTx[pingPongWriteIndex], temp.data, 240 * 240 * 3);
}


void ElectronLowLevel::SetExtraData(uint8_t* _data, uint32_t _len)
{
    if (_len <= 32)
        memcpy(extraDataBufferTx[pingPongWriteIndex], _data, _len);
}


uint8_t* ElectronLowLevel::GetExtraData(uint8_t* _data)
{
    if (_data != nullptr)
        memcpy(_data, extraDataBufferRx, 32);

    return extraDataBufferRx;
}


bool ElectronLowLevel::ReceivePacket(uint8_t* _buffer, uint32_t _packetCount, uint32_t _packetSize)
{
    uint32_t packetCount = _packetCount;
    uint32_t ret;
    do
    {
        do
        {
            ret = USB_BulkReceive(0, EP1_IN, reinterpret_cast<char*>(_buffer), _packetSize, 100);
        } while (ret != _packetSize);

        packetCount--;
    } while (packetCount > 0);

    return packetCount == 0;
}


bool ElectronLowLevel::TransmitPacket(uint8_t* _buffer, uint32_t _packetCount, uint32_t _packetSize)
{
    uint32_t packetCount = _packetCount;
    uint32_t dataOffset = 0;
    uint32_t ret;
    do
    {
        do
        {
            ret = USB_BulkTransmit(0, EP1_OUT,
                                   reinterpret_cast<char*>(_buffer) + dataOffset,
                                   _packetSize, 100);
        } while (!ret);

        dataOffset += _packetSize;
        packetCount--;
    } while (packetCount > 0);

    return packetCount == 0;
}


bool ElectronLowLevel::Connect()
{
    int devNum = USB_ScanDevice(USB_PID, USB_VID);

    if (devNum > 0)
    {
        if (USB_OpenDevice(0))
        {
            isConnected = true;
            timeStamp = 0;

            return true;
        }
    }

    return false;
}


bool ElectronLowLevel::Disconnect()
{
    if (syncTaskHandle.joinable())
        syncTaskHandle.join();

    if (isConnected && USB_CloseDevice(0))
    {
        isConnected = false;
        return true;
    }

    return false;
}


void ElectronLowLevel::SyncTask(ElectronLowLevel* _obj)
{
    uint32_t frameBufferOffset = 0;
    uint8_t index = _obj->pingPongWriteIndex;
    _obj->pingPongWriteIndex = _obj->pingPongWriteIndex == 0 ? 1 : 0;
    for (int p = 0; p < 4; p++)
    {
        // Wait MCU request & receive 32bytes extra data
        _obj->ReceivePacket(reinterpret_cast<uint8_t*>(_obj->extraDataBufferRx),
                            1, 32);

        // Transmit buffer
        _obj->TransmitPacket(reinterpret_cast<uint8_t*>(_obj->frameBufferTx[index]) + frameBufferOffset,
                             84, 512);
        frameBufferOffset += 43008;

        // Fill frame tail & extra data
        memcpy(_obj->usbBuffer200, reinterpret_cast<uint8_t*>(_obj->frameBufferTx[index]) + frameBufferOffset,
               192);
        memcpy(_obj->usbBuffer200 + 192, reinterpret_cast<uint8_t*>(_obj->extraDataBufferTx[index]), 32);

        // Transmit frame tail & extra data
        _obj->TransmitPacket(_obj->usbBuffer200, 1, 224);
        frameBufferOffset += 192;
    }
}

void ElectronLowLevel::SetJointAngles(float _j1, float _j2, float _j3, float _j4, float _j5, float _j6,
                                      bool _enable)
{
    float jointAngleSetPoints[6];

    jointAngleSetPoints[0] = _j1;
    jointAngleSetPoints[1] = _j2;
    jointAngleSetPoints[2] = _j3;
    jointAngleSetPoints[3] = _j4;
    jointAngleSetPoints[4] = _j5;
    jointAngleSetPoints[5] = _j6;

    extraDataBufferTx[pingPongWriteIndex][0] = _enable ? 1 : 0;
    for (int j = 0; j < 6; j++)
        for (int i = 0; i < 4; i++)
        {
            auto* b = (unsigned char*) &(jointAngleSetPoints[j]);
            extraDataBufferTx[pingPongWriteIndex][j * 4 + i + 1] = *(b + i);
        }
}


void ElectronLowLevel::GetJointAngles(float* _jointAngles)
{
    for (int j = 0; j < 6; j++)
    {
        _jointAngles[j] = *((float*) (extraDataBufferRx + 4 * j + 1));
    }
}

