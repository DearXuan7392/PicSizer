#include "cppH.h"

extern "C" _declspec(dllexport) bool CPP_CompressPNG(
	byte * pic, int width, int height, int stride, int pixelBits, int level
) {
	try {
		int offset = level / 2;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int position = y * stride + x;
				if (pixelBits == 4 && x % 4 == 3 && pic[position] != 0 && pic[position] != 255) {
					continue;
				}
				else {
					int result = pic[position] / level * level + offset;
					pic[position] = result <= 255
						? (byte)result
						: (byte)255;
				}
			}
		}
		return true;
	}
	catch (...) {
		return false;
	}
}