using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Vector3Int Pos;
    public BlockType Type;

    public Block(Vector3Int pos, BlockType type)
    {
        Pos = pos;
        Type = type;
    }
}

public enum BlockType
{
    Air,
    Grass
}
