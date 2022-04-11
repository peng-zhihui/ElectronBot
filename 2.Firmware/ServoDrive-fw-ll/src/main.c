/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
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
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "adc.h"
#include "dma.h"
#include "tim.h"
#include "gpio.h"
/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include "common_inc.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */

  LL_APB1_GRP2_EnableClock(LL_APB1_GRP2_PERIPH_SYSCFG);
  LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_PWR);

  HAL_Init();

  /* System interrupt init*/

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_DMA_Init();
  MX_ADC_Init();
  MX_TIM3_Init();
  MX_TIM14_Init();
  /* USER CODE BEGIN 2 */
  Main();
  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  LL_FLASH_SetLatency(LL_FLASH_LATENCY_1);
  while(LL_FLASH_GetLatency() != LL_FLASH_LATENCY_1)
  {
  }
  LL_RCC_HSI_Enable();

   /* Wait till HSI is ready */
  while(LL_RCC_HSI_IsReady() != 1)
  {

  }
  LL_RCC_HSI_SetCalibTrimming(16);
  LL_RCC_HSI14_Enable();

   /* Wait till HSI14 is ready */
  while(LL_RCC_HSI14_IsReady() != 1)
  {

  }
  LL_RCC_HSI14_SetCalibTrimming(16);
  LL_RCC_HSI48_Enable();

   /* Wait till HSI48 is ready */
  while(LL_RCC_HSI48_IsReady() != 1)
  {

  }
  LL_RCC_SetAHBPrescaler(LL_RCC_SYSCLK_DIV_1);
  LL_RCC_SetAPB1Prescaler(LL_RCC_APB1_DIV_1);
  LL_RCC_SetSysClkSource(LL_RCC_SYS_CLKSOURCE_HSI48);

   /* Wait till System clock is ready */
  while(LL_RCC_GetSysClkSource() != LL_RCC_SYS_CLKSOURCE_STATUS_HSI48)
  {

  }
  LL_Init1msTick(48000000);
  LL_SetSystemCoreClock(48000000);
  LL_RCC_HSI14_EnableADCControl();
  LL_RCC_SetI2CClockSource(LL_RCC_I2C1_CLKSOURCE_HSI);
}

/**
  * @brief ADC Initialization Function
  * @param None
  * @retval None
  */
// static void MX_ADC_Init(void)
// {

//   /* USER CODE BEGIN ADC_Init 0 */

//   /* USER CODE END ADC_Init 0 */

//   LL_ADC_InitTypeDef ADC_InitStruct = {0};
//   LL_ADC_REG_InitTypeDef ADC_REG_InitStruct = {0};

//   LL_GPIO_InitTypeDef GPIO_InitStruct = {0};

//   /* Peripheral clock enable */
//   LL_APB1_GRP2_EnableClock(LL_APB1_GRP2_PERIPH_ADC1);

//   LL_AHB1_GRP1_EnableClock(LL_AHB1_GRP1_PERIPH_GPIOA);
//   /**ADC GPIO Configuration
//   PA4   ------> ADC_IN4
//   */
//   GPIO_InitStruct.Pin = LL_GPIO_PIN_4;
//   GPIO_InitStruct.Mode = LL_GPIO_MODE_ANALOG;
//   GPIO_InitStruct.Pull = LL_GPIO_PULL_NO;
//   LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

//   /* ADC DMA Init */

//   /* ADC Init */
//   LL_DMA_SetDataTransferDirection(DMA1, LL_DMA_CHANNEL_1, LL_DMA_DIRECTION_PERIPH_TO_MEMORY);

//   LL_DMA_SetChannelPriorityLevel(DMA1, LL_DMA_CHANNEL_1, LL_DMA_PRIORITY_LOW);

//   LL_DMA_SetMode(DMA1, LL_DMA_CHANNEL_1, LL_DMA_MODE_CIRCULAR);

//   LL_DMA_SetPeriphIncMode(DMA1, LL_DMA_CHANNEL_1, LL_DMA_PERIPH_NOINCREMENT);

//   LL_DMA_SetMemoryIncMode(DMA1, LL_DMA_CHANNEL_1, LL_DMA_MEMORY_INCREMENT);

//   LL_DMA_SetPeriphSize(DMA1, LL_DMA_CHANNEL_1, LL_DMA_PDATAALIGN_HALFWORD);

//   LL_DMA_SetMemorySize(DMA1, LL_DMA_CHANNEL_1, LL_DMA_MDATAALIGN_HALFWORD);

//     LL_DMA_SetDataLength(DMA1,LL_DMA_CHANNEL_1,1);
//     LL_DMA_SetMemoryAddress(DMA1,LL_DMA_CHANNEL_1,(uint32_t )&adcData);
//     LL_DMA_SetPeriphAddress(DMA1,LL_DMA_CHANNEL_1,LL_ADC_DMA_GetRegAddr(ADC1,LL_ADC_DMA_REG_REGULAR_DATA));
    
//     LL_DMA_EnableChannel(DMA1,LL_DMA_CHANNEL_1);

//   /* ADC interrupt Init */
// //   NVIC_SetPriority(ADC1_IRQn, 1);
// //   NVIC_EnableIRQ(ADC1_IRQn);

//   /* USER CODE BEGIN ADC_Init 1 */

//   /* USER CODE END ADC_Init 1 */
//   /** Configure Regular Channel
//   */
//   LL_ADC_REG_SetSequencerChAdd(ADC1, LL_ADC_CHANNEL_4);
//   /** Configure the global features of the ADC (Clock, Resolution, Data Alignment and number of conversion)
//   */
//   ADC_InitStruct.Clock = LL_ADC_CLOCK_ASYNC;
//   ADC_InitStruct.Resolution = LL_ADC_RESOLUTION_12B;
//   ADC_InitStruct.DataAlignment = LL_ADC_DATA_ALIGN_RIGHT;
//   ADC_InitStruct.LowPowerMode = LL_ADC_LP_MODE_NONE;
//   LL_ADC_Init(ADC1, &ADC_InitStruct);
//   ADC_REG_InitStruct.TriggerSource = LL_ADC_REG_TRIG_SOFTWARE;
//   ADC_REG_InitStruct.SequencerDiscont = LL_ADC_REG_SEQ_DISCONT_DISABLE;
//   ADC_REG_InitStruct.ContinuousMode = LL_ADC_REG_CONV_SINGLE;
//   ADC_REG_InitStruct.DMATransfer = LL_ADC_REG_DMA_TRANSFER_UNLIMITED;
//   ADC_REG_InitStruct.Overrun = LL_ADC_REG_OVR_DATA_PRESERVED;
//   LL_ADC_REG_Init(ADC1, &ADC_REG_InitStruct);
//   LL_ADC_REG_SetSequencerScanDirection(ADC1, LL_ADC_REG_SEQ_SCAN_DIR_FORWARD);
//   LL_ADC_SetSamplingTimeCommonChannels(ADC1, LL_ADC_SAMPLINGTIME_1CYCLE_5);
//   /* USER CODE BEGIN ADC_Init 2 */
//   LL_ADC_Enable(ADC1);
// //   LL_ADC_StartCalibration(ADC1); 
// //   while( LL_ADC_IsCalibrationOnGoing(ADC1)); 

//     LL_ADC_REG_StartConversion(ADC1);

//   LL_ADC_REG_SetDMATransfer(ADC1,LL_ADC_REG_DMA_TRANSFER_UNLIMITED);
  
//   /* USER CODE END ADC_Init 2 */

// }

/**
  * @brief TIM3 Initialization Function
  * @param None
  * @retval None
  */
// static void MX_TIM3_Init(void)
// {

//   /* USER CODE BEGIN TIM3_Init 0 */

//   /* USER CODE END TIM3_Init 0 */

//   LL_TIM_InitTypeDef TIM_InitStruct = {0};
//   LL_TIM_OC_InitTypeDef TIM_OC_InitStruct = {0};

//   LL_GPIO_InitTypeDef GPIO_InitStruct = {0};

//   /* Peripheral clock enable */
//   LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_TIM3);

//   /* USER CODE BEGIN TIM3_Init 1 */

//   /* USER CODE END TIM3_Init 1 */
//   TIM_InitStruct.Prescaler = 0;
//   TIM_InitStruct.CounterMode = LL_TIM_COUNTERMODE_UP;
//   TIM_InitStruct.Autoreload = 999;
//   TIM_InitStruct.ClockDivision = LL_TIM_CLOCKDIVISION_DIV1;
//   LL_TIM_Init(TIM3, &TIM_InitStruct);
//   LL_TIM_DisableARRPreload(TIM3);
//   LL_TIM_OC_EnablePreload(TIM3, LL_TIM_CHANNEL_CH1);
//   TIM_OC_InitStruct.OCMode = LL_TIM_OCMODE_PWM1;
//   TIM_OC_InitStruct.OCState = LL_TIM_OCSTATE_DISABLE;
//   TIM_OC_InitStruct.OCNState = LL_TIM_OCSTATE_DISABLE;
//   TIM_OC_InitStruct.CompareValue = 0;
//   TIM_OC_InitStruct.OCPolarity = LL_TIM_OCPOLARITY_HIGH;
//   LL_TIM_OC_Init(TIM3, LL_TIM_CHANNEL_CH1, &TIM_OC_InitStruct);
//   LL_TIM_OC_DisableFast(TIM3, LL_TIM_CHANNEL_CH1);
//   LL_TIM_OC_EnablePreload(TIM3, LL_TIM_CHANNEL_CH2);
//   LL_TIM_OC_Init(TIM3, LL_TIM_CHANNEL_CH2, &TIM_OC_InitStruct);
//   LL_TIM_OC_DisableFast(TIM3, LL_TIM_CHANNEL_CH2);
//   LL_TIM_SetTriggerOutput(TIM3, LL_TIM_TRGO_RESET);
//   LL_TIM_DisableMasterSlaveMode(TIM3);
//   /* USER CODE BEGIN TIM3_Init 2 */

//   /* USER CODE END TIM3_Init 2 */
//   LL_AHB1_GRP1_EnableClock(LL_AHB1_GRP1_PERIPH_GPIOA);
//   /**TIM3 GPIO Configuration
//   PA6   ------> TIM3_CH1
//   PA7   ------> TIM3_CH2
//   */
//   GPIO_InitStruct.Pin = LL_GPIO_PIN_6;
//   GPIO_InitStruct.Mode = LL_GPIO_MODE_ALTERNATE;
//   GPIO_InitStruct.Speed = LL_GPIO_SPEED_FREQ_LOW;
//   GPIO_InitStruct.OutputType = LL_GPIO_OUTPUT_PUSHPULL;
//   GPIO_InitStruct.Pull = LL_GPIO_PULL_NO;
//   GPIO_InitStruct.Alternate = LL_GPIO_AF_1;
//   LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

//   GPIO_InitStruct.Pin = LL_GPIO_PIN_7;
//   GPIO_InitStruct.Mode = LL_GPIO_MODE_ALTERNATE;
//   GPIO_InitStruct.Speed = LL_GPIO_SPEED_FREQ_LOW;
//   GPIO_InitStruct.OutputType = LL_GPIO_OUTPUT_PUSHPULL;
//   GPIO_InitStruct.Pull = LL_GPIO_PULL_NO;
//   GPIO_InitStruct.Alternate = LL_GPIO_AF_1;
//   LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

// }

// /**
//   * @brief TIM14 Initialization Function
//   * @param None
//   * @retval None
//   */
// static void MX_TIM14_Init(void)
// {

//   /* USER CODE BEGIN TIM14_Init 0 */

//   /* USER CODE END TIM14_Init 0 */

//   LL_TIM_InitTypeDef TIM_InitStruct = {0};

//   /* Peripheral clock enable */
//   LL_APB1_GRP1_EnableClock(LL_APB1_GRP1_PERIPH_TIM14);

//   /* TIM14 interrupt Init */
//   NVIC_SetPriority(TIM14_IRQn, 3);
//   NVIC_EnableIRQ(TIM14_IRQn);

//   /* USER CODE BEGIN TIM14_Init 1 */

//   /* USER CODE END TIM14_Init 1 */
//   TIM_InitStruct.Prescaler = 47;
//   TIM_InitStruct.CounterMode = LL_TIM_COUNTERMODE_UP;
//   TIM_InitStruct.Autoreload = 4999;
//   TIM_InitStruct.ClockDivision = LL_TIM_CLOCKDIVISION_DIV1;
//   LL_TIM_Init(TIM14, &TIM_InitStruct);
//   LL_TIM_DisableARRPreload(TIM14);
//   /* USER CODE BEGIN TIM14_Init 2 */

//   /* USER CODE END TIM14_Init 2 */

// }

/**
  * Enable DMA controller clock
  */
// static void MX_DMA_Init(void)
// {

//   /* Init with LL driver */
//   /* DMA controller clock enable */
//   LL_AHB1_GRP1_EnableClock(LL_AHB1_GRP1_PERIPH_DMA1);

//   /* DMA interrupt init */
//   /* DMA1_Channel1_IRQn interrupt configuration */
// //   NVIC_SetPriority(DMA1_Channel1_IRQn, 0);
// //   NVIC_EnableIRQ(DMA1_Channel1_IRQn);

// }

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
// static void MX_GPIO_Init(void)
// {
//   LL_GPIO_InitTypeDef GPIO_InitStruct = {0};

//   /* GPIO Ports Clock Enable */
//   LL_AHB1_GRP1_EnableClock(LL_AHB1_GRP1_PERIPH_GPIOA);

//   /**/
//   LL_GPIO_ResetOutputPin(GPIOA, LL_GPIO_PIN_1);

//   /**/
//   GPIO_InitStruct.Pin = LL_GPIO_PIN_1;
//   GPIO_InitStruct.Mode = LL_GPIO_MODE_OUTPUT;
//   GPIO_InitStruct.Speed = LL_GPIO_SPEED_FREQ_HIGH;
//   GPIO_InitStruct.OutputType = LL_GPIO_OUTPUT_PUSHPULL;
//   GPIO_InitStruct.Pull = LL_GPIO_PULL_NO;
//   LL_GPIO_Init(GPIOA, &GPIO_InitStruct);

// }

/* USER CODE BEGIN 4 */

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
