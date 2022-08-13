; 将 此脚本 与 "mime.types" 放到 "ElectronBotSDK" 目录后，运行此脚本即可测试。

#Persistent
#SingleInstance Force
SetBatchLines -1
SetWorkingDir %A_ScriptDir%

; ElectronBotSDK的加载与连接
Global 姿势 := New LowLevelSDK()
Global 表情 := New PlayerSDK()

; 参数结构："网页路径" : Func("对应调用的函数名称")
paths := {"/" : Func("主界面")
            , "/DianTou" : Func("点头")
            , "/ZuoShou" : Func("左手驱动")
            , "/YouShou" : Func("右手驱动")
            , "/BiaoQing" : Func("表情测试")}

Server := New HttpServer()
Server.LoadMimes("mime.types")
Server.SetPaths(paths)
HTTP服务端口 := 8866
Server.Serve(HTTP服务端口)
Run "http://127.0.0.1:%HTTP服务端口%/" ; 启动脚本后打开控制台网页【这句可删除】
Return


点头(ByRef req, ByRef res) {
    姿势.同步姿势(15, 0, 0, 0, 0, 0)
    Sleep 180
    姿势.同步姿势(0, 0, 0, 0, 0, 0)
    Sleep 180
    姿势.同步姿势(15, 0, 0, 0, 0, 0)
    Sleep 180
    姿势.同步姿势(0, 0, 0, 0, 0, 0)
    ; 后退回主界面
    res.SetBodyText("<script type=""text/javascript"">setTimeout(""history.go(-1)"", 0);  </script>"), res.status := 200
 }

左手驱动(ByRef req, ByRef res) {
    Static onoff
    if (onoff := !onoff)
        姿势.同步姿势(0, 0, 30, 180, 0, 0)
    else
        姿势.同步姿势(0, 0, 0, 0, 0, 0)
    res.SetBodyText("<script type=""text/javascript"">setTimeout(""history.go(-1)"", 0);  </script>"), res.status := 200
 }

右手驱动(ByRef req, ByRef res) {
    Static toggle
    if (toggle := !toggle)
        姿势.同步姿势(0, 0, 0, 0, 30, 180)
    else
        姿势.同步姿势(0, 0, 0, 0, 0, 0)
    res.SetBodyText("<script type=""text/javascript"">setTimeout(""history.go(-1)"", 0);  </script>"), res.status := 200
 }

表情测试(ByRef req, ByRef res) {
    表情.播放表情("video.mp4")
    ; 跳转回主界面
    res.SetBodyText("<head><meta http-equiv=""refresh"" content=""0.1;url=/""></head>"), res.status := 200
 }

; AHK网页控制台：https://github.com/dbgba/HTTPRemoteConsole
; 网页控制台的主页面显示
主界面(ByRef req, ByRef res) {
    主界面网页源码 =
    (LTrim
        <html><head><meta charset="UTF-8"><head><meta name="viewport" content="width=device-width">
        <title>ElectronBot网页控制台</title>
        <link href='http://fonts.googleapis.com/css?family=Ubuntu' rel='stylesheet' type='text/css'><style type="text/css">
        html {font-family: Ubuntu;}
        body {margin:25px;margin-top:5px;}
        h4 {font-size: 20px;color:green;}
        h5 {font-size: 16px;color:teal;}
        </style></head></html>
        <div id="wb_Text10" style="position:absolute;left:169px;top:10px;width:73px;height:18px;text-align:center;z-index:26;">
            <span style="color:#000000;font-family:微软雅黑;font-size:17px;"><strong>简单演示</strong></span>
        </div>
        <hr id="Line2" style="position:absolute;left:19px;top:14px;width:136px;z-index:27;">
        <hr id="Line3" style="position:absolute;left:256px;top:14px;width:135px;z-index:28;">
            <input type="submit" id="Button1" value="点头" style="position:absolute;left:12px;top:46px;width:78px;height:24px;z-index:6;">
            <input type="submit" id="Button2" value="左手驱动" style="position:absolute;left:114px;top:46px;width:78px;height:24px;z-index:7;">
            <input type="submit" id="Button3" value="右手驱动" style="position:absolute;left:217px;top:46px;width:78px;height:24px;z-index:8;">
            <input type="submit" id="Button4" value="表情测试" style="position:absolute;left:321px;top:46px;width:78px;height:24px;z-index:16;">
            <input type="submit" id="Button5" value="待定1" style="position:absolute;left:12px;top:100px;width:78px;height:24px;z-index:12;">
            <input type="submit" id="Button6" value="待定2" style="position:absolute;left:114px;top:100px;width:78px;height:24px;z-index:12;">
            <input type="submit" id="Button7" value="待定3" style="position:absolute;left:217px;top:100px;width:78px;height:24px;z-index:20;">
            <input type="submit" id="Button8" value="待定4" style="position:absolute;left:321px;top:100px;width:78px;height:24px;z-index:21;">
    <script>
    //以下是点击按钮对应跳转的代码
    window.onload = function () {
        var div = document.querySelector("div");
        document.querySelector("#Button1").onclick = function () {
                window.location.href='/DianTou'; ;return false;
          }
        document.querySelector("#Button2").onclick = function () {
                window.location.href='/ZuoShou'; ;return false;
          }
        document.querySelector("#Button3").onclick = function () {
                window.location.href='/YouShou'; ;return false;
          }
        document.querySelector("#Button4").onclick = function () {
                window.location.href='/BiaoQing'; ;return false;
          }
        document.querySelector("#Button5").onclick = function () {
            if (window.confirm('确定要点击XX吗？'))
                window.location.href='/DaiDing'; ;return false;
          }
        document.querySelector("#Button6").onclick = function () {
            if (window.confirm('确定要点击XX吗？'))
                window.location.href='/DaiDing'; ;return false;
          }
        document.querySelector("#Button7").onclick = function () {
            if (window.confirm('确定要点击XX吗？'))
                window.location.href='/DaiDing'; ;return false;
          }
        document.querySelector("#Button8").onclick = function () {
            if (window.confirm('确定要点击XX吗？'))
                window.location.href='/DaiDing'; ;return false;
          }
    }
            </script>
        </body>
    </html>
    )
    res.SetBodyText(主界面网页源码), res.status := 200
}


; ================== 以下是脚本所用的函数类库 ==================

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


; https://www.autohotkey.com/boards/viewtopic.php?t=4890
; https://github.com/Skiouros/AHKhttp/blob/master/AHKhttp.ahk
class Uri
{
    Decode(str) {
        Loop
            If RegExMatch(str, "i)(?<=%)[\da-f]{1,2}", hex)
                StringReplace, str, str, `%%hex%, % Chr("0x" . hex), All
            Else Break
        Return, str
    }

    Encode(str) {
        f = %A_FormatInteger%
        SetFormat, Integer, Hex
        If RegExMatch(str, "^\w+:/{0,2}", pr)
            StringTrimLeft, str, str, StrLen(pr)
        StringReplace, str, str, `%, `%25, All
        Loop
            If RegExMatch(str, "i)[^\w\.~%]", char)
                StringReplace, str, str, %char%, % "%" . Asc(char), All
            Else Break
        SetFormat, Integer, %f%
        Return, pr . str
    }
}

class HttpServer
{
    static servers := {}

    LoadMimes(file) {
        if (!FileExist(file))
            return false

        FileRead, data, % file
        types := StrSplit(data, "`n")
        this.mimes := {}
        for i, data in types {
            info := StrSplit(data, " ")
            type := info.Remove(1)
            ; Seperates type of content and file types
            info := StrSplit(LTrim(SubStr(data, StrLen(type) + 1)), " ")

            for i, ext in info {
                this.mimes[ext] := type
            }
        }
        return true
    }

    GetMimeType(file) {
        default := "text/plain"
        if (!this.mimes)
            return default

        SplitPath, file,,, ext
        type := this.mimes[ext]
        if (!type)
            return default
        return type
    }

    ServeFile(ByRef response, file) {
        f := FileOpen(file, "r")
        length := f.RawRead(data, f.Length)
        f.Close()

        response.SetBody(data, length)
        res.headers["Content-Type"] := this.GetMimeType(file)
    }

    SetPaths(paths) {
        this.paths := paths
    }

    Handle(ByRef request) {
        response := new HttpResponse()
        if (!this.paths[request.path]) {
            func := this.paths["404"]
            response.status := 404
            if (func)
                func.(request, response, this)
            return response
        } else {
            this.paths[request.path].(request, response, this)
        }
        return response
    }

    Serve(port) {
        this.port := port
        HttpServer.servers[port] := this

        AHKsock_Listen(port, "HttpHandler")
    }
}

HttpHandler(sEvent, iSocket = 0, sName = 0, sAddr = 0, sPort = 0, ByRef bData = 0, bDataLength = 0) {
    static sockets := {}

    if (!sockets[iSocket]) {
        sockets[iSocket] := new Socket(iSocket)
        AHKsock_SockOpt(iSocket, "SO_KEEPALIVE", true)
    }
    socket := sockets[iSocket]

    if (sEvent == "DISCONNECTED") {
        socket.request := false
        sockets[iSocket] := false
    } else if (sEvent == "SEND") {
        if (socket.TrySend()) {
            socket.Close()
        }

    } else if (sEvent == "RECEIVED") {
        server := HttpServer.servers[sPort]

        text := StrGet(&bData, "UTF-8")

        ; New request or old?
        if (socket.request) {
            ; Get data and append it to the existing request body
            socket.request.bytesLeft -= StrLen(text)
            socket.request.body := socket.request.body . text
            request := socket.request
        } else {
            ; Parse new request
            request := new HttpRequest(text)

            length := request.headers["Content-Length"]
            request.bytesLeft := length + 0

            if (request.body) {
                request.bytesLeft -= StrLen(request.body)
            }
        }

        if (request.bytesLeft <= 0) {
            request.done := true
        } else {
            socket.request := request
        }

        if (request.done || request.IsMultipart()) {
            response := server.Handle(request)
            if (response.status) {
                socket.SetData(response.Generate())
            }
        }
        if (socket.TrySend()) {
            if (!request.IsMultipart() || request.done) {
                socket.Close()
            }
        }    

    }
}

class HttpRequest
{
    __New(data = "") {
        if (data)
            this.Parse(data)
    }

    GetPathInfo(top) {
        results := []
        while (pos := InStr(top, " ")) {
            results.Insert(SubStr(top, 1, pos - 1))
            top := SubStr(top, pos + 1)
        }
        this.method := results[1]
        this.path := Uri.Decode(results[2])
        this.protocol := top
    }

    GetQuery() {
        pos := InStr(this.path, "?")
        query := StrSplit(SubStr(this.path, pos + 1), "&")
        if (pos)
            this.path := SubStr(this.path, 1, pos - 1)

        this.queries := {}
        for i, value in query {
            pos := InStr(value, "=")
            key := SubStr(value, 1, pos - 1)
            val := SubStr(value, pos + 1)
            this.queries[key] := val
        }
    }

    Parse(data) {
        this.raw := data
        data := StrSplit(data, "`n`r")
        headers := StrSplit(data[1], "`n")
        this.body := LTrim(data[2], "`n")

        this.GetPathInfo(headers.Remove(1))
        this.GetQuery()
        this.headers := {}

        for i, line in headers {
            pos := InStr(line, ":")
            key := SubStr(line, 1, pos - 1)
            val := Trim(SubStr(line, pos + 1), "`n`r ")

            this.headers[key] := val
        }
    }

    IsMultipart() {
        length := this.headers["Content-Length"]
        expect := this.headers["Expect"]

        if (expect = "100-continue" && length > 0)
            return true
        return false
    }
}

class HttpResponse
{
    __New() {
        this.headers := {}
        this.status := 0
        this.protocol := "HTTP/1.1"

        this.SetBodyText("")
    }

    Generate() {
        FormatTime, date,, ddd, d MMM yyyy HH:mm:ss
        this.headers["Date"] := date

        headers := this.protocol . " " . this.status . "`r`n"
        for key, value in this.headers {
            headers := headers . key . ": " . value . "`r`n"
        }
        headers := headers . "`r`n"
        length := this.headers["Content-Length"]

        buffer := new Buffer((StrLen(headers) * 2) + length)
        buffer.WriteStr(headers)

        buffer.Append(this.body)
        buffer.Done()

        return buffer
    }

    SetBody(ByRef body, length) {
        this.body := new Buffer(length)
        this.body.Write(&body, length)
        this.headers["Content-Length"] := length
    }

    SetBodyText(text) {
        this.body := Buffer.FromString(text)
        this.headers["Content-Length"] := this.body.length
    }


}

class Socket
{
    __New(socket) {
        this.socket := socket
    }

    Close(timeout = 5000) {
        AHKsock_Close(this.socket, timeout)
    }

    SetData(data) {
        this.data := data
    }

    TrySend() {
        if (!this.data || this.data == "")
            return false

        p := this.data.GetPointer()
        length := this.data.length

        this.dataSent := 0
        loop {
            if ((i := AHKsock_Send(this.socket, p, length - this.dataSent)) < 0) {
                if (i == -2) {
                    return
                } else {
                    ; Failed to send
                    return
                }
            }

            if (i < length - this.dataSent) {
                this.dataSent += i
            } else {
                break
            }
        }
        this.dataSent := 0
        this.data := ""

        return true
    }
}

class Buffer
{
    __New(len) {
        this.SetCapacity("buffer", len)
        this.length := 0
    }

    FromString(str, encoding = "UTF-8") {
        length := Buffer.GetStrSize(str, encoding)
        buffer := new Buffer(length)
        buffer.WriteStr(str)
        return buffer
    }

    GetStrSize(str, encoding = "UTF-8") {
        encodingSize := ((encoding="utf-16" || encoding="cp1200") ? 2 : 1)
        ; length of string, minus null char
        return StrPut(str, encoding) * encodingSize - encodingSize
    }

    WriteStr(str, encoding = "UTF-8") {
        length := this.GetStrSize(str, encoding)
        VarSetCapacity(text, length)
        StrPut(str, &text, encoding)

        this.Write(&text, length)
        return length
    }

    ; data is a pointer to the data
    Write(data, length) {
        p := this.GetPointer()
        DllCall("RtlMoveMemory", "uint", p + this.length, "uint", data, "uint", length)
        this.length += length
    }

    Append(ByRef buffer) {
        destP := this.GetPointer()
        sourceP := buffer.GetPointer()

        DllCall("RtlMoveMemory", "uint", destP + this.length, "uint", sourceP, "uint", buffer.length)
        this.length += buffer.length
    }

    GetPointer() {
        return this.GetAddress("buffer")
    }

    Done() {
        this.SetCapacity("buffer", this.length)
    }
}


; https://github.com/jleb/AHKsock/blob/master/AHKsock.ahk
/*! TheGood
    AHKsock - A simple AHK implementation of Winsock.
    http://www.autohotkey.com/forum/viewtopic.php?p=355775
    Last updated: January 19, 2011
    
FUNCTION LIST:

________________________________________
AHKsock_Listen(sPort, sFunction = False)

Tells AHKsock to listen on the port in sPort, and call the function in sFunction when events occur. If sPort is a port on
which AHKsock is already listening, the action taken depends on sFunction:
    - If sFunction is False, AHKsock will stop listening on the port in sPort.
    - If sFunction is "()", AHKsock will return the name of the current function AHKsock calls when
      a client connects on the port in sPort.
    - If sFunction is a valid function, AHKsock will set that function as the new function to call
      when a client connects on the port in sPort.

Returns blank on success. On failure, it returns one of the following positive integer:
    2: sFunction is not a valid function.
    3: The WSAStartup() call failed. The error is in ErrorLevel.
    4: The Winsock DLL does not support version 2.2.
    5: The getaddrinfo() call failed. The error is in ErrorLevel.
    6: The socket() call failed. The error is in ErrorLevel.
    7: The bind() call failed. The error is in ErrorLevel.
    8: The WSAAsyncSelect() call failed. The error is in ErrorLevel.
    9: The listen() call failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

See the section titled "STRUCTURE OF THE EVENT-HANDLING FUNCTION AND MORE INFO ABOUT SOCKETS" for more info about how the
function in sFunction interacts with AHKsock.

________________________________________
AHKsock_Connect(sName, sPort, sFunction)

Tells AHKsock to connect to the hostname or IP address in sName on the port in sPort, and call the function in sFunction
when events occur.

Although the function will return right away, the connection attempt will still be in progress. Once the connection attempt
is over, successful or not, sFunction will receive the CONNECTED event. Note that it is important that once AHKsock_Connect
returns, the current thread must stay (or soon after must become) interruptible so that sFunction can be called once the
connection attempt is over.

AHKsock_Connect can only be called again once the previous connection attempt is over. To check if AHKsock_Connect is ready
to make another connection attempt, you may keep polling it by calling AHKsock_Connect(0,0,0) until it returns False.

Returns blank on success. On failure, it returns one of the following positive integer:
    1: AHKsock_Connect is still processing a connection attempt. ErrorLevel contains the name and the port of that
       connection attempt, separated by a tab.
    2: sFunction is not a valid function.
    3: The WSAStartup() call failed. The error is in ErrorLevel.
    4: The Winsock DLL does not support version 2.2.
    5: The getaddrinfo() call failed. The error is in ErrorLevel.
    6: The socket() call failed. The error is in ErrorLevel.
    7: The WSAAsyncSelect() call failed. The error is in ErrorLevel.
    8: The connect() call failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

See the section titled "STRUCTURE OF THE EVENT-HANDLING FUNCTION AND MORE INFO ABOUT SOCKETS" for more info about how the
function in sFunction interacts with AHKsock.

_______________________________________
AHKsock_Send(iSocket, ptrData, iLength)

Sends the data of length iLength to which ptrData points to the connected socket in iSocket.

Returns the number of bytes sent on success. This can be less than the number requested to be sent in the iLength parameter,
i.e. between 1 and iLength. This would occur if no buffer space is available within the transport system to hold the data to
be transmitted, in which case the number of bytes sent can be between 1 and the requested length, depending on buffer
availability on both the client and server computers. On failure, it returns one of the following negative integer:
    -1: WSAStartup hasn't been called yet.
    -2: Received WSAEWOULDBLOCK. This means that calling send() would have blocked the thread.
    -3: The send() call failed. The error is in ErrorLevel.
    -4: The socket specified in iSocket is not a valid socket. This means either that the socket in iSocket hasn't been
        created using AHKsock_Connect or AHKsock_Listen, or that the socket has already been destroyed.
    -5: The socket specified in iSocket is not cleared for sending. You haven't waited for the SEND event before calling,
        either ever, or not since you last received WSAEWOULDBLOCK.

You may start sending data to the connected socket in iSocket only after the socket's associated function receives the first
SEND event. Upon receiving the event, you may keep calling AHKsock_Send to send data until you receive the error -2, at
which point you must wait once again until you receive another SEND event before sending more data. Not waiting for the SEND
event results in receiving error -5 when calling AHKsock_Send.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

____________________________________________
AHKsock_ForceSend(iSocket, ptrData, iLength)

This function is exactly the same as AHKsock_Send, but with three differences:
    - If only part of the data could be sent, it will automatically keep trying to send the remaining part.
    - If it receives WSAEWOULDBLOCK, it will wait for the socket's SEND event and try sending the data again.
    - If the data buffer to send is larger than the socket's send buffer size, it will automatically send the data in
      smaller chunks in order to avoid a performance hit. See http://support.microsoft.com/kb/823764 for more info.

Therefore, AHKsock_ForceSend will return only when all the data has been sent. Because this function relies on waiting for
the socket's SEND event before continuing to send data, it cannot be called in a critical thread. Also, for the same reason,
it cannot be called from a socket's associated function (not specifically iSocket's associated function, but any socket's
associated function).

Another limitation to consider when choosing between AHKsock_Send and AHKsock_ForceSend is that AHKsock_ForceSend will not
return until all the data has been sent (unless an error occurs). Although the script will still be responsive (new threads
will still be able to launch), the thread from which it was called will not resume until it returns. Therefore, if sending
a large amount of data, you should either use AHKsock_Send, or use AHKsock_ForceSend by feeding it smaller pieces of the
data, allowing you to update the GUI if necessary (e.g. a progress bar).

Returns blank on success, which means that all the data to which ptrData points of length iLength has been sent. On failure,
it returns one of the following negative integer:
    -1: WSAStartup hasn't been called yet.
    -3: The send() call failed. The error is in ErrorLevel.
    -4: The socket specified in iSocket is not a valid socket. This means either that the socket in iSocket hasn't been
        created using AHKsock_Connect or AHKsock_Listen, or that the socket has already been destroyed.
    -5: The current thread is critical.
    -6: The getsockopt() call failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

____________________________________________
AHKsock_Close(iSocket = -1, iTimeout = 5000)

Closes the socket in iSocket. If no socket is specified, AHKsock_Close will close all the sockets on record, as well as
terminate use of the Winsock 2 DLL (by calling WSACleanup). If graceful shutdown cannot be attained after the timeout
specified in iTimeout (in milliseconds), it will perform a hard shutdown before calling WSACleanup to free resources. See
the section titled "NOTES ON CLOSING SOCKETS AND AHKsock_Close" for more information.

Returns blank on success. On failure, it returns one of the following positive integer:
    1: The shutdown() call failed. The error is in ErrorLevel. AHKsock_Close forcefully closed the socket and freed the
       associated resources.

Note that when AHKsock_Close is called with no socket specified, it will never return an error.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

___________________________________________________________
AHKsock_GetAddrInfo(sHostName, ByRef sIPList, bOne = False)

Retrieves the list of IP addresses that correspond to the hostname in sHostName. The list is contained in sIPList, delimited
by newline characters. If bOne is True, only one IP (the first one) will be returned.

Returns blank on success. On failure, it returns one of the following positive integer:
    1: The WSAStartup() call failed. The error is in ErrorLevel.
    2: The Winsock DLL does not support version 2.2.
    3: Received WSAHOST_NOT_FOUND. No such host is known.
    4: The getaddrinfo() call failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

_________________________________________________________________________
AHKsock_GetNameInfo(sIP, ByRef sHostName, sPort = 0, ByRef sService = "")

Retrieves the hostname that corresponds to the IP address in sIP. If a port in sPort is supplied, it also retrieves the
service that corresponds to the port in sPort.

Returns blank on success. On failure, it returns on of the following positive integer:
    1: The WSAStartup() call failed. The error is in ErrorLevel.
    2: The Winsock DLL does not support version 2.2.
    3: The IP address supplied in sIP is invalid.
    4: The getnameinfo() call failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

______________________________________________
AHKsock_SockOpt(iSocket, sOption, iValue = -1)

Retrieves or sets a socket option. Supported options are:
    SO_KEEPALIVE: Enable/Disable sending keep-alives. iValue must be True/False to enable/disable. Disabled by default.
    SO_SNDBUF:    Total buffer space reserved for sends. Set iValue to 0 to completely disable the buffer. Default is 8 KB.
    SO_RCVBUF:    Total buffer space reserved for receives. Default is 8 KB.
    TCP_NODELAY:  Enable/Disable the Nagle algorithm for send coalescing. Set iValue to True to disable the Nagle algorithm,
                  set iValue to False to enable the Nagle algorithm, which is the default.

It is usually best to leave these options to their default (especially the Nagle algorithm). Only change them if you
understand the consequences. See MSDN for more information on those options.

If iValue is specified, it sets the option to iValue and returns blank on success. If iValue is left as -1, it returns the
value of the option specified. On failure, it returns one of the following negative integer:
    -1: The getsockopt() failed. The error is in ErrorLevel.
    -2: The setsockopt() failed. The error is in ErrorLevel.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

_______________________________________
AHKsock_Settings(sSetting, sValue = "")

Changes the AHKsock setting in sSetting to sValue. If sValue is blank, the current value for that setting is returned. If
sValue is the word "Reset", the setting is restored to its default value. The possible settings are:
    Message: Determines the Windows message numbers used to monitor network events. The message number in iMessage and the
             next number will be used. Default value is 0x8000. For example, calling AHKsock_Settings("Message", 0x8005)
             will cause AHKsock to use 0x8005 and 0x8006 to monitor network events.
    Buffer:  Determines the size of the buffer (in bytes) used when receiving data. This is thus the maximum size of bData
             when the RECEIVED event is raised. If the data received is more than the buffer size, multiple recv() calls
             (and thus multiple RECEIVED events) will be needed. Note that you shouldn't use this setting as a means of
             delimiting frames. See the "NOTES ON RECEIVING AND SENDING DATA" section for more information about receiving
             and sending data. Default value is 64 KB, which is the maximum for TCP.

If you do call AHKsock_Settings to change the values from their default ones, it is best to do so at the beginning of the
script. The message number used cannot be changed as long as there are active connections.

______________________________________
AHKsock_ErrorHandler(sFunction = """")

Sets the function in sFunction to be the new error handler. If sFunction is left at its default value, it returns the name
of the current error handling function.

An error-handling function is optional, but may be useful when troubleshooting applications. The function will be called
anytime there is an error that arises in a thread which wasn't called by the user but by the receival of a Windows message
which was registered using OnMessage.

The function in sFunction must be of the following format:
MyErrorHandler(iError, iSocket)

The possible values for iError are:
     1: The connect() call failed. The error is in ErrorLevel.
     2: The WSAAsyncSelect() call failed. The error is in ErrorLevel.
     3: The socket() call failed. The error is in ErrorLevel.
     4: The WSAAsyncSelect() call failed. The error is in ErrorLevel.
     5: The connect() call failed. The error is in ErrorLevel.
     6: FD_READ event received with an error. The error is in ErrorLevel. The socket is in iSocket.
     7: The recv() call failed. The error is in ErrorLevel. The socket is in iSocket.
     8: FD_WRITE event received with an error. The error is in ErrorLevel. The socket is in iSocket.
     9: FD_ACCEPT event received with an error. The error is in ErrorLevel. The socket is in iSocket.
    10: The accept() call failed. The error is in ErrorLevel. The listening socket is in iSocket.
    11: The WSAAsyncSelect() call failed. The error is in ErrorLevel. The listening socket is in iSocket.
    12: The listen() call failed. The error is in ErrorLevel. The listening socket is in iSocket.
    13: The shutdown() call failed. The error is in ErrorLevel. The socket is in iSocket.

For the failures which affect ErrorLevel, ErrorLevel will contain either the reason the DllCall itself failed (ie. -1, -2,
An, etc... as laid out in the AHK docs for DllCall) or the Windows Sockets Error Code as defined at:
http://msdn.microsoft.com/en-us/library/ms740668

__________________________________________________________________
NOTES ON SOCKETS AND THE STRUCTURE OF THE EVENT-HANDLING FUNCTION:

The functions used in the sFunction parameter of AHKsock_Listen and AHKsock_Connect must be of the following format:

MyFunction(sEvent, iSocket = 0, sName = 0, sAddr = 0, sPort = 0, ByRef bData = 0, bDataLength = 0)

The variable sEvent contains the event for which MyFunction was called. The event raised is associated with one and only one
socket; the one in iSocket. The meaning of the possible events that can occur depend on the type of socket involved. AHKsock
deals with three different types of sockets:
    - Listening sockets: These sockets are created by a call to AHKsock_Listen. All they do is wait for clients to request
      a connection. These sockets will never appear as the iSocket parameter because requests for connections are
      immediately accepted, and MyFunction immediately receives the ACCEPTED event with iSocket set to the accepted socket.
    - Accepted sockets: These sockets are created once a listening socket receives an incoming connection attempt from a
      client and accepts it. They are thus the sockets that servers use to communicate with clients.
    - Connected sockets: These sockets are created by a successful call to AHKsock_Connect. These are the sockets that
      clients use to communicate with servers.

More info about sockets:
    - You may have multiple client sockets connecting to the same listening socket (ie. on the same port).
    - You may have multiple listening sockets for different ports.
    - You cannot have more than one listening socket for the same port (or you will receive a bind() error).
    - Every single connection between a client and a server will have its own client socket on the client side, and its own
      server (accepted) socket on the server side.

For all of the events that the event-handling function receives,
    - sEvent contains the event that occurred (as described below),
    - iSocket contains the socket on which the event occurred,
    - sName contains a value which depends on the type of socket in iSocket:
        - If the socket is an accepted socket, sName is empty.
        - If the socket is a connected socket, sName is the same value as the sName parameter that was used when
          AHKsock_Connect was called to create the socket. Since AHKsock_Connect accepts both hostnames and IP addresses,
          sName may contain either.
    - sAddr contains the IP address of the socket's endpoint (i.e. the peer's IP address). This means that if the socket in
      iSocket is an accepted socket, sAddr contains the IP address of the client. Conversely, if it is a connected socket,
      sAddr contains the server's IP.
    - sPort contains the server port on which the connection was accepted.

Obviously, if your script only calls AHKsock_Listen (acting as a server) or AHKsock_Connect (acting as a client) you don't
need to check if the socket in iSocket is an accepted socket or a connected socket, since it can only be one or the other.
But if you do call both AHKsock_Listen and AHKsock_Connect with both of them using the same function (e.g. MyFunction), then
you will need to check what type of socket iSocket is by checking the sName parameter.

Of course, it would be easier to simply have two different functions, for example, MyFunction1 and MyFunction2, with one
handling the server part and the other handling the client part so that you don't need to check what type of socket iSocket
is when each function is called. However, this might not be necessary if both server and client are "symmetrical" (i.e. the
conversation doesn't actually change whether or not we're on the server side or the client side). See Example 3 for an
example of this, where only one function is used for both server and client sockets.

The variable sEvent can be one of the following values if iSocket is an accepted socket:
    sEvent =      Event Description:
    ACCEPTED      A client connection was accepted (see the "Listening sockets" section above for more details).
    CONNECTED     <Does not occur on accepted sockets>
    DISCONNECTED  The client disconnected (see AHKsock_Close for more details).
    SEND          You may now send data to the client (see AHKsock_Send for more details).
    RECEIVED      You received data from the client. The data received is in bData and the length is in bDataLength.
    SENDLAST      The client is disconnecting. This is your last chance to send data to it. Once this function returns,
                  disconnection will occur. This event only occurs on the side which did not initiate shutdown (see
                  AHKsock_Close for more details).

The variable sEvent can be one of the following values if iSocket is a connected socket:
    sEvent =      Event Description:
    ACCEPTED      <Does not occur on connected sockets>
    CONNECTED     The connection attempt initiated by calling AHKsock_Connect has completed (see AHKsock_Connect for more
                  details). If it was successful, iSocket will equal the client socket. If it failed, iSocket will equal -1.
                  To get the error code that the failure returned, set an error handling function with AHKsock_ErrorHandler,
                  and read ErrorLevel when iError is equal to 1.
    DISCONNECTED  The server disconnected (see AHKsock_Close for more details).
    SEND          You may now send data to the server (see AHKsock_Send for more details).
    RECEIVED      You received data from the server. The data received is in bData and the length is in bDataLength.
    SENDLAST      The server is disconnecting. This is your last chance to send data to it. Once this function returns,
                  disconnection will occur. This event only occurs on the side which did not initiate shutdown (see 
                  AHKsock_Close for more details).

More information: The event-handling functions described in here are always called with the Critical setting on. This is
necessary in order to ensure proper processing of messages. Note that as long as the event-handling function does not
return, AHKsock cannot process other network messages. Although messages are buffered, smooth operation might suffer when
letting the function run for longer than it should.

___________________________________________
NOTES ON CLOSING SOCKETS AND AHKsock_Close:

There are a few things to note about the AHKsock_Close function. The most important one is this: because the OnExit
subroutine cannot be made interruptible if running due to a call to Exit/ExitApp, AHKsock_Close will not be able to execute
a graceful shutdown if it is called from there. 

A graceful shutdown refers to the proper way of closing a TCP connection. It consists of an exchange of special TCP messages
between the two endpoints to acknowledge that the connection is about to close. It also fires the SENDLAST event in the
socket's associated function to notify that this is the last chance it will have to send data before disconnection. Note
that listening sockets cannot (and therefore do not need to) be gracefully shutdown as it is not an end-to-end connection.
(In practice, you will never have to manually call AHKsock_Close on a listening socket because you do not have access to
them. The socket is closed when you stop listening by calling AHKsock_Listen with no specified value for the second
parameter.)

In order to allow the socket(s) connection(s) to gracefully shutdown (which is always preferable), AHKsock_Close must be
called in a thread which is, or can be made, interruptible. If it is called with a specified socket in iSocket, it will
initiate a graceful shutdown for that socket alone. If it is called with no socket specified, it will initiate a graceful
shutdown for all connected/accepted sockets, and once done, deregister itself from the Windows Sockets implementation and
allow the implementation to free any resources allocated for Winsock (by calling WSACleanup). In that case, if any
subsequent AHKsock function is called, Winsock will automatically be restarted.

Therefore, before exiting your application, AHKsock_Close must be called at least once with no socket specified in order to
free Winsock resources. This can be done in the OnExit subroutine, either if you do not wish to perform a graceful shutdown
(which is not recommended), or if you have already gracefully shutdown all the sockets individually before calling
Exit/ExitApp. Of course, it doesn't have to be done in the OnExit subroutine and can be done anytime before (which is the
recommended method because AHKsock will automatically gracefully shutdown all the sockets on record).

This behaviour has a few repercussions on your application's design. If the only way for the user to terminate your
application is through AHK's default Exit menu item in the tray menu, then upon selecting the Exit menu item, the OnExit sub
will fire, and your application will not have a chance to gracefully shutdown connected sockets. One way around this is to
add your own menu item which will in turn call AHKsock_Close with no socket specified before calling ExitApp to enter the
OnExit sub. See AHKsock Example 1 for an example of this.

This is how the graceful shutdown process occurs between two connected peers:
    a> Once one of the peers (it may be the server of the client) is done sending all its data, it calls AHKsock_Close to
       shutdown the socket. (It is not a good idea to have the last peer receiving data call AHKsock_Close. This will result
       in AHKsock_Send errors on the other peer if more data needs to be sent.) In the next steps, we refer to the peer that
       first calls AHKsock_Close as the invoker, and the other peer simply as the peer.
    b> The peer receives the invoker's intention to close the connection and is given one last chance to send any remaining
       data. This is when the peer's socket's associated function receives the SENDLAST event.
    c> Once the peer is done sending any remaining data (if any), it also calls AHKsock_Close on that same socket to shut it
       down, and then close the socket for good. This happens once the peer's function that received the SENDLAST event
       returns from the event. At this point, the peer's socket's associated function receives the DISCONNECTED event.
    d> This happens in parallel with c>. After the invoker receives the peer's final data (if any), as well as notice that
       the peer has also called AHKsock_Close on the socket, the invoker finally also closes the socket for good. At this
       point, the socket's associated function also receives the DISCONNECTED event.

When AHKsock_Close is called with no socket specified, this process occurs (in parallel) for every connected socket on
record.

____________________________________
NOTES ON RECEIVING AND SENDING DATA:

It's important to understand that AHKsock uses the TCP protocol, which is a stream protocol. This means that the data
received comes as a stream, with no apparent boundaries (i.e. frames or packets). For example, if a peer sends you a string,
it's possible that half the string is received in one RECEIVED event and the other half is received in the next. Of course,
the smaller the string, the less likely this happens. Conversely, the larger the string, the more likely this will occur.

Similarly, calling AHKsock_Send will not necessarily send the data right away. If multiple AHKsock_Send calls are issued,
Winsock might, under certain conditions, wait and accumulate data to send before sending it all at once. This process is
called coalescing. For example, if you send two strings to your peer by using two individual AHKsock_Send calls, the peer
will not necessarily receive two consecutive RECEIVED events for each string, but might instead receive both strings through
a single RECEIVED event.

One efficient method of receiving data as frames is to use length-prefixing. Length-prefixing means that before sending a
frame of variable length to your peer, you first tell it how many bytes will be in the frame. This way, your peer can
divide the received data into frames that can be individually processed. If it received less than a frame, it can store the
received data and wait for the remaining data to arrive before processing the completed frame with the length specified.
This technique is used in in AHKsock Example 3, where peers send each other strings by first declaring how long the string
will be (see the StreamProcessor function of Example 3).

____________________________________
NOTES ON TESTING A STREAM PROCESSOR:

As you write applications that use length-prefixing as described above, you might find it hard to test their ability to
properly cut up and/or put together the data into frames when testing them on the same machine or on a LAN (because the
latency is too low and it is thus harder to stress the connection).

In this case, what you can do to properly test them is to uncomment the comment block in AHKsock_Send, which will sometimes
purposely fail to send part of the data requested. This will allow you to simulate what could happen on a connection going
through the Internet. You may change the probability of failure by changing the number in the If statement.

If your application can still work after uncommenting the block, then it is a sign that it is properly handling frames split
across multiple RECEIVED events. This would also demonstrate your application's ability to cope with partially sent data.
*/

/****************\
 Main functions  |
               */

AHKsock_Listen(sPort, sFunction = False) {
    
    ;Check if there is already a socket listening on this port
    If (sktListen := AHKsock_Sockets("GetSocketFromNamePort", A_Space, sPort)) {
        
        ;Check if we're stopping the listening
        If Not sFunction {
            AHKsock_Close(sktListen) ;Close the socket
        
        ;Check if we're retrieving the current function
        } Else If (sFunction = "()") {
            Return AHKsock_Sockets("GetFunction", sktListen)
        
        ;Check if it's a different function
        } Else If (sFunction <> AHKsock_Sockets("GetFunction", sktListen))
            AHKsock_Sockets("SetFunction", sktListen, sFunction) ;Update it
        
        Return ;We're done
    }
    
    ;Make sure we even have a function
    If Not IsFunc(sFunction)
        Return 2 ;sFunction is not a valid function.
    
    ;Make sure Winsock has been started up
    If (i := AHKsock_Startup())
        Return (i = 1) ? 3 ;The WSAStartup() call failed. The error is in ErrorLevel.
                       : 4 ;The Winsock DLL does not support version 2.2.
    
    ;Resolve the local address and port to be used by the server
    VarSetCapacity(aiHints, 16 + 4 * A_PtrSize, 0)
    NumPut(1, aiHints,  0, "Int") ;ai_flags = AI_PASSIVE
    NumPut(2, aiHints,  4, "Int") ;ai_family = AF_INET
    NumPut(1, aiHints,  8, "Int") ;ai_socktype = SOCK_STREAM
    NumPut(6, aiHints, 12, "Int") ;ai_protocol = IPPROTO_TCP
    iResult := DllCall("Ws2_32\GetAddrInfo", "Ptr", 0, "Ptr", &sPort, "Ptr", &aiHints, "Ptr*", aiResult)
    If (iResult != 0) Or ErrorLevel { ;Check for error
        ErrorLevel := ErrorLevel ? ErrorLevel : iResult
        Return 5 ;The getaddrinfo() call failed. The error is in ErrorLevel.
    }
    
    sktListen := -1 ;INVALID_SOCKET
    sktListen := DllCall("Ws2_32\socket", "Int", NumGet(aiResult+0, 04, "Int")
                                        , "Int", NumGet(aiResult+0, 08, "Int")
                                        , "Int", NumGet(aiResult+0, 12, "Int"), "Ptr")
    If (sktListen = -1) Or ErrorLevel { ;Check for INVALID_SOCKET
        sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
        ErrorLevel := sErrorLevel
        Return 6 ;The socket() call failed. The error is in ErrorLevel.
    }
    
    ;Setup the TCP listening socket
    iResult := DllCall("Ws2_32\bind", "Ptr", sktListen, "Ptr", NumGet(aiResult+0, 16 + 2 * A_PtrSize), "Int", NumGet(aiResult+0, 16, "Ptr"))
    If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
        sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        DllCall("Ws2_32\closesocket",  "Ptr", sktListen)
        DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
        ErrorLevel := sErrorLevel
        Return 7 ;The bind() call failed. The error is in ErrorLevel.
    }
    
    DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
    
    ;Add socket to array with A_Space for Name and IP to indicate that it's a listening socket
    AHKsock_Sockets("Add", sktListen, A_Space, A_Space, sPort, sFunction)
    
    ;We must now actually register the socket
    If AHKsock_RegisterAsyncSelect(sktListen) {
        sErrorLevel := ErrorLevel
        DllCall("Ws2_32\closesocket", "Ptr", sktListen)
        AHKsock_Sockets("Delete", sktListen) ;Remove from array
        ErrorLevel := sErrorLevel
        Return 8 ;The WSAAsyncSelect() call failed. The error is in ErrorLevel.
    }
    
    ;Start listening for incoming connections
    iResult := DllCall("Ws2_32\listen", "Ptr", sktListen, "Int", 0x7FFFFFFF) ;SOMAXCONN
    If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
        sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        DllCall("Ws2_32\closesocket", "Ptr", sktListen)
        AHKsock_Sockets("Delete", sktListen) ;Remove from array
        ErrorLevel := sErrorLevel
        Return 9 ;The listen() call failed. The error is in ErrorLevel.
    }
}

AHKsock_Connect(sName, sPort, sFunction) {
    Static aiResult, iPointer, bProcessing, iMessage
    Static sCurName, sCurPort, sCurFunction, sktConnect
    
    ;Check if it's just to inquire whether or not a call is possible
    If (Not sName And Not sPort And Not sFunction)
        Return bProcessing
    
    ;Check if we're busy
    If bProcessing And (sFunction != iMessage) {
        ErrorLevel := sCurName A_Tab sCurPort
        Return 1 ;AHKsock_Connect is still processing a connection attempt. ErrorLevel contains the name and the port,
                 ;delimited by a tab.
    } Else If bProcessing { ;sFunction = iMessage. The connect operation has finished.
        
        ;Check if it was successful
        If (i := sPort >> 16) {
            
            ;Close the socket that failed
            DllCall("Ws2_32\closesocket", "Ptr", sktConnect)
            
            ;Get the next pointer. ai_next
            iPointer := NumGet(iPointer+0, 16 + 3 * A_PtrSize)
            
            ;Check if we reached the end of the linked structs
            If (iPointer = 0) {
                
                ;We can now free the chain of addrinfo structs
                DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
                
                ;This is to ensure that the user can call AHKsock_Connect() right away upon receiving the message.
                bProcessing := False
                
                ;Raise an error (can't use Return 1 because we were called asynchronously)
                ErrorLevel := i
                AHKsock_RaiseError(1) ;The connect() call failed. The error is in ErrorLevel.
                
                ;Call the function to signal that connection failed
                If IsFunc(sCurFunction)
                    %sCurFunction%("CONNECTED", -1, sCurName, 0, sCurPort)
                
                Return
            }
            
        } Else { ;Successful connection!
            
            ;Get the IP we successfully connected to
            sIP := DllCall("Ws2_32\inet_ntoa", "UInt", NumGet(NumGet(iPointer+0, 16 + 2 * A_PtrSize)+4, 0, "UInt"), "AStr")
            
            ;We can now free the chain of ADDRINFO structs
            DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
            
            ;Add socket to array
            AHKsock_Sockets("Add", sktConnect, sCurName, sIP, sCurPort, sCurFunction)
            
            ;This is to ensure that the user can call AHKsock_Connect() right away upon receiving the message.
            bProcessing := False
            
            ;Do this small bit in Critical so that AHKsock_AsyncSelect doesn't receive
            ;any FD messages before we call the user function
            Critical
            
            ;We must now actually register the socket
            If AHKsock_RegisterAsyncSelect(sktConnect) {
                sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
                DllCall("Ws2_32\closesocket", "Ptr", sktConnect)
                AHKsock_Sockets("Delete", sktConnect) ;Remove from array
                ErrorLevel := sErrorLevel
                AHKsock_RaiseError(2) ;The WSAAsyncSelect() call failed. The error is in ErrorLevel.
                
                If IsFunc(sCurFunction) ;Call the function to signal that connection failed
                    %sCurFunction%("CONNECTED", -1, sCurName, 0, sCurPort)
                
            } Else If IsFunc(sCurFunction) ;Call the function to signal that connection was successful
                %sCurFunction%("CONNECTED", sktConnect, sCurName, sIP, sCurPort)
            
            Return
        }
        
    } Else { ;We were called
        
        ;Make sure we even have a function
        If Not IsFunc(sFunction)
            Return 2 ;sFunction is not a valid function.
        
        bProcessing := True ;Block future calls to AHKsock_Connect() until we're done
        
        ;Keep the values
        sCurName := sName
        sCurPort := sPort
        sCurFunction := sFunction
        
        ;Make sure Winsock has been started up
        If (i := AHKsock_Startup()) {
            bProcessing := False
            Return (i = 1) ? 3 ;The WSAStartup() call failed. The error is in ErrorLevel.
                           : 4 ;The Winsock DLL does not support version 2.2.
        }
        
        ;Resolve the server address and port    
        VarSetCapacity(aiHints, 16 + 4 * A_PtrSize, 0)
        NumPut(2, aiHints,  4, "Int") ;ai_family = AF_INET
        NumPut(1, aiHints,  8, "Int") ;ai_socktype = SOCK_STREAM
        NumPut(6, aiHints, 12, "Int") ;ai_protocol = IPPROTO_TCP
        iResult := DllCall("Ws2_32\GetAddrInfo", "Ptr", &sName, "Ptr", &sPort, "Ptr", &aiHints, "Ptr*", aiResult)
        If (iResult != 0) Or ErrorLevel { ;Check for error
            ErrorLevel := ErrorLevel ? ErrorLevel : iResult
            bProcessing := False
            Return 5 ;The getaddrinfo() call failed. The error is in ErrorLevel.
        }
        
        ;Start with the first struct
        iPointer := aiResult
    }
    
    ;Create a SOCKET for connecting to server
    sktConnect := DllCall("Ws2_32\socket", "Int", NumGet(iPointer+0, 04, "Int")
                                         , "Int", NumGet(iPointer+0, 08, "Int")
                                         , "Int", NumGet(iPointer+0, 12, "Int"), "Ptr")
    If (sktConnect = 0xFFFFFFFF) Or ErrorLevel { ;Check for INVALID_SOCKET
        sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
        bProcessing := False
        ErrorLevel := sErrorLevel
        If (sFunction = iMessage) { ;Check if we were called asynchronously
            AHKsock_RaiseError(3) ;The socket() call failed. The error is in ErrorLevel.
            
            ;Call the function to signal that connection failed
            If IsFunc(sCurFunction)
                %sCurFunction%("CONNECTED", -1)
        }
        Return 6 ;The socket() call failed. The error is in ErrorLevel.
    }
    
    ;Register the socket to know when the connect() function is done. FD_CONNECT = 16
    iMessage := AHKsock_Settings("Message") + 1
    If AHKsock_RegisterAsyncSelect(sktConnect, 16, "AHKsock_Connect", iMessage) {
        sErrorLevel := ErrorLevel
        DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
        DllCall("Ws2_32\closesocket",  "Ptr", sktConnect)
        bProcessing := False
        ErrorLevel := sErrorLevel
        If (sFunction = iMessage) { ;Check if we were called asynchronously
            AHKsock_RaiseError(4) ;The WSAAsyncSelect() call failed. The error is in ErrorLevel.
            
            ;Call the function to signal that connection failed
            If IsFunc(sCurFunction)
                %sCurFunction%("CONNECTED", -1)
        }
        Return 7 ;The WSAAsyncSelect() call failed. The error is in ErrorLevel.
    }
    
    ;Connect to server (the connect() call also implicitly binds the socket to any host address and any port)
    iResult := DllCall("Ws2_32\connect", "Ptr", sktConnect, "Ptr", NumGet(iPointer+0, 16 + 2 * A_PtrSize), "Int", NumGet(iPointer+0, 16))
    If ErrorLevel Or ((iResult = -1) And (AHKsock_LastError() != 10035)) { ;Check for any error other than WSAEWOULDBLOCK
        sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
        DllCall("Ws2_32\closesocket",  "Ptr", sktConnect)
        bProcessing := False
        ErrorLevel := sErrorLevel
        If (sFunction = iMessage) { ;Check if we were called asynchronously
            AHKsock_RaiseError(5) ;The connect() call failed. The error is in ErrorLevel.
            
            ;Call the function to signal that connection failed
            If IsFunc(sCurFunction)
                %sCurFunction%("CONNECTED", -1)
        }
        Return 8 ;The connect() call failed. The error is in ErrorLevel.
    }
}

AHKsock_Send(iSocket, ptrData = 0, iLength = 0) {
    
    ;Make sure the socket is on record. Fail-safe
    If Not AHKsock_Sockets("Index", iSocket)
        Return -4 ;The socket specified in iSocket is not a recognized socket.
    
    ;Make sure Winsock has been started up
    If Not AHKsock_Startup(1)
        Return -1 ;WSAStartup hasn't been called yet.
    
    ;Make sure the socket is cleared for sending
    If Not AHKsock_Sockets("GetSend", iSocket)
        Return -5 ;The socket specified in iSocket is not cleared for sending.
    
    /*! Uncomment this block to simulate the possibility of an incomplete send()
    Random, iRand, 1, 100
    If (iRand <= 30) { ;Probability of failure of 30%
        Random, iRand, 1, iLength - 1 ;Randomize how much of the data will not be sent
        iLength -= iRand
    }
    */
    
    iSendResult := DllCall("Ws2_32\send", "Ptr", iSocket, "Ptr", ptrData, "Int", iLength, "Int", 0)
    If (iSendResult = -1) And ((iErr := AHKsock_LastError()) = 10035) { ;Check specifically for WSAEWOULDBLOCK
        AHKsock_Sockets("SetSend", iSocket, False) ;Update socket's send status
        Return -2 ;Calling send() would have blocked the thread. Try again once you get the proper update.
    } Else If (iSendResult = -1) Or ErrorLevel {
        ErrorLevel := ErrorLevel ? ErrorLevel : iErr
        Return -3 ;The send() call failed. The error is in ErrorLevel.
    } Else Return iSendResult ;The send() operation was successful
}

AHKsock_ForceSend(iSocket, ptrData, iLength) {
    
    ;Make sure Winsock has been started up
    If Not AHKsock_Startup(1)
        Return -1 ;WSAStartup hasn't been called yet
    
    ;Make sure the socket is on record. Fail-safe
    If Not AHKsock_Sockets("Index", iSocket)
        Return -4
    
    ;Make sure that we're not in Critical, or we won't be able to wait for FD_WRITE messages
    If A_IsCritical
        Return -5
    
    ;Extra precaution to make sure FD_WRITE messages can make it
    Thread, Priority, 0
    
    ;We need to make sure not to fill up the send buffer in one call, or we'll get a performance hit.
    ;http://support.microsoft.com/kb/823764
    
    ;Get the socket's send buffer size
    If ((iMaxChunk := AHKsock_SockOpt(iSocket, "SO_SNDBUF")) = -1)
        Return -6
    
    ;Check if we'll be sending in chunks or not
    If (iMaxChunk <= 1) {
        
        ;We'll be sending as much as possible everytime!
        
        Loop { ;Keep sending the data until we're done or until an error occurs
            
            ;Wait until we can send data (ie. when FD_WRITE arrives)
            While Not AHKsock_Sockets("GetSend", iSocket)
                Sleep -1
            
            Loop { ;Keep sending the data until we get WSAEWOULDBLOCK or until an error occurs
                If ((iSendResult := AHKsock_Send(iSocket, ptrData, iLength)) < 0) {
                    If (iSendResult = -2) ;Check specifically for WSAEWOULDBLOCK
                        Break ;Calling send() would have blocked the thread. Break the loop and we'll try again after we
                              ;receive FD_WRITE
                    Else Return iSendResult ;Something bad happened with AHKsock_Send. Return the same value we got.
                } Else {
                    
                    ;AHKsock_Send was able to send bytes. Let's check if it sent only part of what we requested
                    If (iSendResult < iLength) ;Move the offset up by what we were able to send
                        ptrData += iSendResult, iLength -= iSendResult
                    Else Return ;We're done sending all the data
                }
            }
        }
    } Else {
        
        ;We'll be sending in chunks of just under the send buffer size to avoid the performance hit
        
        iMaxChunk -= 1 ;Reduce by 1 to be smaller than the send buffer
        Loop { ;Keep sending the data until we're done or until an error occurs
            
            ;Wait until we can send data (ie. when FD_WRITE arrives)
            While Not AHKsock_Sockets("GetSend", iSocket)
                Sleep -1
            
            ;Check if we have less than the max chunk to send
            If (iLength < iMaxChunk) {
                
                Loop { ;Keep sending the data until we get WSAEWOULDBLOCK or until an error occurs
                    ;Send using the traditional offset method
                    If ((iSendResult := AHKsock_Send(iSocket, ptrData, iLength)) < 0) {
                        If (iSendResult = -2) ;Check specifically for WSAEWOULDBLOCK
                            Break ;Calling send() would have blocked the thread. Break the loop and we'll try again after we
                                  ;receive FD_WRITE
                        Else Return iSendResult ;Something bad happened with AHKsock_Send. Return the same value we got.
                    } Else {
                        
                        ;AHKsock_Send was able to send bytes. Let's check if it sent only part of what we requested
                        If (iSendResult < iLength) ;Move the offset up by what we were able to send
                            ptrData += iSendResult, iLength -= iSendResult
                        Else Return ;We're done sending all the data
                    }
                }
            } Else {
                
                ;Send up to max chunk
                If ((iSendResult := AHKsock_Send(iSocket, ptrData, iMaxChunk)) < 0) {
                    If (iSendResult = -2) ;Check specifically for WSAEWOULDBLOCK
                        Continue ;Calling send() would have blocked the thread. Continue the loop and we'll try again after
                                 ;we receive FD_WRITE
                    Else Return iSendResult ;Something bad happened with AHKsock_Send. Return the same value we got.
                } Else ptrData += iSendResult, iLength -= iSendResult ;Move up offset by updating the pointer and length
            }
        }
    }
}

AHKsock_Close(iSocket = -1, iTimeout = 5000) {
    
    ;Make sure Winsock has been started up
    If Not AHKsock_Startup(1)
        Return ;There's nothing to close
    
    If (iSocket = -1) { ;We need to close all the sockets
        
        ;Check if we even have sockets to close
        If Not AHKsock_Sockets() {
            DllCall("Ws2_32\WSACleanup")
            AHKsock_Startup(2) ;Reset the value to show that we've turned off Winsock
            Return ;We're done!
        }
        
        ;Take the current time (needed for time-outing)
        iStartClose := A_TickCount
        
        Loop % AHKsock_Sockets() ;Close all sockets and cleanup
            AHKsock_ShutdownSocket(AHKsock_Sockets("GetSocketFromIndex", A_Index))
        
        ;Check if we're in the OnExit subroutine
        If Not A_ExitReason {
            
            A_IsCriticalOld := A_IsCritical
            
            ;Make sure we can still receive FD_CLOSE msgs
            Critical, Off
            Thread, Priority, 0
            
            ;We can try a graceful shutdown or wait for a timeout
            While (AHKsock_Sockets()) And (A_TickCount - iStartClose < iTimeout)
                Sleep, -1
            
            ;Restore previous Critical
            Critical, %A_IsCriticalOld%
        }
        
        /*! Used for debugging purposes only
        If (i := AHKsock_Sockets()) {
            If (i = 1)
                OutputDebug, % "Cleaning up now, with the socket " AHKsock_Sockets("GetSocketFromIndex", 1) " remaining..."
            Else {
                OutputDebug, % "Cleaning up now, with the following sockets remaining:"
                Loop % AHKsock_Sockets() {
                    OutputDebug, % AHKsock_Sockets("GetSocketFromIndex", A_Index)
                }
            }
        }
        */
        
        DllCall("Ws2_32\WSACleanup")
        AHKsock_Startup(2) ;Reset the value to show that we've turned off Winsock
        
    ;Close only one socket
    } Else If AHKsock_ShutdownSocket(iSocket) ;Error-checking
        Return 1 ;The shutdown() call failed. The error is in ErrorLevel.
}

AHKsock_GetAddrInfo(sHostName, ByRef sIPList, bOne = False) {
    
    ;Make sure Winsock has been started up
    If (i := AHKsock_Startup())
        Return i ;Return the same error (error 1 and 2)
    
    ;Resolve the address and port    
    VarSetCapacity(aiHints, 16 + 4 * A_PtrSize, 0)
    NumPut(2, aiHints,  4, "Int") ;ai_family = AF_INET
    NumPut(1, aiHints,  8, "Int") ;ai_socktype = SOCK_STREAM
    NumPut(6, aiHints, 12, "Int") ;ai_protocol = IPPROTO_TCP
    iResult := DllCall("Ws2_32\GetAddrInfo", "Ptr", &sHostName, "Ptr", 0, "Ptr", &aiHints, "Ptr*", aiResult)
    If (iResult = 11001) ;Check specifically for WSAHOST_NOT_FOUND since it's the most common error
        Return 3 ;Received WSAHOST_NOT_FOUND. No such host is known.
    Else If (iResult != 0) Or ErrorLevel { ;Check for any other error
        ErrorLevel := ErrorLevel ? ErrorLevel : iResult
        Return 4 ;The getaddrinfo() call failed. The error is in ErrorLevel.
    }
    
    If bOne
        sIPList := DllCall("Ws2_32\inet_ntoa", "UInt", NumGet(NumGet(aiResult+0, 16 + 2 * A_PtrSize)+4, 0, "UInt"), "AStr")
    Else {
        
        ;Start with the first addrinfo struct
        iPointer := aiResult, sIPList := ""
        While iPointer {
            s := DllCall("Ws2_32\inet_ntoa", "UInt", NumGet(NumGet(iPointer+0, 16 + 2 * A_PtrSize)+4, 0, "UInt"), "AStr")
            iPointer := NumGet(iPointer+0, 16 + 3 * A_PtrSize) ;Go to the next addrinfo struct
            sIPList .=  s (iPointer ? "`n" : "") ;Add newline only if it's not the last one
        }
    }
    
    ;We're done
    DllCall("Ws2_32\FreeAddrInfo", "Ptr", aiResult)
}

AHKsock_GetNameInfo(sIP, ByRef sHostName, sPort = 0, ByRef sService = "") {
    
    ;Make sure Winsock has been started up
    If (i := AHKsock_Startup())
        Return i ;Return the same error (error 1 and 2)
    
    ;Translate to IN_ADDR
    iIP := DllCall("Ws2_32\inet_addr", "AStr", sIP, "UInt")
    If (iIP = 0 Or iIP = 0xFFFFFFFF) ;Check for INADDR_NONE or INADDR_ANY
        Return 3 ;The IP address supplied in sIP is invalid.
    
    ;Construct a sockaddr struct
    VarSetCapacity(tSockAddr, 16, 0)
    NumPut(2,   tSockAddr, 0, "Short") ;ai_family = AF_INET
    NumPut(iIP, tSockAddr, 4, "UInt") ;Put in the IN_ADDR
    
    ;Fill in the port field if we're also looking up the service name
    If sPort           ;Translate to network byte order
        NumPut(DllCall("Ws2_32\htons", "UShort", sPort, "UShort"), tSockAddr, 2, "UShort")
    
    ;Prep vars
    VarSetCapacity(sHostName, 1025 * 2, 0) ;NI_MAXHOST
    If sPort
        VarSetCapacity(sService, 32 * 2, 0) ;NI_MAXSERV
    
    iResult := DllCall("Ws2_32\GetNameInfoW", "Ptr", &tSockAddr, "Int", 16, "Str", sHostName, "UInt", 1025 * 2
                                           , sPort ? "Str" : "UInt", sPort ? sService : 0, "UInt", 32 * 2, "Int", 0)
    If (iResult != 0) Or ErrorLevel {
        ErrorLevel := ErrorLevel ? ErrorLevel : DllCall("Ws2_32\WSAGetLastError")
        Return 4 ;The getnameinfo() call failed. The error is in ErrorLevel.
    }
}

AHKsock_SockOpt(iSocket, sOption, iValue = -1) {
    
    ;Prep variable
    VarSetCapacity(iOptVal, iOptValLength := 4, 0)
    If (iValue <> -1)
        NumPut(iValue, iOptVal, 0, "UInt")
    
    If (sOption = "SO_KEEPALIVE") {
        intLevel := 0xFFFF ;SOL_SOCKET
        intOptName := 0x0008 ;SO_KEEPALIVE
    } Else If (sOption = "SO_SNDBUF") {
        intLevel := 0xFFFF ;SOL_SOCKET
        intOptName := 0x1001 ;SO_SNDBUF
    } Else If (sOption = "SO_RCVBUF") {
        intLevel := 0xFFFF ;SOL_SOCKET
        intOptName := 0x1002 ;SO_SNDBUF
    } Else If (sOption = "TCP_NODELAY") {
        intLevel := 6 ;IPPROTO_TCP
        intOptName := 0x0001 ;TCP_NODELAY
    }
    
    ;Check if we're getting or setting
    If (iValue = -1) {
        iResult := DllCall("Ws2_32\getsockopt", "Ptr", iSocket, "Int", intLevel, "Int", intOptName
                                              , "UInt*", iOptVal, "Int*", iOptValLength)
        If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
            ErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
            Return -1
        } Else Return iOptVal
    } Else {
        iResult := DllCall("Ws2_32\setsockopt", "Ptr", iSocket, "Int", intLevel, "Int", intOptName
                                              , "Ptr", &iOptVal, "Int",  iOptValLength)
        If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
            ErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
            Return -2
        }
    }
}

/*******************\
 Support functions  |
                  */

AHKsock_Startup(iMode = 0) {
    Static bAlreadyStarted
    
    /*
    iMode = 0 ;Turns on WSAStartup()
    iMode = 1 ;Returns whether or not WSAStartup has been called
    iMode = 2 ;Resets the static variable to force another call next time iMode = 0
    */
    
    If (iMode = 2)
        bAlreadyStarted := False
    Else If (iMode = 1)
        Return bAlreadyStarted
    Else If Not bAlreadyStarted { ;iMode = 0. Call the function only if it hasn't already been called.
        
        ;Start it up - request version 2.2
        VarSetCapacity(wsaData, A_PtrSize = 4 ? 400 : 408, 0)
        iResult := DllCall("Ws2_32\WSAStartup", "UShort", 0x0202, "Ptr", &wsaData)
        If (iResult != 0) Or ErrorLevel {
            ErrorLevel := ErrorLevel ? ErrorLevel : iResult
            Return 1
        }
        
        ;Make sure the Winsock DLL supports at least version 2.2
        If (NumGet(wsaData, 2, "UShort") < 0x0202) {
            DllCall("Ws2_32\WSACleanup") ;Abort
            ErrorLevel := "The Winsock DLL does not support version 2.2."
            Return 2
        }
        
        bAlreadyStarted := True
    }
}

AHKsock_ShutdownSocket(iSocket) {
    
    ;Check if it's a listening socket
    sName := AHKsock_Sockets("GetName", iSocket)
    If (sName != A_Space) { ;It's not a listening socket. Shutdown send operations.
        iResult := DllCall("Ws2_32\shutdown", "Ptr", iSocket, "Int", 1) ;SD_SEND
        If (iResult = -1) Or ErrorLevel {
            sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
            DllCall("Ws2_32\closesocket", "Ptr", iSocket)
            AHKsock_Sockets("Delete", iSocket)
            ErrorLevel := sErrorLevel
            Return 1
        }
        
        ;Mark it
        AHKsock_Sockets("SetShutdown", iSocket)
        
    } Else {
        DllCall("Ws2_32\closesocket", "Ptr", iSocket) ;It's only a listening socket
        AHKsock_Sockets("Delete", iSocket) ;Remove it from the array
    }
}

/***********************\
 AsyncSelect functions  |
                      */
                                     ;FD_READ | FD_WRITE | FD_ACCEPT | FD_CLOSE
AHKsock_RegisterAsyncSelect(iSocket, fFlags = 43, sFunction = "AHKsock_AsyncSelect", iMsg = 0) {
    Static hwnd := False
    
    If Not hwnd { ;Use the main AHK window
        A_DetectHiddenWindowsOld := A_DetectHiddenWindows
        DetectHiddenWindows, On
        WinGet, hwnd, ID, % "ahk_pid " DllCall("GetCurrentProcessId") " ahk_class AutoHotkey"
        DetectHiddenWindows, %A_DetectHiddenWindowsOld%
    }
    
    iMsg := iMsg ? iMsg : AHKsock_Settings("Message")
    If (OnMessage(iMsg) <> sFunction)
        OnMessage(iMsg, sFunction)
    
    iResult := DllCall("Ws2_32\WSAAsyncSelect", "Ptr", iSocket, "Ptr", hwnd, "UInt", iMsg, "Int", fFlags)
    If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
        ErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
        Return 1
    }
}

AHKsock_AsyncSelect(wParam, lParam) {
    Critical ;So that messages are buffered
    
    ;wParam parameter identifies the socket on which a network event has occurred
    ;The low word of lParam specifies the network event that has occurred.
    ;The high word of lParam contains any error code
    
    ;Make sure the socket is on record. Fail-safe
    If Not AHKsock_Sockets("Index", wParam)
        Return
    
    iEvent := lParam & 0xFFFF, iErrorCode := lParam >> 16
    
    /*! Used for debugging purposes
    OutputDebug, % "AsyncSelect - A network event " iEvent " has occurred on socket " wParam
    If iErrorCode
        OutputDebug, % "AsyncSelect - Error code = " iErrorCode
    */
    
    If (iEvent = 1) { ;FD_READ
        
        ;Check for error
        If iErrorCode { ;WSAENETDOWN is the only possible
            ErrorLevel := iErrorCode
            ;FD_READ event received with an error. The error is in ErrorLevel. The socket is in iSocket.
            AHKsock_RaiseError(6, wParam)
            Return
        }
        
        VarSetCapacity(bufReceived, bufReceivedLength := AHKsock_Settings("Buffer"), 0)
        iResult := DllCall("Ws2_32\recv", "UInt", wParam, "Ptr", &bufReceived, "Int", bufReceivedLength, "Int", 0)
        If (iResult > 0) { ;We received data!
            VarSetCapacity(bufReceived, -1) ;Update the internal length
            
            ;Get associated function and call it
            If IsFunc(sFunc := AHKsock_Sockets("GetFunction", wParam))
                %sFunc%("RECEIVED", wParam, AHKsock_Sockets("GetName", wParam)
                                          , AHKsock_Sockets("GetAddr", wParam)
                                          , AHKsock_Sockets("GetPort", wParam), bufReceived, iResult)
            
        ;Check for error other than WSAEWOULDBLOCK
        } Else If ErrorLevel Or ((iResult = -1) And Not ((iErrorCode := AHKsock_LastError()) = 10035)) {
            ErrorLevel := ErrorLevel ? ErrorLevel : iErrorCode
            AHKsock_RaiseError(7, wParam) ;The recv() call failed. The error is in ErrorLevel. The socket is in iSocket.
            iResult = -1 ;So that if it's a spoofed call from FD_CLOSE, we exit the loop and close the socket
        }
        
        ;Here, we bother with returning a value in case it's a spoofed call from FD_CLOSE
        Return iResult
        
    } Else If (iEvent = 2) { ;FD_WRITE
        
        ;Check for error
        If iErrorCode { ;WSAENETDOWN is the only possible
            ErrorLevel := iErrorCode
            ;FD_WRITE event received with an error. The error is in ErrorLevel. The socket is in iSocket.
            AHKsock_RaiseError(8, wParam)
            Return
        }
        
        ;Update socket's setting
        AHKsock_Sockets("SetSend", wParam, True)
        
        ;Make sure the socket isn't already shut down
        If Not AHKsock_Sockets("GetShutdown", wParam)
            If IsFunc(sFunc := AHKsock_Sockets("GetFunction", wParam))
                %sFunc%("SEND", wParam, AHKsock_Sockets("GetName", wParam)
                                      , AHKsock_Sockets("GetAddr", wParam)
                                      , AHKsock_Sockets("GetPort", wParam))
        
    } Else If (iEvent = 8) { ;FD_ACCEPT
        
        ;Check for error
        If iErrorCode { ;WSAENETDOWN is the only possible
            ErrorLevel := iErrorCode
            ;FD_ACCEPT event received with an error. The error is in ErrorLevel. The socket is in iSocket.
            AHKsock_RaiseError(9, wParam)
            Return
        }
        
        ;We need to accept the connection
        VarSetCapacity(tSockAddr, tSockAddrLength := 16, 0)
        sktClient := DllCall("Ws2_32\accept", "Ptr", wParam, "Ptr", &tSockAddr, "Int*", tSockAddrLength)
        If (sktClient = -1) And ((iErrorCode := AHKsock_LastError()) = 10035) ;Check specifically for WSAEWOULDBLOCK
            Return ;We'll be called again next time we can retry accept()
        Else If (sktClient = -1) Or ErrorLevel { ;Check for INVALID_SOCKET
            ErrorLevel := ErrorLevel ? ErrorLevel : iErrorCode
            ;The accept() call failed. The error is in ErrorLevel. The listening socket is in iSocket.
            AHKsock_RaiseError(10, wParam)
            Return
        }
        
        ;Add to array
        sName := ""
        sAddr := DllCall("Ws2_32\inet_ntoa", "UInt", NumGet(tSockAddr, 4, "UInt"), "AStr")
        sPort := AHKsock_Sockets("GetPort", wParam)
        sFunc := AHKsock_Sockets("GetFunction", wParam)
        AHKsock_Sockets("Add", sktClient, sName, sAddr, sPort, sFunc)
        
        ;Go back to listening
        iResult := DllCall("Ws2_32\listen", "Ptr", wParam, "Int", 0x7FFFFFFF) ;SOMAXCONN       
        If (iResult = -1) Or ErrorLevel { ;Check for SOCKET_ERROR
            sErrorLevel := ErrorLevel ? ErrorLevel : AHKsock_LastError()
            DllCall("Ws2_32\closesocket", "Ptr", wParam)
            AHKsock_Sockets("Delete", wParam) ;Remove from array
            ErrorLevel := sErrorLevel
            ;The listen() call failed. The error is in ErrorLevel. The listening socket is in iSocket.
            AHKsock_RaiseError(12, wParam)
            Return
        }
        
        ;Get associated function and call it
        If IsFunc(sFunc)
            %sFunc%("ACCEPTED", sktClient, sName, sAddr, sPort)
        
    } Else If (iEvent = 32) { ;FD_CLOSE
        
        ;Keep receiving data before closing the socket by spoofing an FD_READ event to call recv()
        While (AHKsock_AsyncSelect(wParam, 1) > 0)
            Sleep, -1
        
        ;Check if we initiated it
        If Not AHKsock_Sockets("GetShutdown", wParam) {
            
            ;Last chance to send data. Get associated function and call it.
            If IsFunc(sFunc := AHKsock_Sockets("GetFunction", wParam))
                %sFunc%("SENDLAST", wParam, AHKsock_Sockets("GetName", wParam)
                                          , AHKsock_Sockets("GetAddr", wParam)
                                          , AHKsock_Sockets("GetPort", wParam))
            
            ;Shutdown the socket. This is to attempt a graceful shutdown
            If AHKsock_ShutdownSocket(wParam) {
                ;The shutdown() call failed. The error is in ErrorLevel. The socket is in iSocket.
                AHKsock_RaiseError(13, wParam)
                Return
            }
        }
        
        ;We just have to close the socket then
        DllCall("Ws2_32\closesocket", "Ptr", wParam)
        
        ;Get associated data before deleting
        sFunc := AHKsock_Sockets("GetFunction", wParam)
        sName := AHKsock_Sockets("GetName", wParam)
        sAddr := AHKsock_Sockets("GetAddr", wParam)
        sPort := AHKsock_Sockets("GetPort", wParam)
        
        ;We can remove it from the array
        AHKsock_Sockets("Delete", wParam)
        
        If IsFunc(sFunc)
            %sFunc%("DISCONNECTED", wParam, sName, sAddr, sPort)
    }
}

/******************\
 Array controller  |
                 */

AHKsock_Sockets(sAction = "Count", iSocket = "", sName = "", sAddr = "", sPort = "", sFunction = "") {
    Static
    Static aSockets0 := 0
    Static iLastSocket := 0xFFFFFFFF ;Cache to lessen index lookups on the same socket
    Local i, ret, A_IsCriticalOld
    
    A_IsCriticalOld := A_IsCritical
    Critical
    
    If (sAction = "Count") {
        ret := aSockets0
        
    } Else If (sAction = "Add") {
        aSockets0 += 1 ;Expand array
        aSockets%aSockets0%_Sock := iSocket
        aSockets%aSockets0%_Name := sName
        aSockets%aSockets0%_Addr := sAddr
        aSockets%aSockets0%_Port := sPort
        aSockets%aSockets0%_Func := sFunction
        aSockets%aSockets0%_Shutdown := False
        aSockets%aSockets0%_Send := False
        
    } Else If (sAction = "Delete") {
        
        ;First we need the index
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        
        If i {
            iLastSocket := 0xFFFF ;Clear cache
            If (i < aSockets0) { ;Let the last item overwrite this one
                aSockets%i%_Sock := aSockets%aSockets0%_Sock
                aSockets%i%_Name := aSockets%aSockets0%_Name
                aSockets%i%_Addr := aSockets%aSockets0%_Addr
                aSockets%i%_Port := aSockets%aSockets0%_Port
                aSockets%i%_Func := aSockets%aSockets0%_Func
                aSockets%i%_Shutdown := aSockets%aSockets0%_Shutdown
                aSockets%i%_Send := aSockets%aSockets0%_Send
                
            }
            aSockets0 -= 1 ;Remove element
        }
        
    } Else If (sAction = "GetName") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Name
        
    } Else If (sAction = "GetAddr") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Addr
        
    } Else If (sAction = "GetPort") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Port
        
    } Else If (sAction = "GetFunction") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Func
        
    } Else If (sAction = "SetFunction") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        aSockets%i%_Func := sName
        
    } Else If (sAction = "GetSend") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Send
        
    } Else If (sAction = "SetSend") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        aSockets%i%_Send := sName
        
    } Else If (sAction = "GetShutdown") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        ret := aSockets%i%_Shutdown
        
    } Else If (sAction = "SetShutdown") {
        i := (iSocket = iLastSocket) ;Check cache
        ? iLastSocketIndex
        : AHKsock_Sockets("Index", iSocket)
        aSockets%i%_Shutdown := True
        
    } Else If (sAction = "GetSocketFromNamePort") {
        Loop % aSockets0 {
            If (aSockets%A_Index%_Name = iSocket)
            And (aSockets%A_Index%_Port = sName) {
                ret := aSockets%A_Index%_Sock
                Break
            }
        }
        
    } Else If (sAction = "GetSocketFromIndex") {
        ret := aSockets%iSocket%_Sock
    
    } Else If (sAction = "Index") {
        Loop % aSockets0 {
            If (aSockets%A_Index%_Sock = iSocket) {
                iLastSocketIndex := A_Index, iLastSocket := iSocket
                ret := A_Index
                Break
            }
        }
    }
    
    ;Restore old Critical setting
    Critical %A_IsCriticalOld%
    Return ret
}

/*****************\
 Error Functions  |
                */

AHKsock_LastError() {
    Return DllCall("Ws2_32\WSAGetLastError")
}

AHKsock_ErrorHandler(sFunction = """") {
    Static sCurrentFunction
    If (sFunction = """")
        Return sCurrentFunction
    Else sCurrentFunction := sFunction
}

AHKsock_RaiseError(iError, iSocket = -1) {
    If IsFunc(sFunc := AHKsock_ErrorHandler())
        %sFunc%(iError, iSocket)
}

/*******************\
 Settings Function  |
                  */

AHKsock_Settings(sSetting, sValue = "") {
    Static iMessage := 0x8000
    Static iBuffer := 65536
    
    If (sSetting = "Message") {
        If Not sValue
            Return iMessage
        Else iMessage := (sValue = "Reset") ? 0x8000 : sValue
    } Else If (sSetting = "Buffer") {
        If Not sValue
            Return iBuffer
        Else iBuffer := (sValue = "Reset") ? 65536 : sValue
    }
}