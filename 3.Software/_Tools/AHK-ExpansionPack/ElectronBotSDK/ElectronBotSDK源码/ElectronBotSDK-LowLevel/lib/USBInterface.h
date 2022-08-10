#ifndef __USBTRANSMIT_H__
#define __USBTRANSMIT_H__

#include <Windows.h>
#include <sdkddkver.h>
#include <iostream>

using namespace std;

#define DLL_API extern "C" _declspec(dllexport)

#define EP0     0x00
#define EP1_IN  0x81
#define EP1_OUT 0x01

DLL_API
int USB_ScanDevice(int _usbPid, int _usbVid);
DLL_API
bool USB_OpenDevice(int _devIndex);
DLL_API
bool USB_CloseDevice(int _devIndex);
DLL_API
bool USB_CheckDevice(int _devIndex);
DLL_API
bool USB_BulkTransmit(int _devIndex, int _pipeNum, char* _sendBuffer, int _len, int _timeout);
DLL_API
int USB_BulkReceive(int _device, int _pipeNum, char* _data, int _len, int _timeout);
DLL_API
bool USB_IntTransmit(int _devIndex, int _pipeNum, char* _sendBuffer, int _len, int _timeout);
DLL_API
int USB_IntReceive(int _device, int _pipeNum, char* _data, int _len, int _timeout);
DLL_API
bool USB_CtrlData(int _devIndex, int _requestType, int _request, int _value,
                  int _index, char* _bytes, int _size, int _timeout);

#endif