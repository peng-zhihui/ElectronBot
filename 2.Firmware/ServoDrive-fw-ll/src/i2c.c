/**
  ******************************************************************************
  * @file    i2c.c
  * @brief   This file provides code for the configuration
  *          of the I2C instances.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2022 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */

/* Includes ------------------------------------------------------------------*/
#include "i2c.h"

/* USER CODE BEGIN 0 */
uint8_t i2cDataRx[8];
uint8_t i2cDataTx[8];

/* USER CODE END 0 */

/* USER CODE BEGIN 1 */
void MY_I2C1_Init(uint32_t _id)
{

  /* USER CODE BEGIN I2C1_Init 0 */

  /* USER CODE END I2C1_Init 0 */

  LL_I2C_InitTypeDef I2C_InitStruct = {0};

  LL_GPIO_InitTypeDef GPIO_InitStruct = {0};

  LL_AHB1_GRP1_EnableClock(LL_AHB1_GRP1_PERIPH_GPIOA);
  /**I2C1 GPIO Configuration
  PA9   ------> I2C1_SCL
  PA10   ------> I2C1_SDA
  */
  GPIO_InitStruct.Pin = LL_GPIO_PIN_9;
  GPIO_InitStruct.Mode = LL_GPIO_MODE_ALTERNATE;
  GPIO_InitStruct.Speed = LL_GPIO_SPEED_FREQ_HIGH;
  GPIO_InitStruct.OutputType = LL_GPIO_OUTPUT_OPENDRAIN;
  GPIO_InitStruct.Pull = LL_GPIO_PULL_UP;
  GPIO_InitStruct.Alternate = LL_GPIO_AF_4;
  LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  GPIO_InitStruct.Pin = LL_GPIO_PIN_10;
  GPIO_InitStruct.Mode = LL_GPIO_MODE_ALTERNATE;
  GPIO_InitStruct.Speed = LL_GPIO_SPEED_FREQ_HIGH;
  GPIO_InitStruct.OutputType = LL_GPIO_OUTPUT_OPENDRAIN;
  GPIO_InitStruct.Pull = LL_GPIO_PULL_UP;
  GPIO_InitStruct.Alternate = LL_GPIO_AF_4;
  LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  LL_DMA_SetDataTransferDirection(DMA1, LL_DMA_CHANNEL_3, LL_DMA_DIRECTION_PERIPH_TO_MEMORY);

  LL_DMA_SetChannelPriorityLevel(DMA1, LL_DMA_CHANNEL_3, LL_DMA_PRIORITY_HIGH);

  LL_DMA_SetMode(DMA1, LL_DMA_CHANNEL_3, LL_DMA_MODE_NORMAL);

  LL_DMA_SetPeriphIncMode(DMA1, LL_DMA_CHANNEL_3, LL_DMA_PERIPH_NOINCREMENT);

  LL_DMA_SetMemoryIncMode(DMA1, LL_DMA_CHANNEL_3, LL_DMA_MEMORY_INCREMENT);

  LL_DMA_SetPeriphSize(DMA1, LL_DMA_CHANNEL_3, LL_DMA_PDATAALIGN_BYTE);

  LL_DMA_SetMemorySize(DMA1, LL_DMA_CHANNEL_3, LL_DMA_MDATAALIGN_BYTE);

  LL_DMA_SetDataLength(DMA1,LL_DMA_CHANNEL_3,5);
  LL_DMA_SetMemoryAddress(DMA1,LL_DMA_CHANNEL_3,(uint32_t)i2cDataRx);
  LL_DMA_SetPeriphAddress(DMA1,LL_DMA_CHANNEL_3,LL_I2C_DMA_GetRegAddr(I2C1,LL_I2C_DMA_REG_DATA_RECEIVE));  
  LL_DMA_EnableIT_TC(DMA1,LL_DMA_CHANNEL_3);
  /* Peripheral clock enable */
  LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_I2C1);

  /* I2C1 interrupt Init */
  NVIC_SetPriority(I2C1_IRQn, 0);
  NVIC_EnableIRQ(I2C1_IRQn);

  /* USER CODE BEGIN I2C1_Init 1 */
  
  /* USER CODE END I2C1_Init 1 */
  /** I2C Initialization
  */
  
  LL_I2C_DisableGeneralCall(I2C1);
  LL_I2C_EnableClockStretching(I2C1);
  I2C_InitStruct.PeripheralMode = LL_I2C_MODE_I2C;
  I2C_InitStruct.Timing = 0x2000090E;
  I2C_InitStruct.AnalogFilter = LL_I2C_ANALOGFILTER_ENABLE;
  I2C_InitStruct.DigitalFilter = 0;
  I2C_InitStruct.OwnAddress1 = _id;
  I2C_InitStruct.TypeAcknowledge = LL_I2C_ACK;
  I2C_InitStruct.OwnAddrSize = LL_I2C_OWNADDRESS1_7BIT;
  LL_I2C_Init(I2C1, &I2C_InitStruct);
  LL_I2C_EnableAutoEndMode(I2C1);
  LL_I2C_SetOwnAddress2(I2C1, 0, LL_I2C_OWNADDRESS2_NOMASK);
  LL_I2C_EnableOwnAddress2(I2C1);
    /* USER CODE BEGIN I2C1_Init 2 */
    LL_I2C_Enable(I2C1);
    LL_I2C_EnableIT_ADDR(I2C1);
    LL_I2C_EnableDMAReq_RX(I2C1);
    /* USER CODE END I2C1_Init 2 */
}

void Set_ID(uint8_t _id)
{
    LL_I2C_DisableOwnAddress1(I2C1);
    LL_I2C_SetOwnAddress1(I2C1,_id,LL_I2C_OWNADDRESS1_7BIT);
    LL_I2C_EnableOwnAddress1(I2C1);
}

ErrorStatus Slave_Transmit(uint8_t *pdata,uint16_t size,uint32_t timeout)
{
    uint32_t Timeout = timeout;
    while(!LL_I2C_IsActiveFlag_ADDR(I2C1))
    {
    /* Check Systick counter flag to decrement the time-out value */
        if (LL_SYSTICK_IsActiveCounterFlag()) 
        {
            if(Timeout-- == 0)
            {
            /* Time-out occurred. return error */
            return ERROR;
            }
        }
    }
  /*Clear ADDR flag and loop until end of transfer*/
  /* Clear ADDR flag value in ISR register */
    LL_I2C_ClearFlag_ADDR(I2C1);  
  
   if(LL_I2C_GetTransferDirection(I2C1) == LL_I2C_DIRECTION_READ)
   {
       /* Loop until TXE flag is raised  */
        while(size > 0)
        {
            /* Transmit data (TXE flag raised) **********************************/
            /* Check TXE flag value in ISR register */
            if(LL_I2C_IsActiveFlag_TXE(I2C1))
            {
            /* Write data in Transmit Data register.
                TXE flag is cleared by writing data in TXDR register */
                LL_I2C_TransmitData8(I2C1, (*pdata++));
                size--;
                Timeout = timeout;
            }
            /* Check Systick counter flag to decrement the time-out value */
            if (LL_SYSTICK_IsActiveCounterFlag()) 
            {
                if(Timeout-- == 0)
                {
                    /* Time-out occurred. return error */
                    return ERROR;
                }
            }
        } 
   }
   return SUCCESS;  
}
/* USER CODE END 1 */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
