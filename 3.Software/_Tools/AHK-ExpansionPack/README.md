* 这是基于稚晖君[ElectronBot](https://github.com/peng-zhihui/ElectronBot)的SDK玩法扩展的示例，使用AutoHotkey来进行SDK调用与演示

* 因为AutoHotkey是脚本语言，用记事本即可编辑.ahk文件，和批处理.bat原理类似。对于想要修改自定义参数的可编辑.ahk文件，保存运行即可。无需前端界面


# 使用介绍
* 虽然AHK可以将解释器与.ahk脚本设为同名，免安装使用。但是为了方便调试使用，但是建议到[AutoHotkey](https://www.autohotkey.com/download/ahk-install.exe)官网下载安装一下【建议使用默认选项安装】。毕竟安装包就3MB大小，能避免不必要的麻烦。

* 安装好AutoHotkey后，就可以对.ahk文件右键选Edit Script来编辑脚本了。项目所有示例中我只保留适配与ElectronBot相关的代码，而实际使用中我们可以使用AHK自身对电脑状态的判断和调用来实现很多的功能。
* 例如：判断时间来设置电脑音量、当ElectronBot做姿势后，同时播报自定义语音、判断进程状态，决定开关某程序等等。需要根据自身情况查看[AHK中文帮助文档](https://www.autoahk.com/help/autohotkey/zh-cn/docs/commands/WinActive.htm)或使用对应函数类库做具体实现。

　

每个文件夹的示例中都附带了详细截图与代码注释，以下仅做一些简单的介绍。

**1.通过微信来控制ElectronBot**

调用Accessible(无障碍)获取指定联系人的信息做为控制信号，即可实现比如：收到ElectronBot姿势参数后，调用SDK完成动作。

　

**2.对智能家居的控制**

此示例选了一个简单易完成的方法硬改来实现电脑控制智能家居。示例用的继电器是CH340驱动，而AHK就能直接与CH340设备通信来进行控制。

　

**3.量子纠缠Demo**

之前想模仿稚晖君视频里的"量子纠缠"做的Demo，将摄像头里的面部截取出来，并同步到ElectronBot的脸上同时做出右手挥手动作。

　

**4.ElectronBot网页控制与网络互动，手机网页控制ElectronBot**

此示例是简单的验证了一下AHK通过公网IP或者局域网远程同步两台ElectronBot的互动反应。还附带了网页控制示例，用浏览器来操作ElectronBot的话，就能实现手机或电脑的远程控制。

　

**5.摄像头手势识别与ElectronBot动作同步**

此示例需要与我另一个手势识别项目配合来实现，示例预设演示同步动作有：双手放下时、单手举起握拳时、单手举起做剪刀手时、双手举起握拳时。摄像头识别到上述手势后，ElectronBot也会同步反应。当双手举起握拳时，会执行一套预设的长动作。

　

**6.语音控制ElectronBot**

此示例调用的是Windows自带的API来实现的，不能保证所有系统都能复现。大致就是演示一种玩法，让ElectronBot可以变成你的语音助手，并做出指定反馈。已经提供了SDK调用方法，用什么语音方案可自行选择。

　

**7.连接ElectronBot时自动推送表情**

ElectronBot在每次接入USB时都会以默认花屏显示，此辅助示例通过注册USB设备监控事件来让每次接入的时，ElectronBot可以立即同步指定表情或姿势。

　

**8.热键快速调试ElectronBot**

在日常调试ElectronBot时，需要打开上位机调整对应参数后，再同步到ElectronBot。而AutoHotkey就是为设置键盘快捷键而生的，示例简单演示了F1、F2为头部上下，F3选择同步表情文件，上下键为左手的抬起放下，左右键为右手的抬起放下。
