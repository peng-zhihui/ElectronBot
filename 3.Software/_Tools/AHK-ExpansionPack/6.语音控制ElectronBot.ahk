/*
系统要求：
由于语音识别是调用Windows自带API来实现的，使用的是外接电脑麦克风做收音识别。
有些系统为了缩减体积会删掉此语音识别播报API导致无效，用原版镜像上安装的系统基本都不会出这问题。
如果此脚本运行时报错，则该系统缺少TTS组件无法使用语音识别功能。

使用方法：
将此脚本放到 "ElectronBotSDK" 目录后，运行此脚本即可测试。
先说出唤醒词"电子电子"，唤醒后再说出对应的命令即可让ElectronBot同步反应。例如："点头"

需要更动态的方式可以参考：https://github.com/dbgba/VisualGestureRecognition 里，
"Lib" 文件夹中的 "_SpeechRecognition.ahk" 脚本内容做参考和修改。
*/
#Persistent
#SingleInstance Force
SetBatchLines -1
SetWorkingDir %A_ScriptDir%

CoordMode ToolTip
Global 执行命令开关

Global 姿势 := New LowLevelSDK()
Global 表情 := New PlayerSDK()

Global LLSDKFilePath := "test.jpg"  ; 用全局变量，让所有同步姿势都使用这个表情

唤醒词 := New 唤醒词类
唤醒词.Recognize(["电子电子"])
 
执行命令 := New 执行命令类
执行命令.Recognize(["你好", "左手", "右手", "点头", "开心"])

Global 异步语音播报 := New TTS()
Return

Class 唤醒词类 extends SpeechRecognizer {
	OnRecognize(Text) {
		if (Text = "电子电子") {
			ToolTip % "唤醒词：" Text , A_ScreenWidth//1.13, A_ScreenHeight//1.13
            ComObjCreate("SAPI.SpVoice").Speak("在呢")  ; 使用同步语音播报可替代延时，防止直接进入执行指令
            执行命令开关 := 1
            SetTimer 无应答重置, -5000  ; 唤醒后，5秒内无命令则重置唤醒
        }
	}
}

Class 执行命令类 extends SpeechRecognizer {
	OnRecognize(Text) {
        if 执行命令开关 {
            if (Text = "你好") {
                ToolTip % "你说了：" Text
                Loop 2 {  ; 招手2次
                    Sleep 130
                    姿势.同步姿势(15, 0, 10, 180, 0, 0)
                    Sleep 130
                    姿势.同步姿势(15, 0, 30, 180, 0, 0)
                }
            }

            if (Text = "左手") {
               姿势.同步姿势(0, 0, 30, 180, 0, 0)
               ToolTip % "你说了：" Text
            }

            if (Text = "右手") {
               姿势.同步姿势(0, 0, 0, 0, 30, 180)
               ToolTip % "你说了：" Text
            }

            if (Text = "点头") {
                ToolTip % "你说了：" Text
               姿势.同步姿势(15, 0, 0, 0, 0, 0)
               Sleep 180
               姿势.同步姿势(0, 0, 0, 0, 0, 0)
               Sleep 180
               姿势.同步姿势(15, 0, 0, 0, 0, 0)
               Sleep 180
               姿势.同步姿势(0, 0, 0, 0, 0, 0)
            }

            if (Text = "开心") {
                ToolTip % "你说了：" Text
                表情.播放表情("video.mp4")
            }
            异步语音播报.Speak("你说了：" Text)
            执行命令开关 := ""
        }
	}
}

无应答重置:
    执行命令开关 := ""
Return


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

/*
    by Uberi，Modified from：https://gist.github.com/Uberi/6263822
	UBERI's SAPI Speech Wrapper for AHK
	Speech Recognition
	==================
	A class providing access to Microsoft's SAPI. Requires the SAPI SDK.
	Reference
	---------
	### Recognizer := new SpeechRecognizer
	Creates a new speech recognizer instance.
	The instance starts off listening to any phrases.
	### Recognizer.Recognize(Values = True)
	Set the values that can be recognized by the recognizer.
	if `Values` is an array of strings, the array is interpreted as a list of possibile phrases to recognize. Phrases not in the array will not be recognized. This provides a relatively high degree of recognition accuracy 

compared to dictation mode.
		if `Values` is otherwise truthy, dictation mode is enabled, which means that the speech recognizer will attempt to recognize any phrases spoken.
			if `Values` is falsy, the speech recognizer will be disabled and will stop listening if currently doing so.
				Returns the speech recognizer instance.
	### Recognizer.Listen(State = True)
	Set the state of the recognizer.
	if `State` is truthy, then the recognizer will start listening if not already doing so.
		if `State` is falsy, then the recognizer will stop listening if currently doing so.
			Returns the speech recognizer instance.
	### Text := Recognizer.Prompt(Timeout = -1)
	Obtains the next phrase spoken as plain text.
	if `Timeout` is a positive number, the function will stop and return a blank string after this amount of time, if the user has not said anything in this interval.
		if `Timeout` is a negative number, the function will wait indefinitely for the user to speak a phrase.
			Returns the text spoken.
	### Recognizer.OnRecognize(Text)
	A callback invoked immediately upon any phrases being recognized.
	The `Text` parameter received the phrase spoken.
	This function is meant to be overridden in subclasses. By default, it does nothing.
	The return value is discarded.
*/

class SpeechRecognizer { ; speech recognition class by Uberi
	static Contexts := {}

	__New() {
		try this.cListener := ComObjCreate("SAPI.SpInprocRecognizer") ;obtain speech recognizer (ISpeechRecognizer object)
			, cAudioInputs := this.cListener.GetAudioInputs() ;obtain list of audio inputs (ISpeechObjectTokens object)
			, this.cListener.AudioInput := cAudioInputs.Item(0) ;set audio device to first input
		 catch e
			throw Exception("Could not create recognizer: " . e.Message)

		try this.cContext := this.cListener.CreateRecoContext() ;obtain speech recognition context (ISpeechRecoContext object)
		 catch e
			throw Exception("Could not create recognition context: " . e.Message)

		try this.cGrammar := this.cContext.CreateGrammar() ;obtain phrase manager (ISpeechRecoGrammar object)
		 catch e
			throw Exception("Could not create recognition grammar: " . e.Message)

		;create rule to use when dictation mode is off
		try this.cRules := this.cGrammar.Rules() ;obtain list of grammar rules (ISpeechGrammarRules object)
			, this.cRule := this.cRules.Add("WordsRule",0x1 | 0x20) ;add a new grammar rule (SRATopLevel | SRADynamic)
		 catch e
			throw Exception("Could not create speech recognition grammar rules: " . e.Message)

		this.Phrases(["hello", "hi", "greetings", "salutations"])
		, this.Dictate(True)

		, SpeechRecognizer.Contexts[&this.cContext] := &this ;store a weak reference to the instance so event callbacks can obtain this instance
		, this.Prompting := False ;prompting defaults to inactive

		, ComObjConnect(this.cContext, "SpeechRecognizer_") ;connect the recognition context events to functions
	}

	Recognize(Values = True) {
		if Values { ;enable speech recognition
			this.Listen(True)
			if IsObject(Values) ;list of phrases to use
				this.Phrases(Values)
			 else ;recognize any phrase
				this.Dictate(True)
		} else ;disable speech recognition
			this.Listen(False)
		Return this
	}

	Listen(State = True) {
		try if State
				this.cListener.State := 1 ;SRSActive
			 else
				this.cListener.State := 0 ;SRSInactive
		 catch e
			throw Exception("Could not set listener state: " . e.Message)
		Return this
	}

	Prompt(Timeout = -1) {
		this.Prompting := True
		, this.SpokenText := ""
		if (Timeout < 0) ;no timeout
			While, this.Prompting
				Sleep 0
		 else {
			StartTime := A_TickCount
			While, this.Prompting && (A_TickCount - StartTime) > Timeout
				Sleep 0
		}
		Return this.SpokenText
	}

	Phrases(PhraseList) {
		try this.cRule.Clear() ;reset rule to initial state
		 catch e
			throw Exception("Could not reset rule: " . e.Message)

		try cState := this.cRule.InitialState() ;obtain rule initial state (ISpeechGrammarRuleState object)
		 catch e
			throw Exception("Could not obtain rule initial state: " . e.Message)

		;add rules to recognize
		cNull := ComObjParameter(13,0) ;null IUnknown pointer
		For Index, Phrase In PhraseList
			try cState.AddWordTransition(cNull, Phrase) ;add a no-op rule state transition triggered by a phrase
			 catch e
				throw Exception("Could not add rule """ . Phrase . """: " . e.Message)

		try this.cRules.Commit() ;compile all rules in the rule collection
		 catch e
			throw Exception("Could not update rule: " . e.Message)

		this.Dictate(False) ;disable dictation mode
		Return this
	}

	Dictate(Enable = True) {
		try if Enable ;enable dictation mode
				this.cGrammar.DictationSetState(1) ;enable dictation mode (SGDSActive)
				, this.cGrammar.CmdSetRuleState("WordsRule", 0) ;disable the rule (SGDSInactive)
			 else ;disable dictation mode
				this.cGrammar.DictationSetState(0) ;disable dictation mode (SGDSInactive)
				, this.cGrammar.CmdSetRuleState("WordsRule", 1) ;enable the rule (SGDSActive)
		 catch e
			throw Exception("Could not set grammar dictation state: " . e.Message)
		Return this
	}

	OnRecognize(Text) {
		;placeholder function meant to be overridden in subclasses
	}

	__Delete() { ; remove weak reference to the instance
		this.base.Contexts.Remove(&this.cContext, "")
	}
}

SpeechRecognizer_Recognition(StreamNumber, StreamPosition, RecognitionType, cResult, cContext) { ;speech recognition engine produced a recognition
	try pPhrase := cResult.PhraseInfo() ;obtain detailed information about recognized phrase (ISpeechPhraseInfo object from ISpeechRecoResult object)
		, Text := pPhrase.GetText() ;obtain the spoken text
	 catch e
		throw Exception("Could not obtain recognition result text: " . e.Message)

	Instance := Object(SpeechRecognizer.Contexts[&cContext]) ;obtain reference to the recognizer

	;handle prompting mode
	if Instance.Prompting
		Instance.SpokenText := Text
		, Instance.Prompting := False

	Instance.OnRecognize(Text) ;invoke callback in recognizer
}

; https://www.autohotkey.com/boards/viewtopic.php?f=6&t=12304
; Class TTS by evilC
; Based on code by Learning one. For AHK_L. Thanks: jballi, Sean, Frankie.
; AHK forum location: www.autohotkey.com/forum/topic57773.html
; Read more: msdn.microsoft.com/en-us/library/ms723602(v=VS.85).aspx, www.autohotkey.com/forum/topic45471.html, www.autohotkey.com/forum/topic83162.html
Class TTS {
	VoiceList := []  ; An indexed array of the available voice names
	, VoiceAssoc := {}  ; An Associative array of voice names, key = voice name, value = voice index (VoiceList lookup)
	, VoiceCount := 0  ; The number of voices available
	, VoiceNumber := 0  ; The number of the current voice
	, VoiceName := ""  ; The name of the current voice
	
	__New() {
		this.oVoice := ComObjCreate("SAPI.SpVoice")
		, this._GetVoices()
		, this.SetVoice(this.VoiceList.1)
	}

	; speak or stop speaking
	ToggleSpeak(text) {
		Status := this.oVoice.Status.RunningState
		if Status = 1	; finished
			this.oVoice.Speak(text,0x1)	; speak asynchronously
		Else if Status = 0	; paused
		{
			this.oVoice.Resume
			this.oVoice.Speak("",0x1|0x2)  ; stop
			this.oVoice.Speak(text,0x1)  ; speak asynchronously
		} Else if Status = 2  ; reading
			this.oVoice.Speak("",0x1|0x2)  ; stop
	}

	; speak asynchronously
	Speak(text) {
		Status := this.oVoice.Status.RunningState
		if Status = 0  ; paused
			this.oVoice.Resume
		this.oVoice.Speak("",0x1|0x2)  ; stop
		, this.oVoice.Speak(text,0x1)  ; speak asynchronously
	}

	; speak synchronously
	SpeakWait(text) {
		Status := this.oVoice.Status.RunningState
		if Status = 0  ; paused
			this.oVoice.Resume
		this.oVoice.Speak("",0x1|0x2)  ; stop
		, this.oVoice.Speak(text,0x0)  ; speak synchronously
	}

	; Pause toggle
	Pause() {
		Status := this.oVoice.Status.RunningState
		if Status = 0  ; paused
			this.oVoice.Resume
		else if Status = 2  ; reading
			this.oVoice.Pause
	}

	Stop() {
		Status := this.oVoice.Status.RunningState
		if Status = 0	; paused
			this.oVoice.Resume
		this.oVoice.Speak("",0x1|0x2)	; stop
	}

	; rate (reading speed): rate from -10 to 10. 0 is default.
	SetRate(rate) {
		this.oVoice.Rate := rate
	}

	; volume (reading loudness): vol from 0 to 100. 100 is default
	SetVolume(vol) {
		this.oVoice.Volume := vol
	}

	; pitch : From -10 to 10. 0 is default.
	; http://msdn.microsoft.com/en-us/library/ms717077(v=vs.85).aspx
	SetPitch(pitch) {
		this.oVoice.Speak("<pitch absmiddle = '" pitch "'/>",0x20)
	}

	; Set voice by name
	SetVoice(VoiceName) {
		if (!ObjHasKey(this.VoiceAssoc, VoiceName))
			return 0
		While !(this.oVoice.Status.RunningState = 1)
		Sleep, 20
		this.oVoice.Voice := this.oVoice.GetVoices("Name=" VoiceName).Item(0) ; set voice to param1
		, this.VoiceName := VoiceName
		, this.VoiceNumber := this.VoiceAssoc[VoiceName]
		return 1
	}

	; Set voice by index
	SetVoiceByIndex(VoiceIndex) {
		return this.SetVoice(this.VoiceList[VoiceIndex])
	}

	; Use the next voice. Loops around at end
	NextVoice() {
		v := this.VoiceNumber + 1
		if (v > this.VoiceCount)
			v := 1
		return this.SetVoiceByIndex(v)
	}

	; Returns an array of voice names
	GetVoices() {
		return this.VoiceList
	}

	GetStatus(){
		Status := this.oVoice.Status.RunningState
		if Status = 0 ; paused
			Return "paused"
		Else if Status = 1 ; finished
			Return "finished"
		Else if Status = 2 ; reading
			Return "reading"
	}

	GetCount() {
		return this.VoiceCount
	}

	SpeakToFile(param1, param2) {
		oldAOS := this.oVoice.AudioOutputStream
		, oldAAOFCONS := this.oVoice.AllowAudioOutputFormatChangesOnNextSet
		, this.oVoice.AllowAudioOutputFormatChangesOnNextSet := 1	

		, SpStream := ComObjCreate("SAPI.SpFileStream")
		FileDelete, % param2  ; OutputFilePath
		SpStream.Open(param2, 3)
		, this.oVoice.AudioOutputStream := SpStream
		, this.SpeakWait(param1)
		, SpStream.Close()
		, this.oVoice.AudioOutputStream := oldAOS
		, this.oVoice.AllowAudioOutputFormatChangesOnNextSet := oldAAOFCONS
	}

	; ====== Private funcs, not intended to be called by user =======
	_GetVoices() {
		this.VoiceList := []
		, this.VoiceAssoc := {}
		, this.VoiceCount := this.oVoice.GetVoices.Count
		Loop, % this.VoiceCount
			Name := this.oVoice.GetVoices.Item(A_Index-1).GetAttribute("Name")  ; 0 based
			, this.VoiceList.push(Name)
			, this.VoiceAssoc[Name] := A_Index
	}
}