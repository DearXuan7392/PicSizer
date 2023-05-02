#include "cudaH.cuh"

__global__ void _SetBrightnessByKernel(
    byte* pic, int width, int height, int stride, byte brightness
) {
    int i = blockIdx.x * (*Device_MaxThreads) + threadIdx.x; // 第i个像素
    if (i >= width * height) return;
    int x = i % width;//x坐标
    int y = i / width;//y坐标 
    byte* p = pic + y * stride + x * 3; // 第i个像素的起始地址
    byte* r = p;
    byte* g = p + 1;
    byte* b = p + 2;
    *r = *r * brightness / 100;
    *g = *g * brightness / 100;
    *b = *b * brightness / 100;
}

extern "C" _declspec(dllexport) bool CUDA_SetBrightness(
    byte * pic, int width, int height, int stride, byte brightness
) {
    int size = stride * height;
    byte* dev_pic;
    // 预分配显存
    if (cudaMalloc((void**)&dev_pic, size) != cudaSuccess) {
        goto OnError;
    }
    if (cudaMemcpy(dev_pic, pic, size, cudaMemcpyHostToDevice) != cudaSuccess) {
        goto OnError;
    }
    _SetBrightnessByKernel << <width * height / Host_MaxThreads + 1, Host_MaxThreads >> > (dev_pic, width, height, stride, brightness);
    if (cudaMemcpy(pic, dev_pic, size, cudaMemcpyDeviceToHost) != cudaSuccess) {
        goto OnError;
    }
    if (cudaFree(dev_pic) != cudaSuccess) {
        goto OnError;
    }
    return true;
OnError:
    cudaFree(dev_pic);
    return false;
}