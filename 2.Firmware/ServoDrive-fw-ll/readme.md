这个是智能舵机的LL库版本，代码体积得到优化，使用vscode和platformio开发，开发环境和编译参考[这个视频](https://www.bilibili.com/video/BV1US4y1P7Xr/)

## **2022.04.18**

1. 将i2c部分由hal库改为ll库实现；
2. 设置i2c地址立刻生效，不需要重启；