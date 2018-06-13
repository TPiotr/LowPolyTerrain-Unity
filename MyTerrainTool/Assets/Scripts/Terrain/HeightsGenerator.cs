using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HeightsGenerator {

    float GetHeight(int x, int z);

    Color GetColor(int x, int z, float height);
}

public class FlatHeightsGenerator : HeightsGenerator
{
    public float GetHeight(int x, int z)
    {
        return 0;
    }

    public Color GetColor(int x, int z, float height)
    {
        return Color.white;
    }
}

public class RandomHeightsGenerator : HeightsGenerator
{
    public float noise_scale = .1f;
    public float height_scale = 4;

    public float GetHeight(int x, int z)
    {
        float h = Mathf.PerlinNoise(x * noise_scale, z * noise_scale);
        return h * height_scale;
    }

    public Color GetColor(int x, int z, float height)
    {
        return Color.white;
    }
}