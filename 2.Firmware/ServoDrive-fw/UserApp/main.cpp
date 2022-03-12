#include <cstring>
#include "common_inc.h"
#include "configurations.h"

Motor motor;
BoardConfig_t boardConfig;


/* Default Entry -------------------------------------------------------*/
void Main()
{
    // Read data from Flash
    EEPROM eeprom;
    eeprom.get(0, boardConfig);
    if (boardConfig.configStatus != CONFIG_OK) // use default settings
    {
        boardConfig = BoardConfig_t{
            .configStatus = CONFIG_OK,
            .nodeId = 12, // 7bit address, has to be even number
            .initPos = 90,
            .toqueLimit =  0.5,
            .velocityLimit=0,
            .adcValAtAngleMin=250,
            .adcValAtAngleMax=3000,
            .mechanicalAngleMin=0,
            .mechanicalAngleMax=180,
            .dceKp = 10,
            .dceKv = 0,
            .dceKi = 0,
            .dceKd = 50,
            .enableMotorOnBoot=false
        };
        eeprom.put(0, boardConfig);
    }
    motor.SetTorqueLimit(boardConfig.toqueLimit);
    motor.mechanicalAngleMin = boardConfig.mechanicalAngleMin;
    motor.mechanicalAngleMax = boardConfig.mechanicalAngleMax;
    motor.adcValAtAngleMin = boardConfig.adcValAtAngleMin;
    motor.adcValAtAngleMax = boardConfig.adcValAtAngleMax;
    motor.dce.kp = boardConfig.dceKp;
    motor.dce.ki = boardConfig.dceKi;
    motor.dce.kv = boardConfig.dceKv;
    motor.dce.kd = boardConfig.dceKd;
    motor.dce.setPointPos = boardConfig.initPos;
    motor.SetEnable(boardConfig.enableMotorOnBoot);

    // Init PWM
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_2);
    __HAL_TIM_SET_COMPARE(&htim3, TIM_CHANNEL_1, 0);
    __HAL_TIM_SET_COMPARE(&htim3, TIM_CHANNEL_2, 0);


    // Start receive data
    MY_I2C1_Init(boardConfig.nodeId);
    HAL_Delay(10);
    HAL_I2C_Slave_Receive_IT(&hi2c1, (uint8_t*) i2cDataRx, 5);

    // Start control loop at 200Hz
    HAL_TIM_Base_Start_IT(&htim14);


    while (1)
    {
        if (boardConfig.configStatus == CONFIG_COMMIT)
        {
            boardConfig.configStatus = CONFIG_OK;
            eeprom.put(0, boardConfig);
        } else if (boardConfig.configStatus == CONFIG_RESTORE)
        {
            eeprom.put(0, boardConfig);
            HAL_NVIC_SystemReset();
        }

        HAL_Delay(10);
    }
}


/* Callbacks -------------------------------------------------------*/
void HAL_ADC_ConvCpltCallback(ADC_HandleTypeDef* AdcHandle)
{

}


// Command handler
void HAL_I2C_SlaveRxCpltCallback(I2C_HandleTypeDef* hi2c)
{
    HAL_StatusTypeDef state = HAL_ERROR;

    float valF = *((float*) (i2cDataRx + 1));

    i2cDataTx[0] = i2cDataRx[0];
    switch (i2cDataRx[0])
    {
        case 0x01:  // Set angle
        {
            motor.dce.setPointPos = valF;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x02: // Set velocity
        {
            motor.dce.setPointVel = valF;
            auto* b = (unsigned char*) &(motor.velocity);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x03: // Set torque
        {
            motor.SetTorqueLimit(valF);
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x11: // Get angle
        {
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x12: // Get velocity
        {
            auto* b = (unsigned char*) &(motor.velocity);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x21: // Set id
        {
            boardConfig.nodeId = i2cDataRx[1];
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x22: // Set kp
        {
            motor.dce.kp = valF;
            boardConfig.dceKp = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x23: // Set ki
        {
            motor.dce.ki = valF;
            boardConfig.dceKi = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x24: // Set kv
        {
            motor.dce.kv = valF;
            boardConfig.dceKv = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x25: // Set kd
        {
            motor.dce.kd = valF;
            boardConfig.dceKd = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x26: // Set torque limit
        {
            motor.SetTorqueLimit(valF);
            boardConfig.toqueLimit = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0x27: // Set init pos
        {
            boardConfig.initPos = valF;
            boardConfig.configStatus = CONFIG_COMMIT;
            auto* b = (unsigned char*) &(motor.angle);
            for (int i = 0; i < 4; i++)
                i2cDataTx[i + 1] = *(b + i);
            break;
        }
        case 0xff:
            motor.SetEnable(i2cDataRx[1] != 0);
            break;
        default:
            break;
    }

    do
    {
        state = HAL_I2C_Slave_Transmit(&hi2c1, (uint8_t*) i2cDataTx, 5, 10000);
    } while (state != HAL_OK);

    do
    {
        state = HAL_I2C_Slave_Receive_IT(&hi2c1, (uint8_t*) i2cDataRx, 5);
    } while (state != HAL_OK);
}


// Control loop
void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef* htim)
{
    if (htim->Instance == TIM14)
    {
        // Read sensor data
        HAL_ADC_Start_DMA(&hadc, (uint32_t*) adcData, 1);

        motor.angle = motor.mechanicalAngleMin +
                      (motor.mechanicalAngleMax - motor.mechanicalAngleMin) *
                      ((float) adcData[0] - (float) motor.adcValAtAngleMin) /
                      ((float) motor.adcValAtAngleMax - (float) motor.adcValAtAngleMin);

        // Calculate PID
        motor.CalcDceOutput(motor.angle, 0);
        motor.SetPwm((int16_t) motor.dce.output);
    }
}