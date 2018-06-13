using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorldheightsGenerator : MonoBehaviour, HeightsGenerator {

    private float maxx_height = -100;
    private float min_height = 100;

    public Color min_color, max_color;

    public float heightScale = 10f;
    public float scale = .1f;
    public int octaves = 1;
    public float persistance = 1;
    public float lacunarity = 0.5f;

    public bool randomizeColors;
    public float randomizeFactor = .01f;

    public float maxRandomHeightOffset = .05f;

    public Color GetColor(int x, int z, float height)
    {
        Color c = Color.Lerp(min_color, max_color,  ((height - min_height)  / (maxx_height - min_height)));

        if(randomizeColors)
        {
            float offset = Random.Range(-randomizeFactor / 2f, randomizeFactor / 2f);
            c.r += offset;
            c.g += offset;
            c.b += offset;
        }

        return c;
    }

    public float GetHeight(int x, int z)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for(int i = 0; i < octaves; i++)
        {
            float sampleX = x * frequency * scale;
            float sampleY = z * frequency * scale;

            float perlin = Mathf.PerlinNoise(sampleX, sampleY);
            noiseHeight += perlin * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        //noiseHeight = Mathf.Clamp(noiseHeight, 0, max_height);

        //noiseHeight = Mathf.InverseLerp(0, max_height, noiseHeight);
        float height = noiseHeight * heightScale;

        if (height > maxx_height)
        {
            maxx_height = height;
        }
        else if (height < min_height)
            min_height = height;

        //Debug.Log("H: " + height);
        return height + Random.Range(0f, maxRandomHeightOffset);
    }

}
