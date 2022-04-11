#ifndef CONFIGURATIONS_H
#define CONFIGURATIONS_H

#ifdef __cplusplus
extern "C" {
#endif
/*---------------------------- C Scope ---------------------------*/
#include <stdbool.h>
#include "stdint-gcc.h"


typedef enum configStatus_t
{
    CONFIG_RESTORE = 0,
    CONFIG_OK,
    CONFIG_COMMIT
} configStatus_t;


typedef struct Config_t
{
    configStatus_t configStatus;
    uint8_t nodeId;
    float initPos;
    float toqueLimit;
    float velocityLimit;
    uint16_t adcValAtAngleMin;
    uint16_t adcValAtAngleMax;
    float mechanicalAngleMin;
    float mechanicalAngleMax;
    float dceKp;
    float dceKv;
    float dceKi;
    float dceKd;
    bool enableMotorOnBoot;
} BoardConfig_t;

extern BoardConfig_t boardConfig;


#ifdef __cplusplus
}
/*---------------------------- C++ Scope ---------------------------*/

#include "random_flash_interface.h"
#include "motor.h"


#endif
#endif
