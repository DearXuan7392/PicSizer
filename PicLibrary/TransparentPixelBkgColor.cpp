#include "cppH.h"

extern "C" _declspec(dllexport) bool CPP_SetTransparentPixelBackgroundColor(
    byte * pic, int width, int height, int stride, byte R, byte G, byte B
) {
    try {
        int x, y;
        byte* p, * r, * g, * b, * a;
        for (x = 0; x < width; x++) {
            for (y = 0; y < height; y++) {
                p = pic + y * stride + x * 4;
                a = p + 3;
                if (*a == 255) continue;
                r = p + 2;
                g = p + 1;
                b = p + 0;
                *r = (*r * *a + R * (255 - *a)) / 255;
                *g = (*g * *a + G * (255 - *a)) / 255;
                *b = (*b * *a + B * (255 - *a)) / 255;
                *a = 255;
            }
        }
        return true;
    }
    catch (...) {
        return false;
    }

}