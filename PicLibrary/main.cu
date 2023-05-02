#include "cudaH.cuh"
#include <string.h>

struct PicDllResultStr {
    /// <summary>
    /// Dll是否加载成功
    /// </summary>
    bool enable;

    /// <summary>
    /// 支持的GPU数量
    /// </summary>
    int GpuCount;

    /// <summary>
    /// 当前GPU名称
    /// </summary>
    char GpuName[256];

    /// <summary>
    /// GPU最大线程数
    /// </summary>
    int GpuMaxThreadsPerBlock;

    /// <summary>
    /// GPU Grid数量
    /// </summary>
    int GpuGrid[3];

    /// <summary>
    /// GPU 错误名称
    /// </summary>
    char GpuErrorName[512];

    /// <summary>
    /// GPU 报错信息
    /// </summary>
    char GpuErrorStr[4096];
};

int GetGpuInfo(PicDllResultStr& res);

extern "C" _declspec(dllexport) int LoadDll(
    PicDllResultStr & res
) {
    res.enable = true;
    return GetGpuInfo(res);
}

/// <summary>
/// 获取GPU信息,获取成功返回 0
/// </summary>
int GetGpuInfo(PicDllResultStr& res) {
    cudaDeviceProp cudaProp; // GPU信息
    //获取GPU数量
    if (cudaGetDeviceCount(&res.GpuCount) != cudaSuccess) {
        goto OnError;
    }
    //GPU数量为0
    if (res.GpuCount == 0) {
        goto OnError;
    }
    //获取GPU属性
    if (cudaGetDeviceProperties(&cudaProp, 0) != cudaSuccess) {
        goto OnError;
    }
    //GPU最大线程数
    Host_MaxThreads = cudaProp.maxThreadsPerBlock;
    if (Host_MaxThreads > 2048) Host_MaxThreads = 2048;
    if (cudaMalloc((void**)&Device_MaxThreads, sizeof(int)) != cudaSuccess) {
        goto OnError;
    }
    if (cudaMemcpy(Device_MaxThreads, &Host_MaxThreads, sizeof(int), cudaMemcpyHostToDevice) != cudaSuccess) {
        goto OnError;
    }
    res.GpuMaxThreadsPerBlock = cudaProp.maxThreadsPerBlock;
    //Grid数
    memcpy((void*) & res.GpuGrid, (void*) &cudaProp.maxGridSize, sizeof(int) * 3);
    //GPU名称
    strcpy(res.GpuName, cudaProp.name);
    return 0;

OnError:
    cudaFree(Device_MaxThreads);
    cudaError_t error = cudaGetLastError();
    strcpy(res.GpuErrorName, cudaGetErrorName(error));
    strcpy(res.GpuErrorStr, cudaGetErrorString(error));
    return -1;
}