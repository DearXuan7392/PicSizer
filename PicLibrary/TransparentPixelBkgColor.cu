#include "cudaH.cuh"

__global__ void _SetTransparentPixelBackgroundColorByKernel(
    byte* pic, int width, int height, int stride, byte R, byte G, byte B
) {
    int i = blockIdx.x * (*Device_MaxThreads) + threadIdx.x; // 第i个像素
    if (i >= width * height) return;
    int x = i % width;//x坐标
    int y = i / width;//y坐标 
    byte* p = pic + y * stride + x * 4; // 第i个像素的起始地址
    byte* a = p + 3;
    byte* r = p + 2;
    byte* g = p + 1;
    byte* b = p;
    if (*a == 255) return;
    *r = (*r * *a + R * (255 - *a)) / 255;
    *g = (*g * *a + G * (255 - *a)) / 255;
    *b = (*b * *a + B * (255 - *a)) / 255;
    *a = 255;
}

extern "C" _declspec(dllexport) bool CUDA_SetTransparentPixelBackgroundColor(
    byte * pic, int width, int height, int stride, byte R, byte G, byte B
) {
    int size = stride * height;
    int totalPixel = width * height;
    byte* dev_pic;
    if (cudaMalloc((void**)&dev_pic, size) != cudaSuccess) {
        goto OnError;
    }
    if (cudaMemcpy(dev_pic, pic, size, cudaMemcpyHostToDevice) != cudaSuccess) {
        goto OnError;
    }
    _SetTransparentPixelBackgroundColorByKernel << <totalPixel / Host_MaxThreads + 1, Host_MaxThreads >> > (dev_pic, width, height, stride, R, G, B);
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