/*
以下演示的是，将摄像头里的面部截取出来，并同步到ElectronBot的脸上并做出右手挥手动作。

使用方法：
将 此脚本 和 "haarcascade_frontalface_default.xml" 放到 "ElectronBotSDK" 目录后，
运行此脚本即可查看测试。【需要确保摄像头画面中有面部可识别】

也可提取 https://github.com/dbgba/VisualGestureRecognition ，做手势同步和语音识别
*/
SetBatchLines -1
#SingleInstance Force
SetWorkingDir %A_ScriptDir%

; 加载并初始化OpenCV
hOpenCV := DllCall("LoadLibrary", "Str", "opencv_world455.dll", "Ptr")
hOpenCVCom := DllCall("LoadLibrary", "Str", "autoit_opencv_com455.dll", "Ptr")

Try cv := ComObjCreate("OpenCV.cv")
  catch
    DllCall("autoit_opencv_com455.dll\DllInstall", "int", 1, "Wstr", A_IsAdmin=0 ? "user" : "", "cdecl")
    , cv := ComObjCreate("OpenCV.cv")

Global CV := ComObjCreate("OpenCV.CV")
Cap := ComObjCreate("OpenCV.CV.VideoCapture")
Frame := ComObjCreate("OpenCV.CV.MAT")
Cap.open(0)  ; 调用第几个摄像头，从0开始

faceCascade := ComObjCreate("Opencv.cv.CascadeClassifier")
faceCascade.load("haarcascade_frontalface_default.xml")  ; 加载OpenCV人脸识别判断

; LowLevelSDK 加载与连接
姿势 := New LowLevelSDK()

; 进入脸部获取同步循环，按Esc键可退出脚本
Loop {  
    Ret := Cap.read(Frame)

    frame := cv.rotate(frame, 2)  ; 适配ElectronBot使用画面旋转，如果不是ElectronBot需要删除此行才有图像

    faces := faceCascade.detectMultiScale(Frame, 1.1+0, 10+0)  ; 人脸识别

    ; 纯人脸识别完整显示
    ; Loop % faces.MaxIndex()+1
    ;     CV.rectangle(Frame, faces[A_Index-1], ComArrayMake([255, 0, 255]), 3)
    ; CV.imshow("Live", Frame)

    ; 识别面部并裁剪显示【如果没有图像，尝试删除cv.rotate或者使用上面这段"纯人脸识别完整显示"获取】
    if (faces.MaxIndex()>-1) {
        frame := Crop(frame, faces[0])  ; 裁剪人像
        frame := cv.resize(frame, ComArrayMake([240, 240]))  ; 调整图像分辨率
        frame := cv.cvtColor(frame, 3)  ; CV_BGRA2RGB := 3
        姿势.设置姿势(15, 0, ( (_Waving:=!_Waving) ? 0 : 30), 130, 0, 0)  ; 挥动左手
        姿势.设置图像Data(frame.data())
        姿势.同步()
        ; CV.imshow("Live", frame)  ; 去掉这行注释可在电脑也显示人像图像
     } else {
        姿势.同步姿势(0, 0, 0, 0, 0, 0)  ; 检测不到面部时，就回正
     }

}
Return

; 按Esc键可退出脚本
Esc::ExitApp


; ================== 以下是脚本所用的函数类库 ==================

Crop(Img, Pos) {
    x := Pos[0]
    , y := Pos[1]
    , Width := Pos[2]
    , Height := Pos[3]
    , Row_Array := Array()
    , Col_Array := Array()

    ; CV := ComObjCreate("OpenCV.CV")

    Loop % Height - 1
        Row_Array.Push(Img.Row(y++))

    Img := CV.vconcat(ComArrayMake(Row_Array))

    Loop % Width - 1
        Col_Array.Push(Img.Col(x++))

    Img := CV.hconcat(ComArrayMake(Col_Array))
    Return Img
}

ComArrayMake(InputArray) {
    Arr := ComObjArray(VT_VARIANT:=12, InputArray.Length())

    Loop % InputArray.Length()
        Arr[A_Index-1] := InputArray[A_Index]

    Return Arr
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