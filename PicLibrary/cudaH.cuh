#ifndef _CUDA_H
#define _CUDA_H

#include "cuda_runtime.h"
#include "device_launch_parameters.h"

typedef unsigned char byte;

/// <summary>
/// GPU最大线程数，该变量定义在显存中
/// </summary>
int* Device_MaxThreads;

/// <summary>
/// GPU最大线程数，该变量定义在内存中
/// </summary>
int Host_MaxThreads;

/// <summary>
/// 入口点函数,使用GPU修改亮度
/// </summary>
/// <param name="pic">像素起始地址<br/>像素格式:RGB(24bit)</param>
/// <param name="width">宽度</param>
/// <param name="height">高度</param>
/// <param name="stride">扫描宽度</param>
/// <param name="brightness">亮度(0~100)</param>
/// <returns>是否执行成功</returns>
extern "C" _declspec(dllexport) bool CUDA_SetBrightness(
    byte * pic, int width, int height, int stride, byte brightness
);

/// <summary>
/// 入口点函数,使用GPU设置透明像素背景色
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
/// 入口点函数,使用GPU压缩PNG图片
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
extern "C" _declspec(dllexport) bool CUDA_CompressPNG(
    byte * pic, int width, int height, int stride, int pixelBits, int level
);

#endif // !_CUDA_H