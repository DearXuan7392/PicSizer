#include "cudaH.cuh"

__global__ void _CompressPNGByKernel(
	byte* pic, int width, int height, int stride, int pixelBits, int level
) {
    // 像素序号
    int i = blockIdx.x * (*Device_MaxThreads) + threadIdx.x;
    if (i >= width * height ) return;
    int x = i % width; // x 坐标
    int y = i / width; // y 坐标
    int offset = level / 2;
    // 第 i 个像素起始地址
    byte* p = pic + y * stride + x * pixelBits;
    for (int j = 0; j < 3; j++) {
        int result = *p / level * level + offset;
        *p = result <= 255
            ? (byte)result
            : (byte)255;
        p++;
    }
    // 包含透明通道的图片需要额外处理
    if (pixelBits == 4 && *p != 255 && *p != 0) {
        int result = *p / level * level + offset;
        *p = result <= 255
            ? (byte)result
            : (byte)255;
    }
}

extern "C" _declspec(dllexport) bool CUDA_CompressPNG(
	byte * pic, int width, int height, int stride, int pixelBits, int level
) {
	int size = stride * height;
    // 像素在显存中的地址
    byte* dev_pic;
    // 预分配显存
    if (cudaMalloc((void**)&dev_pic, size) != cudaSuccess) {
        goto OnError;
    }
    if (cudaMemcpy(dev_pic, pic, size, cudaMemcpyHostToDevice) != cudaSuccess) {
        goto OnError;
    }
    _CompressPNGByKernel << < width * height / Host_MaxThreads + 1, Host_MaxThreads >> >
        (dev_pic, width, height, stride, pixelBits, level);
    // 将显存复制回内存
    if (cudaMemcpy(pic, dev_pic, size, cudaMemcpyDeviceToHost) != cudaSuccess) {
        goto OnError;
    }
    // 释放显存
    if (cudaFree(dev_pic) != cudaSuccess) {
        goto OnError;
    }
    return true;
// 错误处理
OnError:
    cudaFree(dev_pic);
    return false;
}