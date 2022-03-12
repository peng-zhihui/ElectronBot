#include "usbd_cdc_if.h"
#include "robot.h"

extern Robot electron;
extern USBD_HandleTypeDef hUsbDeviceHS;


static int8_t CDC_Init_HS();
static int8_t CDC_DeInit_HS();
static int8_t CDC_Control_HS(uint8_t cmd, uint8_t* pbuf, uint16_t length);
static int8_t CDC_Receive_HS(uint8_t* pbuf, uint32_t* Len);
static int8_t CDC_TransmitCplt_HS(uint8_t* pbuf, uint32_t* Len, uint8_t epnum);


USBD_CDC_ItfTypeDef USBD_Interface_fops_HS =
    {
        CDC_Init_HS,
        CDC_DeInit_HS,
        CDC_Control_HS,
        CDC_Receive_HS,
        CDC_TransmitCplt_HS
    };


static int8_t CDC_Init_HS()
{
    USBD_CDC_SetTxBuffer(&hUsbDeviceHS, electron.usbBuffer.extraDataTx, 0);
    USBD_CDC_SetRxBuffer(&hUsbDeviceHS, electron.usbBuffer.rxData[electron.usbBuffer.pingPongIndex]);
    return (USBD_OK);
}


static int8_t CDC_DeInit_HS()
{
    return (USBD_OK);
}


static int8_t CDC_Control_HS(uint8_t cmd, uint8_t* pbuf, uint16_t length)
{
    switch (cmd)
    {
        case CDC_SEND_ENCAPSULATED_COMMAND:

            break;

        case CDC_GET_ENCAPSULATED_RESPONSE:

            break;

        case CDC_SET_COMM_FEATURE:

            break;

        case CDC_GET_COMM_FEATURE:

            break;

        case CDC_CLEAR_COMM_FEATURE:

            break;

            /*******************************************************************************/
            /* Line Coding Structure                                                       */
            /*-----------------------------------------------------------------------------*/
            /* Offset | Field       | Size | Value  | Description                          */
            /* 0      | dwDTERate   |   4  | Number |Data terminal rate, in bits per second*/
            /* 4      | bCharFormat |   1  | Number | Stop bits                            */
            /*                                        0 - 1 Stop bit                       */
            /*                                        1 - 1.5 Stop bits                    */
            /*                                        2 - 2 Stop bits                      */
            /* 5      | bParityType |  1   | Number | Parity                               */
            /*                                        0 - None                             */
            /*                                        1 - Odd                              */
            /*                                        2 - Even                             */
            /*                                        3 - Mark                             */
            /*                                        4 - Space                            */
            /* 6      | bDataBits  |   1   | Number Data bits (5, 6, 7, 8 or 16).          */
            /*******************************************************************************/
        case CDC_SET_LINE_CODING:

            break;

        case CDC_GET_LINE_CODING:

            break;

        case CDC_SET_CONTROL_LINE_STATE:

            break;

        case CDC_SEND_BREAK:

            break;

        default:
            break;
    }

    return (USBD_OK);
}


static int8_t CDC_Receive_HS(uint8_t* Buf, uint32_t* Len)
{
    // USBD_CDC_SetRxBuffer(&hUsbDeviceHS, frameBuffer[1]);
    // USBD_CDC_ReceivePacket(&hUsbDeviceHS);
    // Use LL version code instead

    // Got data length
    electron.usbBuffer.receivedPacketLen = *Len;
    if (electron.usbBuffer.receivedPacketLen == 224)
        electron.SwitchPingPongBuffer();

    // Prepare next receive
    USBD_CDC_HandleTypeDef* hcdc = (USBD_CDC_HandleTypeDef*) (hUsbDeviceHS.pClassData);

    // Set receive buffer
    hcdc->RxBuffer = electron.GetPingPongBufferPtr();
    if (electron.usbBuffer.receivedPacketLen == 512)
    {
        electron.usbBuffer.rxDataOffset += 512;
    }

    return USBD_LL_PrepareReceive(&hUsbDeviceHS, CDC_OUT_EP, hcdc->RxBuffer, CDC_DATA_HS_OUT_PACKET_SIZE);

}


uint8_t CDC_Transmit_HS(uint8_t* Buf, uint16_t Len)
{
    uint8_t result = USBD_OK;
    USBD_CDC_HandleTypeDef* hcdc = (USBD_CDC_HandleTypeDef*) hUsbDeviceHS.pClassData;
    if (hcdc->TxState != 0)
    {
        return USBD_BUSY;
    }
    USBD_CDC_SetTxBuffer(&hUsbDeviceHS, Buf, Len);
    result = USBD_CDC_TransmitPacket(&hUsbDeviceHS);
    return result;
}


static int8_t CDC_TransmitCplt_HS(uint8_t* Buf, uint32_t* Len, uint8_t epnum)
{
    uint8_t result = USBD_OK;
    UNUSED(Buf);
    UNUSED(Len);
    UNUSED(epnum);
    return result;
}
