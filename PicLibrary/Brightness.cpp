#include "cppH.h"

extern "C" _declspec(dllexport) bool CPP_SetBrightness(
    byte * pic, int width, int height, int stride, byte brightness
) {
    try {
        int x, y;
        byte* p, * r, * g, * b;
        for (x = 0; x < width; x++) {
            for (y = 0; y < height; y++) {
                p = pic + y * stride + x * 3;
                r = p;
                g = p + 1;
                b = p + 2;
                *r = *r * brightness / 100;
                *g = *g * brightness / 100;
                *b = *b * brightness / 100;
            }
        }
        return true;
    }
    catch (...) {
        return false;
    }

}