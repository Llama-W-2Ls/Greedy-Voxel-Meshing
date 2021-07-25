using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures : MonoBehaviour
{
    public static Textures instance;

    public Texture[] textures;
    public Dictionary<BlockType, Dictionary<BlockFace, Vector2[]>> TextureDict;

    void Awake()
    {
        instance = this;

        Initialize();
    }

    void Initialize()
    {
        TextureDict = new Dictionary<BlockType, Dictionary<BlockFace, Vector2[]>>();

        foreach (var texture in textures)
        {
            texture.Initialize();
            TextureDict.Add(texture.Type, texture.UVs);
        }
    }
}

[System.Serializable]
public struct Texture
{
    public BlockType Type;
    public Face[] Faces;

    public Dictionary<BlockFace, Vector2[]> UVs;

    public void Initialize()
    {
        UVs = new Dictionary<BlockFace, Vector2[]>();

        foreach (var face in Faces)
        {
            var adjustedUVs = new Vector2[face.UVs.Length];

            for (int i = 0; i < adjustedUVs.Length; i++)
            {
                adjustedUVs[i] = face.UVs[i] / 8f;
            }

            UVs.Add(face.blockFace, adjustedUVs);
        }
    }
}

[System.Serializable]
public struct Face
{
    public BlockFace blockFace;
    public Vector2[] UVs;
}

public enum BlockFace
{
    Left,
    Right,
    Top,
    Bottom,
    Front,
    Back
}