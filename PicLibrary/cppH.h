#ifndef _CPP_H
#define _CPP_H

typedef unsigned char byte;

/// <summary>
/// 入口点函数,使用CPU降低图像亮度
/// </summary>
/// <param name="pic">像素起始地址<br/>像素格式:RGB(24bit)</param>
/// <param name="width">宽度</param>
/// <param name="height">高度</param>
/// <param name="stride">扫描宽度</param>
/// <param name="brightness">亮度</param>
/// <returns>是否执行成功</returns>
extern "C" _declspec(dllexport) bool CPP_SetBrightness(
    byte * pic, int width, int height, int stride, byte brightness
);

/// <summary>
/// 入口点函数,使用CPU设置透明像素背景色
/// </summary>
/// <param name="pic">像素起始地址<br/>像素格式:BGRA(32bit)</param>
/// <param name="width">宽度</param>
/// <param name="height">高度</param>
/// <param name="stride">扫描宽度</param>
/// <param name="R">R</param>
/// <param name="G">G</param>
/// <param name="B">B</param>
/// <returns>是否执行成功</returns>
extern "C" _declspec(dllexport) bool CPP_SetTransparentPixelBackgroundColorByKernel(
    byte * pic, int width, int height, int stride, byte R, byte G, byte B
);

/// <summary>
/// 入口点函数,使用CPU压缩PNG图片
/// </summary>
/// <param name="pic">像素起始地址</param>
/// <param name="width">宽度</param>
/// <param name="height">高度</param>
/// <param name="stride">扫描宽度</param>
/// <param name="pixelBits">单个像素所占字节数</param>
/// <param name="R">R</param>
/// <param name="G">G</param>
/// <param name="B">B</param>
/// <returns>是否执行成功</returns>
extern "C" _declspec(dllexport) bool CPP_CompressPNG(
    byte * pic, int width, int height, int stride, int pixelBits, int level
);

#endif // !_CPP_H