#NoEnv
SetBatchLines -1
SetWorkingDir %A_ScriptDir%

姿势 := New LowLevelSDK()
表情 := New PlayerSDK()

异步抽帧上传表情("video.mp4")

异步播放音频("C:\Windows\Media\Alarm01.wav")  ; 播放指定系统声音做个演示

; Global LLSDKFilePath := "test.jpg"  ; 去掉注释可让所有同步姿势都使用这个表情

姿势.同步姿势(15, 30, 30, 180, 30, 180)
Sleep 200
姿势.同步姿势(15, 20, 30, 0, 30, 0)
Sleep 200
姿势.同步姿势(15, -30, 30, 180, 30, 180)
Sleep 200
姿势.同步姿势(15, 30, 30, 180, 30, 180)
Sleep 200
姿势.同步姿势(15, 0, 30, 180, 30, 180)  ; 回正

Loop 2 {  ; 招手2次
    Sleep 180
    姿势.同步姿势(15, 0, 0, 180, 0, 0)
    Sleep 180
    姿势.同步姿势(15, 0, 30, 180, 0, 0)
}

; 侧身拍打3次
Sleep 180
姿势.同步姿势(15, 20, 30, 0, 30, 0)
Sleep 180
姿势.同步姿势(15, 20, 0, 0, 0, 0)
Sleep 180
姿势.同步姿势(15, -20, 30, 0, 30, 0)
Sleep 180
姿势.同步姿势(15, -20, 0, 0, 0, 0)
Sleep 180
姿势.同步姿势(15, 20, 30, 0, 30, 0)
Sleep 180
姿势.同步姿势(15, 20, 0, 0, 0, 0)

Loop 2 {  ; 回正挥舞双手2次
    Sleep 200
    姿势.同步姿势(15, 0, 8, 180, 8, 180)
    Sleep 200
    姿势.同步姿势(15, 0, 8, 0, 8, 0)
}

Sleep 200
姿势.同步姿势(15, 20, 8, 0, 8, 0)
Sleep 200
姿势.同步姿势(15, 20, 8, 100, 8, 100)
Sleep 200
姿势.同步姿势(15, -20, 8, 0, 8, 0)
Sleep 200
姿势.同步姿势(15, -20, 8, 100, 8, 100)

Sleep 200
姿势.同步姿势(15, 20, 30, -20, 30, 180)
Sleep 300
姿势.同步姿势(15, -20, 30, 180, 30, -20)  ; 摆造型
Sleep 100

Loop 2 {  ;  高举双手左右晃动展示
    Sleep 200
    姿势.同步姿势(15, 40, 20, 180, 20, 180)
    Sleep 200
    姿势.同步姿势(15, -40, 20, 180, 20, 180)
}

Loop 2 {  ; 拜拜
    Sleep 200
    姿势.同步姿势(15, 0, 0, 180, 0, 0)
    Sleep 200
    姿势.同步姿势(15, 0, 30, 180, 0, 0)
}
Sleep 200
姿势.同步姿势(15, 0, 0, 0, 0, 0)  ; 回正

Sleep 200
; 播放表情前需确保与上个动作有至少150毫秒的延时，避免线程抢占卡死
表情.播放表情("video.mp4")

表情.断开连接()
姿势.断开连接()
ExitApp


; ================== 以下是脚本所用的函数类库 ==================

; 按每秒只取10帧显示，每秒10帧的单帧间隔为166毫秒。所以动作之间的延时间隔推荐180毫秒
异步抽帧上传表情(表情文件路径) {
    Global ; 设置函数为全局变量，与函数外变量互相影响
    hOpenCV := DllCall("LoadLibrary", "Str", "opencv_world455.dll", "Ptr")
    hOpenCVCom := DllCall("LoadLibrary", "Str", "autoit_opencv_com455.dll", "Ptr")

    Try cv := ComObjCreate("OpenCV.cv")
      catch
        DllCall("autoit_opencv_com455.dll\DllInstall", "int", 1, "Wstr", A_IsAdmin=0 ? "user" : "", "cdecl")
        , cv := ComObjCreate("OpenCV.cv")

    cap := ComObjCreate("OpenCV.cv.VideoCapture")
    frame := ComObjCreate("OpenCV.cv.Mat")

    ; 判断文件是否存在和转移文件修正不支持中文路径的问题
    if FileExist(表情文件路径) {
        if (StrLen(表情文件路径) = StrPut(表情文件路径, "utf-8")-1) {
            cap.open(表情文件路径)
         } else {
            SplitPath, 表情文件路径, , , _表情后缀名
            FileCopy, %表情文件路径%, % A_AppDataCommon "\ElectronBotVideo." _表情后缀名, 1
            cap.open(A_AppDataCommon "\ElectronBotVideo." _表情后缀名)
        }
     } else {
        MsgBox 0x30, 设置的表情文件不存在！, 请检查文件路径是否正确，`n建议放在同目录下，方便免路径读取。
        ExitApp
    }

    if !cap.isOpened() {
        MsgBox 0x30, 表情文件未能成功打开, 请检查表情视频是否符合标准，`n建议放在同目录下，方便免路径读取。
        ExitApp
    }

    ; 视频信息获取
    fps := cap.get(5)  ; 每秒帧数：CAP_PROP_FPS := 5
    ; 按每秒只取10帧显示，每秒10帧的单帧间隔为166毫秒。
    抽帧间隔 := Round(fps/10)  ; 60帧=隔6帧显示一次
    初始次数 := 1

    SetTimer 异步表情播放, 1
    Sleep 130
    Return
    
    异步表情播放:
        初始次数++
        if (Mod(初始次数, 抽帧间隔)=0) {  ; 60帧=隔6帧显示一次
            cap.read(frame)
            if (frame.data()=0) {  ; 当最后一帧数据为空时，停止循环
                SetTimer 异步表情播放, Off
                ; 异步抽帧上传表情("video.mp4")  ; 可设置循环或重复播放表情
                Return
            }
            frame := cv.resize(frame, ComArrayMake([240, 240]))
            frame := cv.cvtColor(frame, 3)  ; CV_BGRA2RGB := 3
            姿势.设置图像Data(frame.data())
        }
    Return
}

ComArrayMake(InputArray) {
    Arr := ComObjArray(VT_VARIANT:=12, InputArray.Length())

    Loop % InputArray.Length()
        Arr[A_Index-1] := InputArray[A_Index]

    Return Arr
}

; ================== 异步播放音频函数库 ==================
异步播放音频(mFile) {
	VarSetCapacity(DN, 16)
    , DLLFunc := "winmm.dll\mciSendString" (A_IsUnicode ? "W" : "A")
	, DllCall(DLLFunc, "Str", "Open """ mFile """ Alias MP3", "Uint", 0, "Uint", 0, "Uint", 0)
	, DllCall(DLLFunc, "Str", "Status MP3 Length", "Str", DN, "Uint", 16, "Uint", 0)
	, DllCall(DLLFunc, "Str", "Close MP3", "Uint", 0, "Uint", 0, "Uint",0)
	if !DN
        MsgBox 0x10, 不支持此音频文件, 系统API不支持此音频播放，`n可转换成恒定码率再试一次。, 2
    DllCall(DLLFunc, "Str", "Open """ mFile """", "Uint", 0, "Uint", 0, "Uint", 0)
    , DllCall(DLLFunc, "Str", "Play """ mFile """ FROM 000 to " DN, "Uint", 0, "Uint", 0, "Uint", 0)
}

继续播放(mFile) {
    DllCall("Winmm.dll\mciSendString", "Str", "Play """ mFile """", "Uint", 0, "Uint", 0, "Uint", 0)
}

暂停播放(mFile) {
    DllCall("Winmm.dll\mciSendString", "Str", "Pause """ mFile """", "Uint", 0, "Uint", 0, "Uint", 0)
}

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