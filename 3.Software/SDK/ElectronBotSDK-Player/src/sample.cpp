#include "electron_player.h"


int main()
{
    ElectronPlayer robot;

    if (robot.Connect())
        printf("Robot connected!\n");
    else
    {
        printf("Connect failed!\n");
        getchar();
        return 0;
    }

    robot.SetPose(ElectronPlayer::RobotPose_t{0, 30, 10, 0, 15, 0});
    robot.Play("video.mp4");

    robot.Disconnect();
    printf("Robot Disconnected.\n");

    getchar();
    return 0;
}

