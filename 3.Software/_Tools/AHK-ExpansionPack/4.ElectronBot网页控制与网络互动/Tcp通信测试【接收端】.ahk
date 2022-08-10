; 因为不熟悉Tcp工作流程，目前只能单向发送

#SingleInstance Force
SetWorkingDir %A_ScriptDir%
SetBatchLines -1
; 被控端

Global 姿势 := New LowLevelSDK()

; 测试公网连接时，不能服务端和客户端在同一台电脑上运行。【会连接不上】
Loop {
    myTcp := New SocketTCP()
    myTcp.Connect(["10.0.0.30", 33345])  ; 支持动态域名解析，比如：myname.f3322.net
    ToolTip % "循环不断等待接收信息：`n" 接收信息 := myTcp.recvText()
    if (InStr(接收信息, "姿势")!=0) {  ; 判断信息是否包含"姿势"两字，做为自定义命令识别
        分割成数组 := StrSplit(接收信息, ",")  ; 将参数分割成数组，参1用正则把非数字内容去掉
        姿势.同步姿势(RegExReplace(分割成数组[1], "\D"), 分割成数组[2], 分割成数组[3], 分割成数组[4], 分割成数组[5],  分割成数组[6])
    }
}
Return

; 如果按F3和F4键ElectronBot都没有反应，那得退出其它上位机和驱动，重新插拔ElectronBot再试一次
F3::
最后一条信息 := "姿势∶15,0,0,0,30,130"
分割成数组 := StrSplit(最后一条信息, ",")  ; 将参数分割成数组，参1用正则把非数字内容去掉
姿势.同步姿势(RegExReplace(分割成数组[1], "\D"), 分割成数组[2], 分割成数组[3], 分割成数组[4], 分割成数组[5],  分割成数组[6])
Return

; 按F4键为直接调试
F4::姿势.同步姿势(0,0,0,0,30,0)

; ================== ElectronBotSDK控制类库 ==================
Class LowLevelSDK {
    ; LowLevel 加载与连接
    __New(FilePath:="ElectronBotSDK-LowLevel.dll") {
        ; 判断文件是否存在和适配中文路径加载
        if FileExist(FilePath) {
            if (InStr(FilePath, "\")=0) and (InStr(FilePath, "/")=0)
                FilePath := A_ScriptDir "\" FilePath
            SplitPath, FilePath, OutFileName, OutDir
            DllCall("SetDllDirectory", "Str", OutDir)  ; 重定向dll加载目录
            DllCall("LoadLibrary", "Str", OutFileName)
            this.DLLFunc := OutFileName
            this.pLowLevel := DllCall(this.DLLFunc "\AHK_New", "Ptr")
            DllCall(this.DLLFunc "\AHK_Connect", "Ptr", this.pLowLevel, "char")
            DllCall("SetDllDirectory", "Str", A_ScriptDir)
        } else {
            MsgBox 0x10, 需加载的SDK文件不存在！, %FilePath% 文件不存在！`n`n请将此脚本转移到附带的SDK改版目录下，`n或者指定SDK的路径后，再次打开脚本进行调用。
            ExitApp
        }
    }

    ; LowLevel 连接
    连接() {
        Return DllCall(this.DLLFunc "\AHK_Connect", "Ptr", this.pLowLevel, "char")
    }

    ; 断开LowLevel连接并清理占用
    断开连接() {
        DllCall(this.DLLFunc "\AHK_Disconnect", "Ptr", this.pLowLevel, "char")
        DllCall(this.DLLFunc "\AHK_Delete", "Ptr", this.pLowLevel)
    }

    ; 函数定义与稚晖君的上位机顺序一致：
    ; 参1=头部、参2=身体转向、参3=左臂展开、参4=左臂抬起、参5=右臂展开、参6=右臂抬起，参7可以设置图片路径
    ; 参1=-15~15、参2=-90~90、参3=-30~30、参4=-180~180、参5=-30~30、参6=-180~180。超过限定值则无效
    同步姿势(_j1, _j6, _j2, _j3, _j4, _j5, FilePath:="") {
        ; 原SDK对应：j1=头部、j2=左臂展开、j3=右臂抬起、j4=右臂展开、j5=左臂抬起、j6=身体转向
        DllCall(this.DLLFunc "\AHK_SetJointAngles", "Ptr", this.pLowLevel, "Float", _j1, "Float", _j2, "Float", _j3, "Float", _j4, "Float", _j5, "Float", _j6, "int", True)
        ; 在外部用全局变量设置路径可以让每个动作都调用一张图片，比如：Global LLSDKFilePath := "test.jpg"
        , (LLSDKFilePath!="" && FilePath := LLSDKFilePath)
        if (FilePath!="")
            if FileExist(FilePath)
                DllCall(this.DLLFunc "\AHK_SetImageSrc_Path", "Ptr", this.pLowLevel, "astr", FilePath)
        Return DllCall(this.DLLFunc "\AHK_Sync", "Ptr", this.pLowLevel, "char")  ; 同步上传
    }

    ; 仅设置不同步上传
    设置姿势(_j1, _j6, _j2, _j3, _j4, _j5, Enable:=True) {
        DllCall(this.DLLFunc "\AHK_SetJointAngles", "Ptr", this.pLowLevel, "Float", _j1, "Float", _j2, "Float", _j3, "Float", _j4, "Float", _j5, "Float", _j6, "int", Enable)
    }

    ; 需要同步姿势后，才能获取关节角度。返回数组
    获取关节角度() {
        VarSetCapacity(_angles, 24)
        DllCall(this.DLLFunc "\AHK_GetJointAngles", "Ptr", this.pLowLevel, "Ptr", &_angles)
        Sleep 10  ; 需要获取两次得到当前数据
        DllCall(this.DLLFunc "\AHK_GetJointAngles", "Ptr", this.pLowLevel, "Ptr", &_angles)

        Return [ NumGet(_angles, 0, "float"), NumGet(_angles, 4, "float"), NumGet(_angles, 8, "float"), NumGet(_angles, 12, "float"), NumGet(_angles, 16, "float"), NumGet(_angles, 20, "float") ]
    }

    同步() {
        Return DllCall(this.DLLFunc "\AHK_Sync", "Ptr", this.pLowLevel, "char")
    }

    设置图像源路径(FilePath) {
        DllCall(this.DLLFunc "\AHK_SetImageSrc_Path", "Ptr", this.pLowLevel, "astr", FilePath)
    }

    设置图像Data(image_data) {
        DllCall(this.DLLFunc "\AHK_SetImageSrc_MatData", "Ptr", this.pLowLevel, "Ptr", image_data)
    }

    设置额外数据(_data, _len) {  ; 没条件测试此项
        DllCall(this.DLLFunc "\AHK_SetExtraData", "Ptr", this.pLowLevel, "Uint*", _data, "Uint", _len)
    }

    获取额外数据(_data) {  ; 没条件测试此项
        Return DllCall(this.DLLFunc "\AHK_GetExtraData", "Ptr", this.pLowLevel, "Uint*", _data)
    }
}

Class PlayerSDK {
    ; Player 加载与连接
    __New(FilePath:="ElectronBotSDK-Player.dll") {
        ; 判断文件是否存在和适配中文路径加载
        if FileExist(FilePath) {
            if (InStr(FilePath, "\")=0) and (InStr(FilePath, "/")=0)
                FilePath := A_ScriptDir "\" FilePath
            SplitPath, FilePath, OutFileName, OutDir
            DllCall("SetDllDirectory", "Str", OutDir)  ; 重定向dll加载目录
            DllCall("LoadLibrary", "Str", OutFileName)
            this.DLLFunc := OutFileName
            this.pPlayer := DllCall(this.DLLFunc "\AHK_New", "Ptr")
            DllCall(this.DLLFunc "\AHK_Connect", "Ptr", this.pPlayer, "char")
            DllCall("SetDllDirectory", "Str", A_ScriptDir)
        } else {
            MsgBox 0x10, 需加载的SDK文件不存在！, %FilePath% 文件不存在！`n`n请将此脚本转移到附带的SDK改版目录下，`n或者指定SDK的路径后，再次打开脚本进行调用。
            ExitApp
        }
    }

    ; 断开Player连接并清理占用
    断开连接() {
        if this.ExpressionsPlayed {
            DllCall(this.DLLFunc "\AHK_Disconnect", "Ptr", this.pPlayer, "char")
            DllCall(this.DLLFunc "\AHK_Delete", "Ptr", this.pPlayer)
        }
    }

    ; Player 连接
    连接() {
        Return DllCall(this.DLLFunc "\AHK_Connect", "Ptr", this.pPlayer, "char")
    }

    ; 需确保与上个动作有150毫秒延时，避免线程抢占卡死
    播放表情(FilePath) {
        if FileExist(FilePath) {
            DllCall(this.DLLFunc "\AHK_Stop", "Ptr", this.pPlayer)
            this.pPlayer := DllCall(this.DLLFunc "\AHK_New", "Ptr")
            this.ExpressionsPlayed := DllCall(this.DLLFunc "\AHK_Connect", "Ptr", this.pPlayer, "char")
            Return DllCall(this.DLLFunc "\AHK_Play", "Ptr", this.pPlayer, "astr", FilePath)
        }
    }

    停止表情() {
        DllCall(this.DLLFunc "\AHK_Stop", "Ptr", this.pPlayer)
    }

    设置播放速度(ratio) {
        DllCall(this.DLLFunc "\AHK_SetPlaySpeed", "Ptr", this.pPlayer, "Float", ratio)
    }
}

; ==================== Socket通信类库 ====================
; https://github.com/G33kDude/Socket.ahk
class Socket {
	static WM_SOCKET := 0x9987, MSG_PEEK := 2
	static FD_READ := 1, FD_ACCEPT := 8, FD_CLOSE := 32
	static Blocking := True, BlockSleep := 50

	__New(Socket:=-1) {
		static Init
		if (!Init) {
			DllCall("LoadLibrary", "Str", "Ws2_32", "Ptr")
			, VarSetCapacity(WSAData, 394+A_PtrSize)
			if (Error := DllCall("Ws2_32\WSAStartup", "UShort", 0x0202, "Ptr", &WSAData))
				throw Exception("Error starting Winsock",, Error)
			if (NumGet(WSAData, 2, "UShort") != 0x0202)
				throw Exception("Winsock version 2.2 not available")
			Init := True
		}
		this.Socket := Socket
	}

	__Delete() {
		if (this.Socket != -1)
			this.Disconnect()
	}

	Connect(Address) {
		if (this.Socket != -1)
			throw Exception("Socket already connected")
		Next := pAddrInfo := this.GetAddrInfo(Address)
		while Next {
			ai_addrlen := NumGet(Next+0, 16, "UPtr")
			, ai_addr := NumGet(Next+0, 16+(2*A_PtrSize), "Ptr")
			if ((this.Socket := DllCall("Ws2_32\socket", "Int", NumGet(Next+0, 4, "Int")
				, "Int", this.SocketType, "Int", this.ProtocolId, "UInt")) != -1) {
				if (DllCall("Ws2_32\WSAConnect", "UInt", this.Socket, "Ptr", ai_addr
					, "UInt", ai_addrlen, "Ptr", 0, "Ptr", 0, "Ptr", 0, "Ptr", 0, "Int") == 0) {
					DllCall("Ws2_32\freeaddrinfo", "Ptr", pAddrInfo) ; TODO: Error Handling
				return this.EventProcRegister(this.FD_READ | this.FD_CLOSE)
				}
				this.Disconnect()
			}
			Next := NumGet(Next+0, 16+(3*A_PtrSize), "Ptr")
		}
		throw Exception("Error connecting")
	}

	Bind(Address) {
		if (this.Socket != -1)
			throw Exception("Socket already connected")
		Next := pAddrInfo := this.GetAddrInfo(Address)
		while Next {
			ai_addrlen := NumGet(Next+0, 16, "UPtr")
			, ai_addr := NumGet(Next+0, 16+(2*A_PtrSize), "Ptr")
			if ((this.Socket := DllCall("Ws2_32\socket", "Int", NumGet(Next+0, 4, "Int")
				, "Int", this.SocketType, "Int", this.ProtocolId, "UInt")) != -1) {
				if (DllCall("Ws2_32\bind", "UInt", this.Socket, "Ptr", ai_addr
					, "UInt", ai_addrlen, "Int") == 0) {
					DllCall("Ws2_32\freeaddrinfo", "Ptr", pAddrInfo) ; TODO: ERROR HANDLING
				return this.EventProcRegister(this.FD_READ | this.FD_ACCEPT | this.FD_CLOSE)
				}
				this.Disconnect()
			}
			Next := NumGet(Next+0, 16+(3*A_PtrSize), "Ptr")
		}
		throw Exception("Error binding")
	}

	Listen(backlog=32) {
		return DllCall("Ws2_32\listen", "UInt", this.Socket, "Int", backlog) == 0
	}

	Accept() {
		if ((s := DllCall("Ws2_32\accept", "UInt", this.Socket, "Ptr", 0, "Ptr", 0, "Ptr")) == -1)
			throw Exception("Error calling accept",, this.GetLastError())
		Sock := new Socket(s)
		, Sock.ProtocolId := this.ProtocolId
		, Sock.SocketType := this.SocketType
		, Sock.EventProcRegister(this.FD_READ | this.FD_CLOSE)
		return Sock
	}

	Disconnect() {
		; Return 0 if not connected
		if (this.Socket == -1)
			return 0

		; Unregister the socket event handler and close the socket
		this.EventProcUnregister()
		if (DllCall("Ws2_32\closesocket", "UInt", this.Socket, "Int") == -1)
			throw Exception("Error closing socket",, this.GetLastError())
		this.Socket := -1
		return 1
	}

	MsgSize() {
		static FIONREAD := 0x4004667F
		if (DllCall("Ws2_32\ioctlsocket", "UInt", this.Socket, "UInt", FIONREAD, "UInt*", argp) == -1)
			throw Exception("Error calling ioctlsocket",, this.GetLastError())
		return argp
	}

	Send(pBuffer, BufSize, Flags:=0) {
		if ((r := DllCall("Ws2_32\send", "UInt", this.Socket, "Ptr", pBuffer, "Int", BufSize, "Int", Flags)) == -1)
			throw Exception("Error calling send",, this.GetLastError())
		return r
	}

	SendText(Text, Flags:=0, Encoding:="UTF-8") {
		VarSetCapacity(Buffer, StrPut(Text, Encoding) * ((Encoding="UTF-16"||Encoding="cp1200") ? 2 : 1))
		return this.Send(&Buffer, StrPut(Text, &Buffer, Encoding) - 1)
	}

	Recv(ByRef Buffer, BufSize:=0, Flags:=0) {
		while (!(Length := this.MsgSize()) && this.Blocking)
			Sleep, this.BlockSleep
		if !Length
			return 0
		if !BufSize
			BufSize := Length
		VarSetCapacity(Buffer, BufSize)
		if ((r := DllCall("Ws2_32\recv", "UInt", this.Socket, "Ptr", &Buffer, "Int", BufSize, "Int", Flags)) == -1)
			throw Exception("Error calling recv",, this.GetLastError())
		return r
	}

	RecvText(BufSize:=0, Flags:=0, Encoding:="UTF-8") {
		if (Length := this.Recv(Buffer, BufSize, flags))
			return StrGet(&Buffer, Length, Encoding)
		return ""
	}

	RecvLine(BufSize:=0, Flags:=0, Encoding:="UTF-8", KeepEnd:=False) {
		while !(i := InStr(this.RecvText(BufSize, Flags|this.MSG_PEEK, Encoding), "`n")) {
			if !this.Blocking
				return ""
			Sleep, this.BlockSleep
		}
		if KeepEnd
			return this.RecvText(i, Flags, Encoding)
		else
			return RTrim(this.RecvText(i, Flags, Encoding), "`r`n")
	}

	GetAddrInfo(Address) {
		; TODO: Use GetAddrInfoW
		Host := Address[1], Port := Address[2]
		, VarSetCapacity(Hints, 16+(4*A_PtrSize), 0)
		, NumPut(this.SocketType, Hints, 8, "Int")
		, NumPut(this.ProtocolId, Hints, 12, "Int")
		if (Error := DllCall("Ws2_32\getaddrinfo", "AStr", Host, "AStr", Port, "Ptr", &Hints, "Ptr*", Result))
			throw Exception("Error calling GetAddrInfo",, Error)
		return Result
	}

	OnMessage(wParam, lParam, Msg, hWnd) {
		Critical
		if (Msg != this.WM_SOCKET || wParam != this.Socket)
			return
		if (lParam & this.FD_READ)
			this.onRecv()
		else if (lParam & this.FD_ACCEPT)
			this.onAccept()
		else if (lParam & this.FD_CLOSE)
			this.EventProcUnregister(), this.OnDisconnect()
	}

	EventProcRegister(lEvent) {
		this.AsyncSelect(lEvent)
		if !this.Bound
			this.Bound := this.OnMessage.Bind(this)
			, OnMessage(this.WM_SOCKET, this.Bound)
	}

	EventProcUnregister() {
		this.AsyncSelect(0)
		if this.Bound
			OnMessage(this.WM_SOCKET, this.Bound, 0)
			, this.Bound := False
	}

	AsyncSelect(lEvent) {
		if (DllCall("Ws2_32\WSAAsyncSelect"
			, "UInt", this.Socket    ; s
			, "Ptr", A_ScriptHwnd    ; hWnd
			, "UInt", this.WM_SOCKET ; wMsg
			, "UInt", lEvent) == -1) ; lEvent
			throw Exception("Error calling WSAAsyncSelect",, this.GetLastError())
	}

	GetLastError() {
		return DllCall("Ws2_32\WSAGetLastError")
	}
}

class SocketTCP extends Socket {
	static ProtocolId := 6 ; IPPROTO_TCP
	static SocketType := 1 ; SOCK_STREAM
}

class SocketUDP extends Socket {
	static ProtocolId := 17 ; IPPROTO_UDP
	static SocketType := 2  ; SOCK_DGRAM

	SetBroadcast(Enable) {
		static SOL_SOCKET := 0xFFFF, SO_BROADCAST := 0x20
		if (DllCall("Ws2_32\setsockopt"
			, "UInt", this.Socket ; SOCKET s
			, "Int", SOL_SOCKET   ; int    level
			, "Int", SO_BROADCAST ; int    optname
			, "UInt*", !!Enable   ; *char  optval
			, "Int", 4) == -1)    ; int    optlen
			throw Exception("Error calling setsockopt",, this.GetLastError())
	}
}