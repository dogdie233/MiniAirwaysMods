using Unity.Collections;

using UnityEngine;

namespace GiveMeAnUpgrade;

public static class Helper
{
    public static Texture2D GenerateUpgradeImage()
    {
        int size = 512, xOffset = -54;

        var image = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixelData = new NativeArray<Color32>(size * size, Allocator.Temp, NativeArrayOptions.ClearMemory);
        for (int x = 216; x < 296; x++)
            for (int y = 16; y < 400; y++)
                pixelData[y * size + x + xOffset] = new Color32(255, 255, 255, 255);

        for (int x = 64; x < 256; x++)
            for (int y = 128; y < 232; y++)
                pixelData[(y + x) * size + x + xOffset] = new Color32(255, 255, 255, 255);

        for (int x = size - 1 - 64; x >= 256; x--)
            for (int y = 128; y < 232; y++)
                pixelData[(y + size - 1 - x) * size + x + xOffset] = new Color32(255, 255, 255, 255);

        image.SetPixelData(pixelData, 0);
        image.Apply(false, true);
        return image;
    }
}
