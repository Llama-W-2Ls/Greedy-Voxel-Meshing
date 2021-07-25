using UnityEngine;

public static class Noise
{
    public static float GetValue(float x, float y, NoiseSettings settings)
    {
        #region Random Offset

        System.Random randNumGen = new System.Random(settings.Seed);

        float offsetX = randNumGen.Next(-10000, 10000) + settings.Offset.x;
        float offsetY = randNumGen.Next(-10000, 10000) + settings.Offset.y;

        #endregion

        float sampleX = x / settings.Scale + offsetX;
        float sampleY = y / settings.Scale + offsetY;

        float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

        return noiseValue * settings.HeightScale;
    }
}

[System.Serializable]
public struct NoiseSettings
{
    public float Scale, HeightScale;
    public int Seed;
    public Vector2 Offset;
}
