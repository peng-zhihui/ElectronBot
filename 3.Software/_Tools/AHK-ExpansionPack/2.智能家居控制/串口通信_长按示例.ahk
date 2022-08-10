/*
以下以小米无线开关为例，使用继电器实现变相控制智能家居。
其实也可以用Arduino+MOS管来实现，单击、双击、长按这些操作。
如果YKUS-1继电器设置无误的话，运行此脚本会得到继电器长按1.3秒的反应
*/
SetBatchLines -1
#SingleInstance Force

RS232_Port   := "COM1"  ; 这里改成你继电器对应的COM号
RS232_Baud  := 9600
RS232_Parity := "N"
RS232_Data  := 8
RS232_Stop  := 1

RS232_Settings := RS232_Port ":baud=" RS232_Baud " parity=" RS232_Parity " data=" RS232_Data " stop=" RS232_Stop " dtr=Off"

; 初始化并尝试连接串行端口
RS232_FileHandle := RS232_Initialize(RS232_Settings)

; 执行继电器长按操作示例代码
RS232_Write(RS232_FileHandle, "0xA0,0x01,0x03,0xA4")  ; 按下
Sleep 1300  ; 延时1.3秒=长按效果
RS232_Write(RS232_FileHandle, "0xA0,0x01,0x02,0xA3")  ; 抬起
Return

/*
; 执行继电器双击操作示例代码
RS232_Write(RS232_FileHandle, "0xA0,0x01,0x03,0xA4")  ; 按下
Sleep 100
RS232_Write(RS232_FileHandle, "0xA0,0x01,0x02,0xA3")  ; 抬起

Sleep 100  ; 延时100毫秒后，再次点击

RS232_Write(RS232_FileHandle, "0xA0,0x01,0x03,0xA4")  ; 按下
Sleep 100
RS232_Write(RS232_FileHandle, "0xA0,0x01,0x02,0xA3")  ; 抬起
Return
*/

; ================== 以下是脚本所用的函数类库 ==================

;###### Initialize RS232 COM Subroutine #################################
RS232_Initialize(RS232_Settings) {
    ;###### Extract/Format the RS232 COM Port Number ######
    ; 7/23/08 Thanks krisky68 for finding/solving the bug in which RS232 COM Ports greater than 9 didn't work.
    StringSplit, RS232_Temp, RS232_Settings, `:
    RS232_Temp1_Len := StrLen(RS232_Temp1)  ; For COM Ports > 9 \\.\ needs to prepended to the COM Port name.
    if (RS232_Temp1_Len > 4)  ; So the valid names are
        RS232_COM = \\.\%RS232_Temp1%  ; ... COM8  COM9   \\.\COM10  \\.\COM11  \\.\COM12 and so on...
     else
        RS232_COM = \\.\%RS232_Temp1%

    ; 8/10/09 A BIG Thanks to trenton_xavier for figuring out how to make COM Ports greater than 9 work for USB-Serial Dongles.
    StringTrimLeft, RS232_Settings, RS232_Settings, RS232_Temp1_Len+1 ; Remove the COM number (+1 for the semicolon) for BuildCommDCB.

    ;###### Build RS232 COM DCB ######
    ; Creates the structure that contains the RS232 COM Port number, baud rate,...
    VarSetCapacity(DCB, 28)
    , BCD_Result := DllCall("BuildCommDCB"
        ,"Str", RS232_Settings  ; lpDef
        ,"UInt", &DCB)  ; lpDCB
    if (BCD_Result <> 1)
        return RS232_FileHandle:=0

    ;###### Create RS232 COM File ######
    ; Creates the RS232 COM Port File Handle
    RS232_FileHandle := DllCall("CreateFile"
        ,"Str", RS232_COM  ; File Name
        ,"UInt", 0xC0000000  ; Desired Access
        ,"UInt", 3  ; Safe Mode
        ,"UInt", 0  ; Security Attributes
        ,"UInt", 3  ; Creation Disposition
        ,"UInt", 0  ; Flags And Attributes
        ,"UInt", 0  ; Template File
        ,"Cdecl Int")

    if (RS232_FileHandle < 1)
        return RS232_FileHandle := 0

    ;###### Set COM State ######
    ; Sets the RS232 COM Port number, baud rate,...
    SCS_Result := DllCall("SetCommState"
        ,"UInt", RS232_FileHandle ; File Handle
        ,"UInt", &DCB)  ; Pointer to DCB structure
    if (SCS_Result <> 1)
        RS232_Close(RS232_FileHandle)

    ;###### Create the SetCommTimeouts Structure ######
    ReadIntervalTimeout = 0xffffffff
    ReadTotalTimeoutMultiplier = 0x00000000
    ReadTotalTimeoutConstant = 0x00000000
    WriteTotalTimeoutMultiplier= 0x00000000
    WriteTotalTimeoutConstant  = 0x00000000

    VarSetCapacity(Data, 20, 0)  ; 5 * sizeof(DWORD)
    , NumPut(ReadIntervalTimeout, Data, 0, "UInt")
    , NumPut(ReadTotalTimeoutMultiplier, Data, 4, "UInt")
    , NumPut(ReadTotalTimeoutConstant, Data, 8, "UInt")
    , NumPut(WriteTotalTimeoutMultiplier, Data, 12, "UInt")
    , NumPut(WriteTotalTimeoutConstant, Data, 16, "UInt")

    ;###### Set the RS232 COM Timeouts ######
    , SCT_result := DllCall("SetCommTimeouts"
        ,"UInt", RS232_FileHandle  ; File Handle
        ,"UInt", &Data)  ; Pointer to the data structure
    if (SCT_result <> 1)
        RS232_Close(RS232_FileHandle)

    return RS232_FileHandle
}

;###### Close RS232 COM Subroutine #####################################
RS232_Close(RS232_FileHandle) {
    DllCall("CloseHandle", "UInt", RS232_FileHandle)
}

;###### Write to RS232 COM Subroutines ##################################
RS232_Write(RS232_FileHandle, Message) {
    SetFormat, Integer, DEC
    ; Parse the Message. Byte0 is the number of bytes in the array.
    StringSplit, Byte, Message, `,
    Data_Length=%Byte0%

    ; Set the Data buffer size, prefill with 0xFF.
    VarSetCapacity(Data, Byte0, 0xFF)

    ; Write the Message into the Data buffer
    i=1
    Loop %Byte0%
        NumPut(Byte%i%, Data, (i-1) , "UChar"), i++

    ;###### Write the data to the RS232 COM Port ######
    WF_Result := DllCall("WriteFile"
        ,"UInt", RS232_FileHandle  ; File Handle
        ,"UInt", &Data  ; Pointer to string to send
        ,"UInt", Data_Length  ; Data Length
        ,"UInt*", Bytes_Sent  ; Returns pointer to num bytes sent
        ,"Int", "NULL")
    if (WF_Result <> 1 or Bytes_Sent <> Data_Length)
        MsgBox,DLL 写文件 到 RS232 COM 失败, result=%WF_Result% `nData Length=%Data_Length% `nBytes_Sent=%Bytes_Sent%
}

;###### Read from RS232 COM Subroutines #################################
RS232_Read(RS232_FileHandle, Num_Bytes, ByRef RS232_Bytes_Received) {
    SetFormat, Integer, HEX

    ; Set the Data buffer size, prefill with 0x55 = ASCII character "U"
    ; VarSetCapacity won't assign anything less than 3 bytes. Meaning: If you
    ; tell it you want 1 or 2 byte size variable it will give you 3.
    Data_Length := VarSetCapacity(Data, Num_Bytes, 0x55)

    ;###### Read the data from the RS232 COM Port ######
    Read_Result := DllCall("ReadFile"
        ,"UInt", RS232_FileHandle  ; hFile
        ,"Str", Data  ; lpBuffer
        ,"Int", Num_Bytes  ; nNumberOfBytesToRead
        ,"UInt*", RS232_Bytes_Received  ; lpNumberOfBytesReceived
        ,"Int", 0)  ; lpOverlapped

    if (Read_Result <> 1) {
        ; MsgBox, 串行端口通信有问题。 `nRS232 COM 上的 Dll 读取文件 失败，result=%Read_Result% - 脚本现在将退出。
        RS232_Close(RS232_FileHandle)
        Exit
    }

    ;###### Format the received data ######
    ; This loop is necessary because AHK doesn't handle NULL (0x00) characters very nicely.
    ; Quote from AHK documentation under DllCall:
    ;     "Any binary zero stored in a variable by a function will hide all data to the right
    ;     of the zero; that is, such data cannot be accessed or changed by most commands and
    ;     functions. However, such data can be manipulated by the address and dereference operators
    ;     (& and *), as well as DllCall itself."
    i = 0
    Data_HEX =
    Loop %RS232_Bytes_Received% {
        ; First byte into the Rx FIFO ends up at position 0
        Data_HEX_Temp := NumGet(Data, i, "UChar")  ; Convert to HEX byte-by-byte
        StringTrimLeft, Data_HEX_Temp, Data_HEX_Temp, 2  ; Remove the 0x (added by the above line) from the front

        ; If there is only 1 character then add the leading "0'
        Length := StrLen(Data_HEX_Temp)
        if (Length =1)
            Data_HEX_Temp = 0%Data_HEX_Temp%
        i++
        ; Put it all together
        , Data_HEX := Data_HEX . Data_HEX_Temp
    }
    SetFormat, Integer, DEC
    return Data_HEX
}