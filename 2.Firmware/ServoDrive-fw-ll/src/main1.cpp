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
    LL_TIM_EnableCounter(TIM3);
    LL_TIM_CC_EnableChannel(TIM3,LL_TIM_CHANNEL_CH1);
    LL_TIM_CC_EnableChannel(TIM3,LL_TIM_CHANNEL_CH2);
    LL_TIM_OC_SetCompareCH1(TIM3,0);
    LL_TIM_OC_SetCompareCH2(TIM3,0);

    // Start receive data
    MY_I2C1_Init(boardConfig.nodeId);
    LL_mDelay(10);
    // Start control loop at 200Hz
    LL_TIM_EnableIT_UPDATE(TIM14);
    LL_TIM_EnableCounter(TIM14);


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
    /* for debug */
       //LL_GPIO_TogglePin(GPIOA,GPIO_PIN_1);
    }
}


/* Callbacks -------------------------------------------------------*/
// void HAL_ADC_ConvCpltCallback(ADC_HandleTypeDef* AdcHandle)
// {

// }


// // Command handler
void I2C_SlaveDMARxCpltCallback()
{
    ErrorStatus state;

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
       state = Slave_Transmit(i2cDataTx,5,5000);
    } while (state != SUCCESS);
    if(i2cDataRx[0] == 0x21)
    {
        Set_ID(boardConfig.nodeId);
    }

}


// Control loop
void TIM14_PeriodElapsedCallback()
{
        // Read sensor data
    LL_DMA_EnableChannel(DMA1,LL_DMA_CHANNEL_1);  
    LL_ADC_REG_StartConversion(ADC1);
    
    motor.angle = motor.mechanicalAngleMin +
                    (motor.mechanicalAngleMax - motor.mechanicalAngleMin) *
                    ((float) adcData[0] - (float) motor.adcValAtAngleMin) /
                    ((float) motor.adcValAtAngleMax - (float) motor.adcValAtAngleMin);
    // Calculate PID
    motor.CalcDceOutput(motor.angle, 0);
    motor.SetPwm((int16_t) motor.dce.output);
}