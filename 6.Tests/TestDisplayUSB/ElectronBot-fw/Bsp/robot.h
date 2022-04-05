#ifndef ELECTRONBOT_FW_ROBOT_H
#define ELECTRONBOT_FW_ROBOT_H

#include "stm32f4xx.h"
#include "screen.h"
#include "i2c.h"


#define ANY 0

class Robot
{
public:
    Robot(SPI_HandleTypeDef* _screenSpi, I2C_HandleTypeDef* _motorI2c) :
        screenSpi(_screenSpi), motorI2c(_motorI2c)
    {
        lcd = new Screen(screenSpi);

        /********* Need to adjust parameters for specific hardware *********/
        joint[ANY] = JointStatus_t{
            0,
            -180,
            180,
            90
        };

        joint[1] = JointStatus_t{ // Head
            2,
            70,
            95,
            0,
            -15,
            15,
            true
            // electron.SetJointId(electron.joint[ANY], 2);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 30);
            // electron.SetJointKi(electron.joint[ANY], 0.4);
            // electron.SetJointKd(electron.joint[ANY], 200);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 0.5);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };

        joint[2] = JointStatus_t{ // Left arm roll
            4,
            -9,
            3,
            0,
            0,
            30,
            false
            // electron.SetJointId(electron.joint[ANY], 4);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 50);
            // electron.SetJointKi(electron.joint[ANY], 0.8);
            // electron.SetJointKd(electron.joint[ANY], 600);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 1);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };

        joint[3] = JointStatus_t{ // Left arm pitch
            6,
            -16,
            117,
            0,
            -20,
            180,
            false
            // electron.SetJointId(electron.joint[ANY],6);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 50);
            // electron.SetJointKi(electron.joint[ANY], 0.8);
            // electron.SetJointKd(electron.joint[ANY], 300);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 0.5);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };

        joint[4] = JointStatus_t{ // Right arm roll
            8,
            133,
            141,
            0,
            0,
            30,
            true
            // electron.SetJointId(electron.joint[ANY], 8);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 50);
            // electron.SetJointKi(electron.joint[ANY], 0.8);
            // electron.SetJointKd(electron.joint[ANY], 600);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 1);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };

        joint[5] = JointStatus_t{ // Right arm pitch
            10,
            15,
            150,
            0,
            -20,
            180,
            true
            // electron.SetJointId(electron.joint[ANY],10);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 50);
            // electron.SetJointKi(electron.joint[ANY], 0.8);
            // electron.SetJointKd(electron.joint[ANY], 300);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 0.5);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };

        joint[6] = JointStatus_t{ // Body
            12,
            0,
            180,
            0,
            -90,
            90,
            false
            // electron.SetJointId(electron.joint[ANY],12);
            // electron.SetJointInitAngle(electron.joint[ANY], 0);
            // electron.SetJointKp(electron.joint[ANY], 150);
            // electron.SetJointKi(electron.joint[ANY], 0.8);
            // electron.SetJointKd(electron.joint[ANY], 300);
            // electron.SetJointTorqueLimit(electron.joint[ANY], 0.5);
            // electron.SetJointEnable(electron.joint[ANY], true);
        };
        /********* Need to adjust parameters for specific hardware *********/
    }


    struct UsbBuffer_t
    {
        uint8_t extraDataTx[32];
        uint8_t rxData[2][60 * 240 * 3 + 32]; // 43232bytes, 43200 of which are lcd buffer
        volatile uint16_t receivedPacketLen = 0;
        volatile uint8_t pingPongIndex = 0;
        volatile uint32_t rxDataOffset = 0;
    };
    UsbBuffer_t usbBuffer;


    struct JointStatus_t
    {
        uint8_t id;
        float angleMin;
        float angleMax;
        float angle;
        float modelAngelMin;
        float modelAngelMax;
        bool inverted = false;
    };
    JointStatus_t joint[7];


    uint8_t* GetPingPongBufferPtr();
    uint8_t* GetLcdBufferPtr();
    uint8_t* GetExtraDataRxPtr();
    void SwitchPingPongBuffer();
    void SendUsbPacket(uint8_t* _data, uint32_t _len);
    void ReceiveUsbPacketUntilSizeIs(uint32_t _count);
    void SetJointId(JointStatus_t &_joint, uint8_t _id);
    void SetJointKp(JointStatus_t &_joint, float _value);
    void SetJointKi(JointStatus_t &_joint, float _value);
    void SetJointKv(JointStatus_t &_joint, float _value);
    void SetJointKd(JointStatus_t &_joint, float _value);
    void SetJointEnable(JointStatus_t &_joint, bool _enable);
    void SetJointInitAngle(JointStatus_t &_joint, float _angle);
    void SetJointTorqueLimit(JointStatus_t &_joint, float _percent);

    void UpdateServoAngle(JointStatus_t &_joint);
    void UpdateServoAngle(JointStatus_t &_joint, float _angleSetPoint);
    void UpdateJointAngle(JointStatus_t &_joint);
    void UpdateJointAngle(JointStatus_t &_joint, float _angleSetPoint);

    Screen* lcd;


private:
    SPI_HandleTypeDef* screenSpi;
    I2C_HandleTypeDef* motorI2c;

    uint8_t i2cRxData[8];
    uint8_t i2cTxData[8];
    uint8_t usbExtraData[32];

    void TransmitAndReceiveI2cPacket(uint8_t _id);
};

#endif //ELECTRONBOT_FW_ROBOT_H
