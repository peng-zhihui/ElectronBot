#ifndef __USBD_CDC_IF_H__
#define __USBD_CDC_IF_H__

#include "usbd_cdc.h"


extern USBD_CDC_ItfTypeDef USBD_Interface_fops_HS;

uint8_t CDC_Transmit_HS(uint8_t* Buf, uint16_t Len);


#endif