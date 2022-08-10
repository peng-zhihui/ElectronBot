; 因为不熟悉Tcp工作流程，目前只能单向发送

#Persistent
#SingleInstance Force
SetBatchLines -1

; 被连接，可以控制连接者的ElectronBot
myTcp := New SocketTCP()
myTcp.onAccept := Func("OnTCPAccept")
myTcp.bind(["0.0.0.0", 33345])
myTcp.listen()
Return

; 按F1键或F2键向接收端发送命令
F1::newTcp.sendText("姿势∶15,0,0,0,30,150")

F2::newTcp.sendText("姿势∶0,0,0,0,0,0")

; 收到信息时触发
OnTCPAccept() {
    Global
    newTcp := myTcp.accept()
}


; ; ==================== Socket通信类库 ====================
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