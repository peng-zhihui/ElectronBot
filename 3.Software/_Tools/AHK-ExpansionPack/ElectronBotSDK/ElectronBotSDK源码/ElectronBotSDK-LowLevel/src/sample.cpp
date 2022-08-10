#include "electron_low_level.h"

using namespace cv;


int main()
{
    ElectronLowLevel robot;

    if (robot.Connect())
        printf("Robot connected!\n");
    else
    {
        printf("Connect failed!\n");
        getchar();
        return 0;
    }

    VideoCapture video("video.mp4");
    Mat frame;
    uint8_t extraData[32];
    float jointAngles[6];
    float jointAngleSetPoints[6];

    int t = 10;

    while (t--)
    {
        video >> frame;
        if (frame.empty())
        {
            // video.set(CV_CAP_PROP_POS_FRAMES, 0);
            // continue;
            break;
        }

        robot.SetImageSrc(frame);


        robot.SetJointAngles(0, 0, 0, 0, 0, 0, false);
        robot.Sync();
        robot.GetJointAngles(jointAngles);

        printf("%f,%f,%f,%f,%f,%f\n",
               jointAngles[0], jointAngles[1], jointAngles[2],
               jointAngles[3], jointAngles[4], jointAngles[5]);
    }

    robot.Disconnect();
    printf("Robot Disconnected.\n");


    getchar();
    return 0;
}

