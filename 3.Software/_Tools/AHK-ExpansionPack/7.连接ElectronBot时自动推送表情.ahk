; 可以与 "8.热键快速调试ElectronBot.ahk" 脚本进行合并，实现热拔插同步表情与连接
#NoEnv
#SingleInstance Force
SetWorkingDir %A_ScriptDir%
SetBatchLines -1

OnMessage(0x219, "WM_DEVICECHANGE")  ; 注册USB设备监控事件
拔出时数组 := JEE_DeviceList()  ; 初始化监控事件

; 初始表情图和初始姿势的代码示例
新进程代码=
(`%
#NoTrayIcon
SetWorkingDir %A_ScriptDir%
DllCall("LoadLibrary", "Str", "ElectronBotSDK-LowLevel.dll")  ; 加载SDK文件路径
pLowLevel := DllCall("ElectronBotSDK-LowLevel\AHK_New", "Ptr")
DllCall("ElectronBotSDK-LowLevel\AHK_Connect", "Ptr", pLowLevel, "char")
; 原SDK对应：j1=头部、j2=左臂展开、j3=右臂抬起、j4=右臂展开、j5=左臂抬起、j6=身体转向
DllCall("ElectronBotSDK-LowLevel\AHK_SetJointAngles", "Ptr", pLowLevel, "Float", 15, "Float", 0, "Float", 0, "Float", 0, "Float", 0, "Float", 0, "int", True)
DllCall("ElectronBotSDK-LowLevel\AHK_SetImageSrc_Path", "Ptr", pLowLevel, "astr", "test.jpg")  ; 图片路径
DllCall("ElectronBotSDK-LowLevel\AHK_Sync", "Ptr", pLowLevel, "char")  ; 同步上传
DllCall("ElectronBotSDK-LowLevel\AHK_Disconnect", "Ptr", pLowLevel, "char")
)

/*
; 播放指定表情的示例代码
新进程代码=
(`%
#NoTrayIcon
SetWorkingDir %A_ScriptDir%
DllCall("LoadLibrary", "Str", "ElectronBotSDK-Player.dll")  ; 加载SDK文件路径
pPlayer := DllCall("ElectronBotSDK-Player\AHK_New", "Ptr")
DllCall("ElectronBotSDK-Player\AHK_Connect", "Ptr", pPlayer, "char")
DllCall("ElectronBotSDK-Player\AHK_Play", "Ptr", pPlayer, "astr", "video.mp4")  ; 播放指定表情路径
DllCall("ElectronBotSDK-Player\AHK_Disconnect", "Ptr", pPlayer, "char")
)
*/
Return

; USB设备监控事件反馈
WM_DEVICECHANGE(wParam, lParam, msg, hwnd) {
    Global
    if (wParam=0x8000) {  ; 插入时
        插入时数组 := JEE_DeviceList()
        Loop % 插入时数组.Length() {
            if (插入时数组[A_Index]!=拔出时数组[A_Index]) {
                ; if (InStr(插入时数组[A_Index], "(COM8")!=0)  ; 此行为ElectronBot的COM端口为8时，同步表情【自行去注释使用】
                    ElectronBot初始化(新进程代码)
                Break
            }
        }
    } else if (wParam=0x8004) {  ; 拔出时
        拔出时数组 := JEE_DeviceList()
    }
}

; 使用临时进程启动自定义脚本
ElectronBot初始化(s) {
    if !A_Is64bitOS
        MsgBox 0x30, 当前AHK解释器不是64位的！, 因为操作 ElectronBot 需要 64 位解释器，`n不是 64 位解释器将无法同步表情与姿势。
	s:=s "`nExitApp"
	, exec := ComObjCreate("WScript.Shell").Exec(A_AhkPath " /f *")
	, exec.StdIn.Write(s)
	, exec.StdIn.Close()
}


; ================== 以下是脚本所用的函数类库 ==================

JEE_DeviceList() {
	local
	; DIGCF_DEFAULT := 0x1 ;only valid with DIGCF_DEVICEINTERFACE
	; DIGCF_PRESENT := 0x2
	; DIGCF_ALLCLASSES := 0x4
	; DIGCF_PROFILE := 0x8
	; DIGCF_DEVICEINTERFACE := 0x10

	if !(hModule := DllCall("kernel32\LoadLibrary", "Str","setupapi.dll", "Ptr"))
	|| !(hDevInfo := DllCall("setupapi\SetupDiGetClassDevs", "Ptr", 0, "Str","USB", "Ptr", 0, "UInt",0x2|0x4, "Ptr"))
		return

	oArray := []
	, vSize := A_PtrSize=8 ? 32 : 28
	, VarSetCapacity(SP_DEVINFO_DATA, vSize, 0)
	, NumPut(vSize, SP_DEVINFO_DATA, 0, "UInt") ; cbSize

	, JEE_PropertyKeyCreate(DEVPKEY_Device_FriendlyName, "{A45C254E-DF1C-4EFD-8020-67D146A850E0}", 14) ; source: propkey.h

	, vPropType := vRequiredSize := 0
	Loop {
		if (!DllCall("setupapi\SetupDiEnumDeviceInfo", "Ptr", hDevInfo, "UInt", A_Index-1, "Ptr", &SP_DEVINFO_DATA)) {
			if (A_LastError != 0x103) ; ERROR_NO_MORE_ITEMS := 0x103
				MsgBox, % "SetupDiEnumDeviceInfo error"
			break
		}

		; get property: friendly name:
		if (!DllCall("setupapi\SetupDiGetDevicePropertyW", "Ptr", hDevInfo, "Ptr", &SP_DEVINFO_DATA, "Ptr", &DEVPKEY_Device_FriendlyName, "UInt*", vPropType, "Ptr", 0, "UInt", 0, "UInt*", vRequiredSize, "UInt", 0) && A_LastError == 0x7A) { ; ERROR_INSUFFICIENT_BUFFER := 0x7A
			VarSetCapacity(vFriendlyName, vRequiredSize)
			if (!DllCall("setupapi\SetupDiGetDevicePropertyW", "Ptr", hDevInfo, "Ptr", &SP_DEVINFO_DATA, "Ptr", &DEVPKEY_Device_FriendlyName, "UInt*", vPropType, "WStr", vFriendlyName, "UInt", vRequiredSize, "Ptr", 0, "UInt", 0))
				MsgBox, % "SetupDiGetDevicePropertyW (DEVPKEY_Device_FriendlyName) error"
		}

        if (RegExMatch(vFriendlyName, "(\(COM([\s\S]*))", COMnumber)!=0)
            oArray[A_Index-1] := vFriendlyName
            , vFriendlyName := ""
	}

	DllCall("setupapi\SetupDiDestroyDeviceInfoList", "Ptr", hDevInfo)
	, DllCall("kernel32\FreeLibrary", "Ptr", hModule)
	return oArray
}

JEE_PropertyKeyCreate(ByRef vPropertyKey, vGUID, vPropertyID) {
	local
	VarSetCapacity(vPropertyKey, 20, 0)
	, vAddr := &vPropertyKey
	, DllCall("ole32\CLSIDFromString", "WStr", vGUID, "Ptr", vAddr) ; fmtid
	, NumPut(vPropertyID, vAddr+0, 16, "UInt") ; pid
}