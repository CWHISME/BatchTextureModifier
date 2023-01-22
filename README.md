# BatchTextureModifier

批量图片处理工具，使用 WPF .NetCore6.0 开发

目前支持：

1. 批量处理图片(也可以单图处理)
2. 预览修改效果、及修改后文件大小
3. 覆盖修改、自动备份(前提是直接覆盖修改的话)
4. 多种模式缩放图片
5. 按照指定参数，批量转换图片格式为 `*.png` `*.jpg` `*.webp` `*.tga` `*.bmp` `*.gif`
6. POT缩放(玩过 Unity 的应该知道什么意思，不过这个跟 Unity 拉伸方式不同)
7. 多线程处理，同时开启处理量取决于CPU核心数目

# 使用

UI 觉得做的挺简单的，直接看图吧：

![](Images/Snipaste_2023-01-21_19-51-37.jpg)
![](Images/Snipaste_2023-01-22_10-54-54.jpg)

# 感谢

图片处理基于图像库 [ImageSharp](https://github.com/SixLabors/ImageSharp)