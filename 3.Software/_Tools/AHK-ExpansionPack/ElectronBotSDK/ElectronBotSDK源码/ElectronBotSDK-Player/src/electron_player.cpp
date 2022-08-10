#include "electron_player.h"
#include <opencv2/opencv.hpp>

//改用opencv_world455.dll发布版本做依赖，使用 Release - x64 编译
void *AHK_New(){
    return new ElectronPlayer();
}

void AHK_Delete(void *electronPlayer){
    delete static_cast<ElectronPlayer *>(electronPlayer);
}

bool AHK_Connect(void *electronPlayer){
    return static_cast<ElectronPlayer *>(electronPlayer)->Connect();
}

bool AHK_Disconnect(void *electronPlayer){
    return static_cast<ElectronPlayer *>(electronPlayer)->Disconnect();
}

//void ElectronPlayer::Play(const std::string &_filePath)
void AHK_Play(void *p, char *filepath){
    static_cast<ElectronPlayer *>(p)->Play(std::string(filepath));
}

//void ElectronPlayer::Stop()
void AHK_Stop(void *electronPlayer){
    static_cast<ElectronPlayer *>(electronPlayer)->Stop();
}

//ElectronPlayer::RobotPose_t ElectronPlayer::GetPose()
void* AHK_GetPose(){
    return new ElectronPlayer::RobotPose_t();
}

//void ElectronPlayer::SetPlaySpeed(float _ratio)
void AHK_SetPlaySpeed(void *electronPlayer, float _ratio){
    static_cast<ElectronPlayer *>(electronPlayer)->SetPlaySpeed(_ratio);
}


bool ElectronPlayer::Connect()
{
    isConnected = lowLevelHandle->Connect();
    return isConnected;
}


bool ElectronPlayer::Disconnect()
{
    if (playTaskHandle.joinable())
        playTaskHandle.join();
    return lowLevelHandle->Disconnect();
}


void ElectronPlayer::Play(const std::string &_filePath)
{
    // Picture type
    if (_filePath.find(".jpg") != std::string::npos ||
        _filePath.find(".png") != std::string::npos ||
        _filePath.find(".bmp") != std::string::npos)
    {
        lowLevelHandle->SetImageSrc(cv::imread(_filePath));
        lowLevelHandle->Sync();
    }
        // Video type
    else if (_filePath.find(".mp4") != std::string::npos ||
             _filePath.find(".mov") != std::string::npos)
    {
        if (isConnected)
        {
            isPlaying = true;
            playTaskHandle = std::thread(PlayTask, this, _filePath, playSpeedRatio);
        }
    }
}


void ElectronPlayer::Play(const std::string &_filePath, float _speedRatio)
{
    playSpeedRatio = _speedRatio;
    Play(_filePath);
}


void ElectronPlayer::Stop()
{
    isPlaying = false;
}


void ElectronPlayer::SetPose(const ElectronPlayer::RobotPose_t &_pose)
{

}


ElectronPlayer::RobotPose_t ElectronPlayer::GetPose()
{
    return ElectronPlayer::RobotPose_t();
}


void ElectronPlayer::SetPlaySpeed(float _ratio)
{
    if (_ratio > 0)
    {
        playSpeedRatio = _ratio;
    }
}


void ElectronPlayer::PlayTask(ElectronPlayer* _obj, const std::string &_filePath, float _speedRatio)
{
    cv::VideoCapture video(_filePath);
    cv::Mat frame;

    //CAP_PROP_FRAME_COUNT = 7
    auto totalFrameCount = video.get(7);
    long index = 1;

    while (_obj->isPlaying && index < totalFrameCount)
    {
        video >> frame;
        index += (long) _speedRatio;
        //CAP_PROP_POS_FRAMES = 1
        video.set(1, index);

        _obj->lowLevelHandle->SetImageSrc(frame);
        _obj->lowLevelHandle->Sync();
    }

    _obj->isPlaying = false;
}

